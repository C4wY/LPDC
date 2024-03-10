using UnityEngine;

public class MathTests : MonoBehaviour
{
    void DrawLine(Vector3 origin, Vector3 direction)
    {
        Gizmos.color = Colors.FromHex("FFF", 0.25f);
        Gizmos.DrawLine(origin - direction * 10, origin + direction * 10);
        Gizmos.color = Colors.FromHex("FFF");
        Gizmos.DrawSphere(origin, 0.025f);
        Gizmos.DrawLine(origin, origin + direction);
    }

    void NearestPointOnLine()
    {
        var child0 = transform.GetChild(0);
        var child1 = transform.GetChild(1);

        if (child0 == null || child1 == null)
            return;

        child0.gameObject.name = "NearestPointOnLine: Line";
        child1.gameObject.name = "NearestPointOnLine: Point";

        var origin = child0.position;
        var direction = (Vector3)child0.localToWorldMatrix.GetColumn(2);

        var point = child1.position;

        Gizmos.color = Colors.FromHex("FFF");
        DrawLine(origin, direction);
        Gizmos.DrawSphere(point, 0.025f);

        {
            MathUtils.NearestPointOnLine(
                origin,
                direction,
                point,
                out var t);
            var p = origin + direction * t;
            Gizmos.color = Colors.FromHex("0FF");
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
            Gizmos.color = Colors.FromHex("0FF");
            Gizmos.DrawWireSphere(p, 0.05f);
        }
    }

    void NearestPointsBetweenLines()
    {
        var child0 = transform.GetChild(2);
        var child1 = transform.GetChild(3);

        if (child0 == null || child1 == null)
            return;

        child0.gameObject.name = "NearestPointsBetweenLines: Line 0";
        child1.gameObject.name = "NearestPointsBetweenLines: Line 1";

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
            Gizmos.color = Colors.FromHex("0FF");
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
            Gizmos.color = Colors.FromHex("0FF");
            Gizmos.DrawWireSphere(p0, 0.05f);
            Gizmos.DrawWireSphere(p1, 0.05f);
        }
    }

    void OnDrawGizmos()
    {
        NearestPointOnLine();
        NearestPointsBetweenLines();
    }
}