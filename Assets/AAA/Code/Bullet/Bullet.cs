using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed { get; set; }
    public Vector2 Direction { get; set; }
    public bool IsInSpiralContact{ get; set; }
    public float DistanceToSelfDestruct{ get; set; }
    
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            transform.position = value;
        }
    }

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            transform.SetScale2D(value);
        }
    }

    private Vector2 _position;
    private float _radius;
}