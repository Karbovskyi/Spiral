using UnityEngine;

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