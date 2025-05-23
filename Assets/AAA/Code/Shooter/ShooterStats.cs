using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ShooterStats", menuName = "Stats/ShooterStats")]
public class ShooterStats : ScriptableObject
{
    public float ShootingInterval = 0.5f;
    public float RotatingSpeed = 1f;
}