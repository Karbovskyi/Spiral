using UnityEngine;
using Zenject;

public interface IShooter
{
}


public interface IInputService
{
    bool TryGetDirectionToTouch(Vector2 startPosition, out Vector2 direction);
}

public class InputService : IInputService
{
    private readonly Camera _camera;

    public InputService(Camera camera)
    {
        _camera = camera;
    }
    public bool TryGetDirectionToTouch(Vector2 startPosition, out Vector2 direction)
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            
            Vector3 worldPosition3D = _camera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, _camera.nearClipPlane)
            );
            
            Vector2 worldPosition2D = worldPosition3D;
            
            direction = worldPosition2D - startPosition;

            // 4. Перевіряємо, чи є напрямок (щоб уникнути ділення на нуль, якщо клік був у startPosition)
            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
                return true;
            }
        }
        
        direction = Vector2.zero;
        return false;
    }
}

public class Shooter : ITickable, IShooter
{
    private readonly IInputService _inputService;
    private readonly IShootingService _shootingService;
    private readonly ShooterStats _shooterStats;
    private readonly Transform _shooterTransform;
    private readonly Transform _shotStartPosition;

    private float _timerToShot;
    
    public Shooter(IInputService inputService, IShootingService shootingService, ShooterStats shooterStats, Transform shooterTransform, Transform shotStartPosition)
    {
        _inputService = inputService;
        _shootingService = shootingService;
        _shooterStats = shooterStats;
        _shooterTransform = shooterTransform;
        _shotStartPosition = shotStartPosition;
        //_timerToShot = shooterStats.ShootingSpeed;
    }
    
    public void Tick()
    {
        if (_inputService.TryGetDirectionToTouch(_shooterTransform.position, out Vector2 direction))
        {
            Move(direction);
            Fire();
        }
    }

    private void Move(Vector2 targetDirection)
    {
        //_shooterTransform.Rotate(0f, 0f, 10 * Time.deltaTime);

        // 1. Визначаємо цільовий кут з вектора напрямку.
        // Atan2 повертає кут в радіанах, конвертуємо його в градуси.
        // Додаємо -90, тому що в Unity кут 0 градусів - це вісь X (праворуч),
        // а ми хочемо, щоб об'єкт дивився "вгору" (transform.up).
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;

        // 2. Створюємо цільовий поворот (Quaternion) з цього кута
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        // 3. Плавно повертаємо поточний об'єкт до цільового повороту
        // Quaternion.RotateTowards забезпечує поворот з постійною кутовою швидкістю.
        _shooterTransform.rotation = Quaternion.RotateTowards(
            _shooterTransform.rotation, 
            targetRotation, 
            _shooterStats.RotatingSpeed * Time.deltaTime
        );
        
    }

    private void Fire()
    {
        _timerToShot -= Time.deltaTime;

        if (_timerToShot <= 0)
        {
            _timerToShot = _shooterStats.ShootingInterval;
            _shootingService.Shot(_shotStartPosition.position, _shotStartPosition.up);
        }
    }
}