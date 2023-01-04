using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugUtils
{

    public static void DrawArc(Vector3 origin, Vector3 direction, float halfAngleDeg, Color color, float duration)
    {
        float dirAngleRad = Mathf.Atan2(direction.y, direction.x);

        Vector3 left = origin + new Vector3(Mathf.Cos(dirAngleRad - halfAngleDeg * Mathf.Deg2Rad), Mathf.Sin(dirAngleRad - halfAngleDeg * Mathf.Deg2Rad), 0) * direction.magnitude;
        Vector3 right = origin + new Vector3(Mathf.Cos(dirAngleRad + halfAngleDeg * Mathf.Deg2Rad), Mathf.Sin(dirAngleRad + halfAngleDeg * Mathf.Deg2Rad), 0) * direction.magnitude;

        Debug.DrawLine(origin, left, color, duration);
        Debug.DrawLine(origin, right, color, duration);

        // Draw arc
        const float angleOffsetRad = Mathf.PI / 16f;

        float currAngleRad = dirAngleRad - halfAngleDeg * Mathf.Deg2Rad;
        float targetAngleRad = dirAngleRad + halfAngleDeg * Mathf.Deg2Rad;
        float nextAngleRad = currAngleRad + angleOffsetRad;

        while (nextAngleRad < targetAngleRad)
        {
            Debug.DrawLine(origin + new Vector3(Mathf.Cos(currAngleRad), Mathf.Sin(currAngleRad), 0) * direction.magnitude, origin + new Vector3(Mathf.Cos(nextAngleRad), Mathf.Sin(nextAngleRad), 0) * direction.magnitude, color, duration);

            currAngleRad = nextAngleRad;
            nextAngleRad += angleOffsetRad;
        }
        Debug.DrawLine(origin + new Vector3(Mathf.Cos(currAngleRad), Mathf.Sin(currAngleRad), 0) * direction.magnitude, origin + new Vector3(Mathf.Cos(targetAngleRad), Mathf.Sin(targetAngleRad), 0) * direction.magnitude, color, duration);
    }

    public static void DrawBox(Vector3 origin, Vector3 halfExtends, Quaternion orientation, Color color, float duration)
    {
        Vector3 p1 = origin + orientation * new Vector3(-halfExtends.x, halfExtends.y, halfExtends.z);
        Vector3 p2 = origin + orientation * new Vector3(halfExtends.x, halfExtends.y, halfExtends.z);
        Vector3 p3 = origin + orientation * new Vector3(halfExtends.x, -halfExtends.y, halfExtends.z);
        Vector3 p4 = origin + orientation * new Vector3(-halfExtends.x, -halfExtends.y, halfExtends.z);
        Vector3 p5 = origin + orientation * new Vector3(-halfExtends.x, halfExtends.y, -halfExtends.z);
        Vector3 p6 = origin + orientation * new Vector3(halfExtends.x, halfExtends.y, -halfExtends.z);
        Vector3 p7 = origin + orientation * new Vector3(halfExtends.x, -halfExtends.y, -halfExtends.z);
        Vector3 p8 = origin + orientation * new Vector3(-halfExtends.x, -halfExtends.y, -halfExtends.z);

        Debug.DrawLine(p1, p2, color, duration);
        Debug.DrawLine(p2, p3, color, duration);
        Debug.DrawLine(p3, p4, color, duration);
        Debug.DrawLine(p4, p1, color, duration);

        Debug.DrawLine(p5, p6, color, duration);
        Debug.DrawLine(p6, p7, color, duration);
        Debug.DrawLine(p7, p8, color, duration);
        Debug.DrawLine(p8, p5, color, duration);

        Debug.DrawLine(p1, p5, color, duration);
        Debug.DrawLine(p2, p6, color, duration);
        Debug.DrawLine(p3, p7, color, duration);
        Debug.DrawLine(p4, p8, color, duration);
    }
}
