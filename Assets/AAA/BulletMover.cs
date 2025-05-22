using System.Collections.Generic;
using UnityEngine;

public class BulletMover : MonoBehaviour
{
    public SpiralGenerator SpiralGenerator;
    public List<Bullet> Bullets;

    private void Update()
{
    foreach (Bullet bullet in Bullets)
    {
        Vector3 oldPos = bullet.transform.position;
        bullet.transform.position += (Vector3)(bullet.Direction.normalized * bullet.Speed * Time.deltaTime);

        if (bullet.IsCanCollide && IsCollidingWithSpiral(
            bullet.transform.position,
            SpiralGenerator.center,
            SpiralGenerator.radiusStart,
            SpiralGenerator.radiusStep,
            SpiralGenerator.spiralThickness,
            SpiralGenerator.angleOffsetDeg,
            SpiralGenerator.clockwise,
            SpiralGenerator.turns))
        {
            // Знаходимо totalTheta для кулі, аналогічно IsCollidingWithSpiral
            Vector2 localPos = (Vector2)bullet.transform.position - SpiralGenerator.center;

            float r = localPos.magnitude;
            float theta = Mathf.Atan2(localPos.y, localPos.x);
            float angleOffsetRad = SpiralGenerator.angleOffsetDeg * Mathf.Deg2Rad;
            theta -= angleOffsetRad;
            if (theta < 0) theta += 2 * Mathf.PI;
            if (SpiralGenerator.clockwise)
                theta = 2 * Mathf.PI - theta;

            int revolutions = Mathf.FloorToInt((r - SpiralGenerator.radiusStart) / SpiralGenerator.radiusStep);
            revolutions = Mathf.Max(0, revolutions);

            float totalTheta = theta + revolutions * 2 * Mathf.PI;

            // Обчислення точки на спіралі
            float a = SpiralGenerator.radiusStart;
            float b = SpiralGenerator.radiusStep / (2f * Mathf.PI);
            float spiralRadius = a + b * totalTheta;
            float angle = SpiralGenerator.clockwise ?
                2 * Mathf.PI - (totalTheta % (2 * Mathf.PI)) + angleOffsetRad :
                (totalTheta % (2 * Mathf.PI)) + angleOffsetRad;

            Vector2 spiralPoint = SpiralGenerator.center + new Vector2(
                spiralRadius * Mathf.Cos(angle),
                spiralRadius * Mathf.Sin(angle)
            );

            // Похідні для дотичної
            float dx = b * Mathf.Cos(angle) - spiralRadius * Mathf.Sin(angle);
            float dy = b * Mathf.Sin(angle) + spiralRadius * Mathf.Cos(angle);

            Vector2 tangent = new Vector2(dx, dy);
            Vector2 normal = new Vector2(-tangent.y, tangent.x).normalized;

            // Відбиваємо напрямок кулі
            Vector2 newDirection = ReflectDirection(bullet.Direction.normalized, normal);
            bullet.Direction = newDirection;

            bullet.IsCollided(); // скидаємо таймер колізії
        }
    }
}
    
    public Vector2 ReflectDirection(Vector2 velocity, Vector2 normal)
    {
        return velocity - 2 * Vector2.Dot(velocity, normal) * normal;
    }
    
    bool IsCollidingWithSpiral(
        Vector2 position,
        Vector2 spiralCenter,
        float radiusStart,
        float radiusStep,
        float spiralThickness,
        float angleOffsetDeg,
        bool clockwise,
        float turns
    )
    {
        Vector2 localPos = position - spiralCenter;

        float x = localPos.x;
        float y = localPos.y;

        float r = Mathf.Sqrt(x * x + y * y);
        float theta = Mathf.Atan2(y, x);

        float angleOffsetRad = angleOffsetDeg * Mathf.Deg2Rad;
        theta -= angleOffsetRad;

        if (theta < 0)
            theta += 2 * Mathf.PI;

        if (clockwise)
            theta = 2 * Mathf.PI - theta;

        int revolutions = Mathf.FloorToInt((r - radiusStart) / radiusStep);
        revolutions = Mathf.Max(0, revolutions);

        float totalTheta = theta + revolutions * 2 * Mathf.PI;

        float maxTheta = turns * 2f * Mathf.PI;
        if (totalTheta > maxTheta)
            return false;

        float a = radiusStart;
        float b = radiusStep / (2f * Mathf.PI);

        float spiralRadius = a + b * totalTheta;

        float distanceFromSpiral = Mathf.Abs(r - spiralRadius);

        return distanceFromSpiral <= spiralThickness / 2f;
    }
}