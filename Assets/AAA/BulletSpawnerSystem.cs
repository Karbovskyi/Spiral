using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class BulletSpawnerSystem : MonoBehaviour
{
    public BulletMover BulletMover;
    
    [Header("Параметри")]
    public GameObject bulletPrefab;
    public float particleSpeed = 2f;
    public float spawnInterval = 0.5f;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnParticle();
        }
    }

    private void SpawnParticle()
    {
        GameObject particle = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        Vector2 direction = Random.insideUnitCircle.normalized;

        var bullet = particle.GetComponent<Bullet>();
        bullet.Speed = particleSpeed;
        bullet.Direction = direction;
        
        BulletMover.Bullets.Add(bullet);
    }
}