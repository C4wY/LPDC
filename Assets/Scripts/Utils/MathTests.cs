using UnityEngine;

public class MathTests : MonoBehaviour
{
    Transform RequireChild(int index, string name)
    {
        if (transform.childCount <= index)
        {
            var newChild = new GameObject().transform;
            newChild.SetParent(transform);
        }

        var child = transform.GetChild(index);
        child.gameObject.name = name;
        return child;
    }

    void DrawLine(Vector3 origin, Vector3 direction)
    {
        Gizmos.color = Colors.Hex("FFF", 0.25f);
        Gizmos.DrawLine(origin - direction * 10, origin + direction * 10);
        Gizmos.color = Colors.Hex("FFF");
        Gizmos.DrawSphere(origin, 0.025f);
        Gizmos.DrawLine(origin, origin + direction);
    }

    void NearestPointOnLine()
    {
        var child0 = RequireChild(0, "NearestPointOnLine: Line");
        var child1 = RequireChild(1, "NearestPointOnLine: Point");

        var origin = child0.position;
        var direction = (Vector3)child0.localToWorldMatrix.GetColumn(2);

        var point = child1.position;

        Gizmos.color = Colors.Hex("FFF");
        DrawLine(origin, direction);
        Gizmos.DrawSphere(point, 0.025f);

        {
            MathUtils.NearestPointOnLine(
                origin,
                direction,
                point,
                out var t);
            var p = origin + direction * t;
            Gizmos.color = Colors.Hex("0FF");
            Gizmos.DrawSphere(p, 0.025f);
            Gizmos.DrawLine(point, p);
        }

        {
            // Direction vector DOES NOT have to be normalized!
            // But let's normalize it for the sake of the example.
            var d = direction.normalized;
            MathUtils.NearestPointOnLine(
                origin,
                d,
                point,
                out var t);
            var p = origin + d * t;
            Gizmos.color = Colors.Hex("0FF");
            Gizmos.DrawWireSphere(p, 0.05f);
        }
    }

    void NearestPointsBetweenLines()
    {
        var child0 = RequireChild(2, "NearestPointsBetweenLines: Line 0");
        var child1 = RequireChild(3, "NearestPointsBetweenLines: Line 1");

        var origin0 = child0.position;
        var direction0 = (Vector3)child0.localToWorldMatrix.GetColumn(2);

        var origin1 = child1.position;
        var direction1 = (Vector3)child1.localToWorldMatrix.GetColumn(2);

        DrawLine(origin0, direction0);
        DrawLine(origin1, direction1);

        {
            MathUtils.NearestPointsBetweenLines(
                origin0,
                direction0,
                origin1,
                direction1,
                out var t0,
                out var t1);
            var p0 = origin0 + direction0 * t0;
            var p1 = origin1 + direction1 * t1;
            Gizmos.color = Colors.Hex("0FF");
            Gizmos.DrawSphere(p0, 0.025f);
            Gizmos.DrawSphere(p1, 0.025f);
            Gizmos.DrawLine(p0, p1);
        }

        {
            // Direction vectors DO NOT have to be normalized!
            // But let's normalize them for the sake of the example.
            var d0 = direction0.normalized;
            var d1 = direction1.normalized;
            MathUtils.NearestPointsBetweenLines(
                origin0,
                d0,
                origin1,
                d1,
                out var t0,
                out var t1);
            var p0 = origin0 + d0 * t0;
            var p1 = origin1 + d1 * t1;
            Gizmos.color = Colors.Hex("0FF");
            Gizmos.DrawWireSphere(p0, 0.05f);
            Gizmos.DrawWireSphere(p1, 0.05f);
        }
    }

    void Arrow()
    {
        var child = RequireChild(4, "Arrow");
        var origin = child.position;
        var direction = (Vector3)child.localToWorldMatrix.GetColumn(2);
        GizmosUtils.DrawArrow(origin, direction);
    }

    void OnDrawGizmos()
    {
        NearestPointOnLine();
        NearestPointsBetweenLines();
        Arrow();
    }
}