using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _radius;
    private Vector2 _position;

    public bool IsInSpiralZone;
    public float RemainDistance;
    
    public float Speed { get; set; }
    public Vector2 Direction { get; set; }

    

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
}