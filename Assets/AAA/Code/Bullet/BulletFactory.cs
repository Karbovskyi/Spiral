using UnityEngine;
using Object = UnityEngine.Object;

public interface IBulletFactory
{
    Bullet CreateBullet();
    void DestroyBullet(Bullet bullet);
}

public class BulletFactory : IBulletFactory
{
    private readonly BulletStats _bulletStats;
    private readonly ObjectPool<Bullet> _bulletPool;
    
    private Vector3 _releasedBulletPosition = new Vector3(1000, 1000, 0);

    public BulletFactory(BulletStats bulletStats)
    {
        _bulletStats = bulletStats;
        _bulletPool = new ObjectPool<Bullet>(GetBullet);
    }

    public Bullet CreateBullet()
    {
        Bullet bullet = _bulletPool.Get();
        bullet.Speed = _bulletStats.Speed;
        bullet.Radius = _bulletStats.Radius;
        bullet.RemainDistance = _bulletStats.Distance;
        return bullet;
    }

    public void DestroyBullet(Bullet bullet)
    {
        bullet.Position = _releasedBulletPosition;
        _bulletPool.Set(bullet);
    }
    
    private Bullet GetBullet()
    {
        GameObject bulletObject = Object.Instantiate(_bulletStats.Prefab);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.Position = _releasedBulletPosition;
        return bullet;
    }
}