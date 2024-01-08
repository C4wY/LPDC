using UnityEngine;

public static class GizmosUtils
{
    public static void DrawCircle(Vector3 position, Vector3 axis, float radius = 1, int subdivisions = 24, float aperture = 1)
    {
        var U = Vector3.Cross(axis, new(axis.y, axis.z, axis.x)).normalized;
        var V = Vector3.Cross(axis, U);
        int count = Mathf.CeilToInt(subdivisions * aperture);
        Vector3 previous = position + radius * U;
        for (int i = 1; i <= count; i++)
        {
            float a = (float)i / count * 2 * Mathf.PI;
            float x = radius * Mathf.Cos(a);
            float y = radius * Mathf.Sin(a);
            Vector3 current = position + U * x + V * y;
            Gizmos.DrawLine(previous, current);
            previous = current;
        }
    }
}