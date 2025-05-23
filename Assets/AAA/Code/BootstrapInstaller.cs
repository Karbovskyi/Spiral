using Shapes;
using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public SpiralStats SpiralStats;
    public ShooterStats ShooterStats;
    public BulletStats BulletStats;
    public Transform ShooterTransform;
    public Transform ShotStartPosition;
    public Polyline SpiralPolyline;
    public Camera Camera;
    public ArrowDrawer ArrowDrawer;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(Camera);
        Container.BindInterfacesTo<SpiralGenerator>().AsSingle().WithArguments(SpiralStats, SpiralPolyline);
        Container.BindInterfacesTo<BulletFactory>().AsSingle().WithArguments(BulletStats);
        Container.BindInterfacesAndSelfTo<SpiralMechanicPhysics>().AsSingle().WithArguments(SpiralStats);
        Container.BindInterfacesTo<ShootingService>().AsSingle();
        Container.BindInterfacesTo<Shooter>().AsSingle().WithArguments(ShooterStats, ShooterTransform, ShotStartPosition);
    }
}

/*
public class SpiralMechanicPhysics
{
    private readonly SpiralGenerator _spiral;

    public SpiralMechanicPhysics(SpiralGenerator spiralGenerator)
    {
        _spiral = spiralGenerator;
    }

    public bool IsBulletCollidedWithSpiral(Bullet bullet)
    {
        Vector2 pos = bullet.transform.position;
        float distance = GetDistanceFromSpiral(pos, out float r, out float spiralRadius);

        float halfThickness = _spiral.spiralThickness / 2f;
        float exitBuffer = 0.01f;

        if (!bullet.IsInSpiralZone)
        {
            if (distance <= halfThickness)
            {
                bullet.IsInSpiralZone = true;
                return true;
            }
        }
        else
        {
            if (distance > halfThickness + exitBuffer)
            {
                bullet.IsInSpiralZone = false;
            }
        }

        return false;
    }

    public void ReflectBullet(Bullet bullet)
    {
        Vector2 pos = bullet.transform.position;
        Vector2 localPos = pos - _spiral.center;

        float r = localPos.magnitude;
        float theta = Mathf.Atan2(localPos.y, localPos.x);
        float angleOffsetRad = _spiral.angleOffsetDeg * Mathf.Deg2Rad;
        theta -= angleOffsetRad;
        if (theta < 0) theta += 2 * Mathf.PI;
        if (_spiral.clockwise)
            theta = 2 * Mathf.PI - theta;

        int revolutions = Mathf.FloorToInt((r - _spiral.radiusStart) / _spiral.radiusStep);
        revolutions = Mathf.Max(0, revolutions);

        float totalTheta = theta + revolutions * 2 * Mathf.PI;

        float a = _spiral.radiusStart;
        float b = _spiral.radiusStep / (2f * Mathf.PI);
        float spiralR = a + b * totalTheta;

        float angle = _spiral.clockwise
            ? 2 * Mathf.PI - (totalTheta % (2 * Mathf.PI)) + angleOffsetRad
            : (totalTheta % (2 * Mathf.PI)) + angleOffsetRad;

        float dx = b * Mathf.Cos(angle) - spiralR * Mathf.Sin(angle);
        float dy = b * Mathf.Sin(angle) + spiralR * Mathf.Cos(angle);

        Vector2 tangent = new Vector2(dx, dy);
        Vector2 normal = new Vector2(-tangent.y, tangent.x).normalized;

        bullet.Direction = Reflect(bullet.Direction.normalized, normal);
    }

    private float GetDistanceFromSpiral(Vector2 position, out float r, out float spiralRadius)
    {
        Vector2 localPos = position - _spiral.center;

        r = localPos.magnitude;
        float theta = Mathf.Atan2(localPos.y, localPos.x);
        float angleOffsetRad = _spiral.angleOffsetDeg * Mathf.Deg2Rad;
        theta -= angleOffsetRad;

        if (theta < 0) theta += 2 * Mathf.PI;
        if (_spiral.clockwise)
            theta = 2 * Mathf.PI - theta;

        int revolutions = Mathf.FloorToInt((r - _spiral.radiusStart) / _spiral.radiusStep);
        revolutions = Mathf.Max(0, revolutions);

        float totalTheta = theta + revolutions * 2 * Mathf.PI;
        float maxTheta = _spiral.turns * 2f * Mathf.PI;

        if (totalTheta > maxTheta)
        {
            spiralRadius = 0;
            return float.MaxValue;
        }

        float a = _spiral.radiusStart;
        float b = _spiral.radiusStep / (2f * Mathf.PI);
        spiralRadius = a + b * totalTheta;

        return Mathf.Abs(r - spiralRadius);
    }

    private Vector2 Reflect(Vector2 velocity, Vector2 normal)
    {
        return velocity - 2 * Vector2.Dot(velocity, normal) * normal;
    }
}
*/