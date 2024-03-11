using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        Color? colorStart = null,
        Color? colorEnd = null,
        bool closed = false,
        float radius = 0.1f,
        bool drawIntermediateSpheres = false,
        float intermediateRadiusRatio = 0.33f,
        float intermediateSpacing = 0.2f)
    {
        var it = path.GetEnumerator();
        var count = path.Count();
        var index = 0;

        if (!it.MoveNext())
            return;

        var current = it.Current;
        var previous = current;
        var first = current;

        void DrawSphere()
        {
            if (radius > 0)
            {
                if (colorStart.HasValue && colorEnd.HasValue)
                {
                    var color = Color.Lerp(colorStart.Value, colorEnd.Value, (float)index / count);
                    Gizmos.color = color;
                }
                Gizmos.DrawSphere(current, radius);
            }
        }

        void DrawSegment()
        {
            if (colorStart.HasValue && colorEnd.HasValue)
            {
                var color = Color.Lerp(colorStart.Value, colorEnd.Value, (index - 0.5f) / count);
                Gizmos.color = color;
            }
            Gizmos.DrawLine(previous, current);

            if (drawIntermediateSpheres)
            {
                var delta = current - previous;
                var length = delta.magnitude;
                var intermediateCount = Mathf.FloorToInt(length / intermediateSpacing);
                for (int i = 1; i < intermediateCount; i++)
                {
                    var t = (float)i / intermediateCount;
                    if (colorStart.HasValue && colorEnd.HasValue)
                    {
                        var color = Color.Lerp(colorStart.Value, colorEnd.Value, (index - 1f + t) / count);
                        Gizmos.color = color;
                    }
                    Gizmos.DrawSphere(previous + delta * t, radius * intermediateRadiusRatio);
                }
            }
        }

        DrawSphere();

        while (it.MoveNext())
        {
            current = it.Current;
            index++;
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

    static readonly System.Lazy<Mesh> cylinderMesh = new(() =>
    {
        var radius = 0.0125f;
        var mesh = MeshUtils.Cylinder(radiusTop: radius, radiusBottom: radius, height: 1, radialSegments: 16, capTop: false, capBottom: true);
        MeshUtils.Transform(mesh, Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0)));
        return mesh;
    });
    static readonly System.Lazy<Mesh> coneMesh = new(() =>
    {
        var mesh = MeshUtils.Cone(radius: 0.05f, height: 0.2f, segments: 16);
        MeshUtils.Transform(mesh, Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0)));
        return mesh;
    });
    public static void DrawArrow(Vector3 origin, Vector3 scaledDirection)
    {
        const float arrowHeadLength = 0.2f;
        var length = scaledDirection.magnitude;
        var direction = scaledDirection / length;

        var lengthMax = arrowHeadLength * 1.5f;
        if (length < lengthMax)
        {
            var m = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one * length / lengthMax) * m;
            DrawArrow(Vector3.zero, lengthMax * direction);
            Gizmos.matrix = m;
            return;
        }

        void Cylinder()
        {
            var position = origin + (length - arrowHeadLength) * 0.5f * direction;
            var rotation = Quaternion.LookRotation(direction);
            var scale = new Vector3(1, 1, length - arrowHeadLength);
            Gizmos.DrawMesh(cylinderMesh.Value, position, rotation, scale);
        }
        void Cone()
        {
            var position = origin + direction * (length - arrowHeadLength);
            var rotation = Quaternion.LookRotation(scaledDirection);
            Gizmos.DrawMesh(coneMesh.Value, position, rotation, Vector3.one);
        }
        var cameraForward = Camera.current.transform.forward;
        if (Vector3.Dot(cameraForward, direction) > 0)
        {
            Cone();
            Cylinder();
        }
        else
        {
            Cylinder();
            Cone();
        }
    }

    public static void DrawLabel(
        Vector3 position,
        string text,
        float fontScale = 1,
        float width = 200,
        float height = 200,
        float screenOffsetX = 0,
        float screenOffsetY = 0,
        TextAnchor anchor = TextAnchor.MiddleCenter)
    {
#if UNITY_EDITOR
        int fz = Mathf.RoundToInt(12f * fontScale);
        if (text.Length > 0 && fz > 0)
        {
            GUI.color = Gizmos.color;

            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.fixedWidth = width;
            centeredStyle.fixedHeight = height;
            centeredStyle.alignment = anchor;
            centeredStyle.fontSize = fz;

            Handles.matrix = Gizmos.matrix;
            var localPosition = Camera.current.transform.InverseTransformPoint(position);
            position += Camera.current.transform.up / localPosition.z * screenOffsetY;
            Handles.Label(position, text, centeredStyle);
        }
#endif
    }
}