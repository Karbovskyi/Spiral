using UnityEngine;

public class ArrowDrawer : MonoBehaviour
{
    public Material lineMaterial;

    public void DrawArrow(Vector2 start, Vector2 direction, float length, Color color)
    {
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.position = new Vector3(start.x, start.y, 0);

        LineRenderer lr = arrowObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.positionCount = 4; // основна лінія + 2 для "пір'я" + 1 для повернення
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.useWorldSpace = true;
        //lr.startColor = color;
        //lr.endColor = color;
        lr.material.color = color;

        Vector3 startPos = new Vector3(start.x, start.y, 0);
        Vector3 endPos = startPos + new Vector3(direction.x, direction.y, 0).normalized * length;

        // Основна лінія
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);

        // Стрілочний наконечник (пір'я)
        Vector3 rightWing = Quaternion.Euler(0, 0, 135) * (endPos - startPos).normalized * (length * 0.2f);
        Vector3 leftWing = Quaternion.Euler(0, 0, -135) * (endPos - startPos).normalized * (length * 0.2f);

        lr.SetPosition(2, endPos + rightWing);
        lr.SetPosition(3, endPos + leftWing);

        Destroy(arrowObj, 0.5f); // автоматичне знищення після 0.5 секунди
    }
}