using System.Collections.Generic;
using UnityEngine;
using Zenject;


public interface IShootingService
{
    void Shot(Vector2 position, Vector2 direction);
}

public class ShootingService : ITickable, IShootingService
{
    private readonly LinkedList<Bullet> _bullets = new LinkedList<Bullet>();
    private readonly IBulletFactory _bulletFactory;
    private readonly SpiralMechanicPhysics _spiralPhysics;

    public ShootingService(IBulletFactory bulletFactory, SpiralMechanicPhysics spiralPhysics)
    {
        _bulletFactory = bulletFactory;
        _spiralPhysics = spiralPhysics;
    }

    public void Tick()
    {
        UpdateBulletsPositions();
    }
    
    public void Shot(Vector2 position, Vector2 direction)
    {
        Bullet bullet = _bulletFactory.CreateBullet();
        bullet.Position = position;
        bullet.Direction = direction;
        _bullets.AddFirst(bullet);
    }

    private void UpdateBulletsPositions()
    {
        LinkedListNode<Bullet> node = _bullets.First;

        while (node != null)
        {
            LinkedListNode<Bullet> currentNode = node;
            node = currentNode.Next;
            Bullet bullet = currentNode.Value;
            
            if (bullet.RemainDistance <= 0)
            {
                _bullets.Remove(currentNode);
                _bulletFactory.DestroyBullet(bullet);
            }
            else
            {
                float distance = bullet.Speed * Time.deltaTime;
                bullet.Position += bullet.Direction * distance;
                bullet.RemainDistance -= distance;
                
                if (!bullet.IsInSpiralZone)
                {
                    if (_spiralPhysics.IsBulletCollidedWithSpiral(bullet))
                    {
                        bullet.IsInSpiralZone = true;
                        _spiralPhysics.ReflectBullet(bullet);
                        //bullet.OnCollision(); // якщо є якась логіка після зіткнення
                    }
                }
                else
                {
                    
                    if (!_spiralPhysics.IsBulletCollidedWithSpiral(bullet))
                    {
                        bullet.IsInSpiralZone = false;   
                    }
                }
            }
        }
    }
}