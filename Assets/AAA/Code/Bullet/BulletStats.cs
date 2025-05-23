using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BulletStats", menuName = "Stats/BulletStats")]
public class BulletStats : ScriptableObject
{
    public GameObject Prefab;
    public float Speed = 3;
    public float Radius = 0.2f;
    [FormerlySerializedAs("Distance")] public float DistanceToSelfDestruct = 6;
}