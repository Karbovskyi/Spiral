using UnityEngine;

public static class TransformExtensions
{
    public static void SetScale2D(this Transform transform, float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1);
    }
}