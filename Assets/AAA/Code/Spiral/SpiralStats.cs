using UnityEngine;

[CreateAssetMenu(fileName = "SpiralStats", menuName = "Stats/SpiralStats")]
public class SpiralStats : ScriptableObject
{
    public Gradient FillGradient;
    public float Turns = 5f;
    public float RadiusStart = 1f;
    public float RadiusStep = 1f;
    public float SpiralThickness = 0.1f;
    public float DetailPerUnit = 5f;
    public bool Clockwise = true;
    public float AngleOffsetDeg = 0f;
    public Vector2 Center = Vector2.zero;
}