using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using Zenject;

public interface ISpiralGenerator
{
    void GenerateSpiral();
}
public class SpiralGenerator : IInitializable, ISpiralGenerator, ITickable
{
    private readonly SpiralStats _stats;
    private readonly Polyline _spiralPolyline;

    public SpiralGenerator(SpiralStats spiralStats, Polyline spiralPolyline)
    {
        _stats = spiralStats;
        _spiralPolyline = spiralPolyline;
    }
    
    public void Initialize()
    {
        GenerateSpiral();
    }
    
    public void GenerateSpiral()
    {
        List<Vector2> dots = GenerateSpiralPoints();
        
        _spiralPolyline.points.Clear();
        
        if (dots.Count < 2) return;

        float stepCount = dots.Count;
        for (var i = 0; i < dots.Count; i++)
        {
            float gradientPosition = i / (stepCount - 1);
            Color color = _stats.FillGradient.Evaluate(gradientPosition);
            _spiralPolyline.AddPoint(dots[i], color);
        } 
        
        _spiralPolyline.Thickness = _stats.SpiralThickness;
        _spiralPolyline.meshOutOfDate = true;
    }
    
    private List<Vector2> GenerateSpiralPoints()
    {
        List<Vector2> result = new();

        if (_stats.Turns <= 0 || _stats.DetailPerUnit <= 0)
        {
            return result;
        }

        float thetaMax = _stats.Turns * 2f * Mathf.PI;
        float b = _stats.RadiusStep / (2f * Mathf.PI);
        float stepSize = 1f / _stats.DetailPerUnit;

        float theta = 0f;
        while (theta <= thetaMax)
        {
            result.Add(GetWorldSpiralPoint(theta, b));
            
            if (theta >= thetaMax) break;
            
            float r = _stats.RadiusStart + b * theta;
            float effectiveRadius = Mathf.Max(0.1f, r); // Захист від ділення на нуль

            // --- ОСНОВНА ЛОГІКА З КОЕФІЦІЄНТОМ ---

            // 1. Розраховуємо крок для "Оптимізації за кривиною" (постійний кутовий крок).
            // Щоб крок був адекватним, нормуємо його до початкового радіуса.
            float thetaStepOptimized = stepSize / Mathf.Max(0.1f, _stats.RadiusStart);

            // 2. Розраховуємо крок для "Стабільної візуальної якості" (постійний крок по довжині).
            float thetaStepQuality = stepSize / effectiveRadius;

            // 3. Змішуємо два підходи за допомогою вашого коефіцієнта DistributionFactor.
            float thetaStep = Mathf.Lerp(thetaStepOptimized, thetaStepQuality, _stats.DistributionFactor);

            theta += thetaStep;
        }

        return result;
    }
    
    private Vector2 GetWorldSpiralPoint(float theta, float b)
    {
        float r = _stats.RadiusStart + b * theta;
        
        float direction = _stats.Clockwise ? -1f : 1f;
        float angle = direction * theta + (_stats.AngleOffsetDeg * Mathf.Deg2Rad);

        float x = r * Mathf.Cos(angle) + _stats.Center.x;
        float y = r * Mathf.Sin(angle) + _stats.Center.y;

        return new Vector2(x, y);
    }

    public void Tick()
    {

        if(_stats.AngleOffsetDegChangeSpeed == 0) return;
        
            // 1. Змінюємо кут зсуву з часом
            _stats.AngleOffsetDeg += _stats.AngleOffsetDegChangeSpeed * Time.deltaTime;

            // 2. (Опціонально) "Зациклюємо" кут, щоб він не ріс до нескінченності
            if (_stats.AngleOffsetDeg > 360f)
            {
                _stats.AngleOffsetDeg -= 360f;
            }
            else if (_stats.AngleOffsetDeg < -360f)
            {
                _stats.AngleOffsetDeg += 360f;
            }

            // 3. Даємо команду генератору перемалювати спіраль з новим кутом
            _spiralPolyline.transform.parent.rotation = Quaternion.Euler(0, 0, _stats.AngleOffsetDeg);
        
    }
}