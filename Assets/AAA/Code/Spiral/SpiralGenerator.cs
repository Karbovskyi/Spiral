using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using Zenject;

public interface ISpiralGenerator
{
    void GenerateSpiral();
}

public class SpiralGenerator : IInitializable, ISpiralGenerator
{
    private readonly SpiralStats _spiralStats;
    private readonly Polyline _spiralPolyline;

    public SpiralGenerator(SpiralStats spiralStats, Polyline spiralPolyline)
    {
        _spiralStats = spiralStats;
        _spiralPolyline = spiralPolyline;
    }
    
    public void Initialize()
    {
        GenerateSpiral();
    }
    
    public void GenerateSpiral()
    {
        List<Vector2> dots = GenerateArchimedeanSpiral(
            _spiralStats.Turns,
            _spiralStats.RadiusStart,
            _spiralStats.RadiusStep,
            _spiralStats.DetailPerUnit,
            _spiralStats.Clockwise,
            _spiralStats.AngleOffsetDeg,
            _spiralStats.Center
        );
        
        _spiralPolyline.points.Clear();
        
        float step = dots.Count;
        for (var i = 0; i < dots.Count; i++)
        {
            float x = i / step;
            Color color = _spiralStats.FillGradient.Evaluate(x);
            _spiralPolyline.AddPoint(dots[i], color);
        } 
        
        _spiralPolyline.Thickness = _spiralStats.SpiralThickness;
        _spiralPolyline.meshOutOfDate = true;
    }
    
    private static List<Vector2> GenerateArchimedeanSpiral(
        float turns,
        float radiusStart,
        float radiusStep,
        float detailPerUnit,
        bool clockwise,
        float angleOffsetDeg,
        Vector2 center
    )
    {
        List<Vector2> result = new();

        float angleOffsetRad = angleOffsetDeg * Mathf.Deg2Rad;
        float thetaMax = turns * 2f * Mathf.PI;
        float b = radiusStep / (2f * Mathf.PI); // r = a + bθ

        float direction = clockwise ? -1f : 1f;

        float theta = 0f;
        float stepSize = 1f / detailPerUnit;
        float accumulatedDistance = 0f;

        Vector2 prev = Vector2.zero;
        bool firstPoint = true;

        while (theta <= thetaMax)
        {
            float r = radiusStart + b * theta;
            float angle = direction * (theta + angleOffsetRad);
            float x = r * Mathf.Cos(angle) + center.x;
            float y = r * Mathf.Sin(angle) + center.y;
            Vector2 current = new Vector2(x, y);

            if (firstPoint)
            {
                result.Add(current);
                firstPoint = false;
            }
            else
            {
                float segmentLength = Vector2.Distance(prev, current);
                accumulatedDistance += segmentLength;

                if (accumulatedDistance >= stepSize)
                {
                    result.Add(current);
                    accumulatedDistance = 0f;
                }
            }

            prev = current;
            theta += 0.01f;
        }

        return result;
    }
}

