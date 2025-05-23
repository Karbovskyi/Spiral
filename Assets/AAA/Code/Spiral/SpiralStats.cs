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
    
    [Header("Distribution Settings")]
    [Tooltip("Регулює розподіл точок. 0 = Оптимізація (менше точок назовні), 1 = Якість (однакова щільність всюди)")]
    [Range(0, 1)]
    public float DistributionFactor = 0.5f; // Поставимо 0.5 як збалансоване значення за замовчуванням

    public float AngleOffsetDegChangeSpeed=10;
}