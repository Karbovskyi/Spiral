using UnityEngine;
using Zenject;

public class SpiralMechanicPhysics : IInitializable
{
    private readonly SpiralStats _stats;
    private float _b;
    private float _maxTheta;

    public SpiralMechanicPhysics(SpiralStats stats)
    {
        _stats = stats;
    }
    
    public void Initialize()
    {
        _b = _stats.RadiusStep / (2f * Mathf.PI);
        _maxTheta = _stats.Turns * 2f * Mathf.PI;
    }

    public bool IsBulletCollidedWithSpiral(Bullet bullet)
    {
        Vector2 closestPoint = FindTrueClosestPointOnSpiral(bullet.Position, out _);
        if (closestPoint == Vector2.positiveInfinity) return false;

        float distance = Vector2.Distance(bullet.Position, closestPoint);
        float collisionThreshold = bullet.Radius + (_stats.SpiralThickness / 2f);
        
        Vector2 vectorToSpiral = (closestPoint - bullet.Position).normalized;
        float dot = Vector2.Dot(bullet.Direction, vectorToSpiral);

        return distance <= collisionThreshold && dot > 0;
    }

    public void ReflectBullet(Bullet bullet)
    {
        Vector2 closestPointOnSpiral = FindTrueClosestPointOnSpiral(bullet.Position, out _);
        if (closestPointOnSpiral == Vector2.positiveInfinity) return;

        Vector2 normal = (bullet.Position - closestPointOnSpiral).normalized;
        bullet.Direction = Vector2.Reflect(bullet.Direction.normalized, normal);

        float collisionThreshold = bullet.Radius + (_stats.SpiralThickness / 2f);
        float overlap = collisionThreshold - Vector2.Distance(bullet.Position, closestPointOnSpiral);
        if (overlap > 0)
        {
            bullet.Position += normal * (overlap + 0.01f);
        }
    }

    private Vector2 FindTrueClosestPointOnSpiral(Vector2 point, out float finalTheta)
    {
        // 1. Переходимо в локальні координати відносно центру спіралі
        Vector2 localPos = point - _stats.Center;

        // 2. "Розкручуємо" позицію кулі на кут, ОБЕРНЕНИЙ до повороту спіралі.
        //    Це дозволяє решті алгоритму працювати так, ніби спіраль не була повернута.
        float offsetRad = -_stats.AngleOffsetDeg * Mathf.Deg2Rad; // ВАЖЛИВО: ЗНАК МІНУС
        float cos = Mathf.Cos(offsetRad);
        float sin = Mathf.Sin(offsetRad);
        
        Vector2 unrotatedLocalPos = new Vector2(
            localPos.x * cos - localPos.y * sin,
            localPos.x * sin + localPos.y * cos
        );

        // 3. Використовуємо цю "розкручену" позицію для початкової оцінки theta
        float r = unrotatedLocalPos.magnitude;
        
        float thetaEstimate = 0f;
        if (_b != 0)
        {
            thetaEstimate = (r - _stats.RadiusStart) / _b;
        }
        
        thetaEstimate = Mathf.Clamp(thetaEstimate, 0, _maxTheta);
        
        float searchTheta = thetaEstimate;
        int iterations = 10;

        for (int i = 0; i < iterations; i++)
        {
            Vector2 spiralPoint = GetSpiralPoint(searchTheta);
            Vector2 tangent = GetSpiralTangent(searchTheta);

            if (tangent.sqrMagnitude < 0.0001f) break;
            
            // ВАЖЛИВО: тут ми порівнюємо реальну позицію кулі 'point'
            // з реальною позицією точки на повернутій спіралі 'spiralPoint'.
            float dot = Vector2.Dot(point - spiralPoint, tangent);
            
            searchTheta += dot / tangent.sqrMagnitude;

            if (searchTheta < 0 || searchTheta > _maxTheta)
            {
                searchTheta = Mathf.Clamp(searchTheta, 0, _maxTheta);
                break;
            }
        }
        
        finalTheta = Mathf.Clamp(searchTheta, 0, _maxTheta);

        Vector2 finalPoint = GetSpiralPoint(finalTheta);
        float maxRadius = _stats.RadiusStart + _b * _maxTheta + _stats.SpiralThickness;
        if (localPos.magnitude > maxRadius * 2) 
        {
            return Vector2.positiveInfinity;
        }

        return finalPoint;
    }

    private Vector2 GetSpiralPoint(float theta)
    {
        float r = _stats.RadiusStart + _b * theta;
        float angle = _stats.AngleOffsetDeg * Mathf.Deg2Rad;
        angle += _stats.Clockwise ? -theta : theta;

        return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle)) + _stats.Center;
    }
    
    private Vector2 GetSpiralTangent(float theta)
    {
        float r = _stats.RadiusStart + _b * theta;
        float dr_dtheta = _b;

        float angle = _stats.AngleOffsetDeg * Mathf.Deg2Rad;
        angle += _stats.Clockwise ? -theta : theta;
        float d_angle_d_theta = _stats.Clockwise ? -1f : 1f;

        float cos_a = Mathf.Cos(angle);
        float sin_a = Mathf.Sin(angle);

        float dx_dtheta = dr_dtheta * cos_a - r * sin_a * d_angle_d_theta;
        float dy_dtheta = dr_dtheta * sin_a + r * cos_a * d_angle_d_theta;
        
        return new Vector2(dx_dtheta, dy_dtheta);
    }
}