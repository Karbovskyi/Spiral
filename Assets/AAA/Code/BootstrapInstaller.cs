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
        
        // For Testing
        Container.BindInstance(SpiralStats).AsSingle();
        Container.BindInstance(ShooterStats).AsSingle();
        Container.BindInstance(BulletStats).AsSingle();
    }
}