using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpiralStats", menuName = "Stats/SpiralStats")]
public class SpiralStats : ScriptableObject
{
    public Gradient FillGradient;

    public float RadiusStart = 1f;
    public float RadiusStep = 1f;
    public float SpiralThickness = 0.1f;
    public float Turns = 5f;
    public bool Clockwise = true;
    public Vector2 Center = Vector2.zero;
    public float AngleOffsetDeg = 0f;
    [FormerlySerializedAs("AngleOffsetDegChangeSpeed")] public float SpiralRotationSpeed=0;
    
    public float DetailPerUnit = 5f;

    [Tooltip("Регулює розподіл точок. 0 = Оптимізація (менше точок назовні), 1 = Якість (однакова щільність всюди)")]
    [Range(0, 1)] public float DistributionFactor = 0.5f; // Поставимо 0.5 як збалансоване значення за замовчуванням
}