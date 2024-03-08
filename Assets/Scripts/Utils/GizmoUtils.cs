using System.Collections.Generic;
using UnityEngine;

public static class GizmosUtils
{
    public static void DrawCircle(Vector3 position, Vector3 axis, float radius = 1, int subdivisions = 32, float aperture = 1)
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

    public static void DrawPath(
        IEnumerable<Vector3> path,
        bool closed = false,
        float radius = 0.1f,
        bool drawIntermediateSpheres = false,
        float intermediateRadiusRatio = 0.5f,
        float intermediateSpacing = 0.2f)
    {
        var it = path.GetEnumerator();

        if (!it.MoveNext())
            return;

        var current = it.Current;
        var previous = current;
        var first = current;

        void DrawSphere()
        {
            if (radius > 0)
                Gizmos.DrawSphere(current, radius);
        }

        void DrawSegment()
        {
            Gizmos.DrawLine(previous, current);
            if (drawIntermediateSpheres)
            {
                var delta = current - previous;
                var length = delta.magnitude;
                var count = Mathf.FloorToInt(length / intermediateSpacing);
                for (int i = 1; i < count; i++)
                    Gizmos.DrawSphere(previous + delta * (i / count), radius * intermediateRadiusRatio);
            }
        }

        DrawSphere();

        while (it.MoveNext())
        {
            current = it.Current;
            DrawSegment();
            DrawSphere();
            previous = current;
        }

        if (closed)
        {
            current = first;
            DrawSegment();
        }
    }
}