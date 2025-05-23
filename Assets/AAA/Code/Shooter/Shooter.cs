using UnityEngine;
using Zenject;

public interface IShooter { }

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
    }
    
    public void Tick()
    {
        if (_shooterStats.AutoMove)
        {
            AutoMove();
            Fire();
            return;
        }
        
        if (_inputService.TryGetDirectionToTouch(_shooterTransform.position, out Vector2 direction))
        {
            Move(direction);
            Fire();
        }
    }

    private void AutoMove()
    {
        _shooterTransform.Rotate(Vector3.forward, _shooterStats.RotatingSpeed * Time.deltaTime);
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

    private void Move(Vector2 targetDirection)
    {
        // 1. Визначаємо цільовий кут з вектора напрямку.
        // Atan2 повертає кут в радіанах, конвертуємо його в градуси.
        // Додаємо -90, тому що в Unity кут 0 градусів - це вісь X (праворуч),
        // а ми хочемо, щоб об'єкт дивився "вгору" (transform.up).
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        
        _shooterTransform.rotation = Quaternion.RotateTowards(
            _shooterTransform.rotation, 
            targetRotation, 
            _shooterStats.RotatingSpeed * Time.deltaTime
        );
    }
}