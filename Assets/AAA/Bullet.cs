using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public Vector2 Direction;


    private float afterCollisionTime = 0.75f;
    private float timer = 0;

    public void IsCollided()
    {
        timer = afterCollisionTime;
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.position = transform.position;
        s.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    
    private void Update()
    {
        timer -= Time.deltaTime;
    }
    
    public bool IsCanCollide => timer <= 0;
}