using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TestingStats", menuName = "Stats/TestingStats")]
public class TestingStats : ScriptableObject
{
    [Header("Spiral")]
    public float RadiusStart = 1f;
    public float RadiusStep = 1f;
    public float SpiralThickness = 0.2f;
    public bool Clockwise = true;
    public Vector2 Center = Vector2.zero;
    public float AngleOffsetDeg = 0f;
    [FormerlySerializedAs("AngleOffsetDegChangeSpeed")] public float SpiralRotationSpeed=0;
    
    
    [Header("Bullet")]
    public float BulletSpeed = 3;
    public float BulletDistance = 100;
    public float BulletRadius = 0.2f;
    
    [Header("Shooter")]
    public float ShootingInterval = 0.1f;
    public float RotatingSpeed = 400f;
    public bool AutoMove = true;
}