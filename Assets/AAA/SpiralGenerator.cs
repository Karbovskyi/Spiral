using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SpiralGenerator : MonoBehaviour
{
    public int Count;
    
    [Header("Spiral Settings")]
    public float turns = 5f;
    public float radiusStart = 1f;
    public float radiusStep = 1f;
    public float spiralThickness = 0.1f;
    public float detailPerUnit = 20f;
    public bool clockwise = true;
    public float angleOffsetDeg = 0f;
    public Vector2 center = Vector2.zero;
    
    [SerializeField] private Polyline _spiralPolyline;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateSpiral();
        }
    }


    public void GenerateSpiral()
    {
        List<Vector2> dots = GenerateArchimedeanSpiral(
            turns,
            radiusStart,
            radiusStep,
            detailPerUnit,
            clockwise,
            angleOffsetDeg,
            center
        );
        
        Count = dots.Count;
        
        _spiralPolyline.points.Clear();
        
                
        for (var i = 0; i < dots.Count; i++) 
            _spiralPolyline.AddPoint(dots[i]);

        _spiralPolyline.Thickness = spiralThickness;
        
        _spiralPolyline.meshOutOfDate = true;
        
    }
    
    public static List<Vector2> GenerateArchimedeanSpiral(
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

