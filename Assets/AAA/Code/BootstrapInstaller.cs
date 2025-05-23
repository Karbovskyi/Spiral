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
        Application.targetFrameRate = 1000;
        
        Container.BindInstance(SpiralStats).AsSingle();
        Container.BindInstance(ShooterStats).AsSingle();
        Container.BindInstance(BulletStats).AsSingle();
        
        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(Camera);
        Container.BindInterfacesTo<SpiralGenerator>().AsSingle().WithArguments( SpiralPolyline);
        Container.BindInterfacesTo<BulletFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpiralMechanicPhysics>().AsSingle();
        Container.BindInterfacesTo<ShootingService>().AsSingle();
        Container.BindInterfacesTo<Shooter>().AsSingle().WithArguments(ShooterTransform, ShotStartPosition);
    }
}