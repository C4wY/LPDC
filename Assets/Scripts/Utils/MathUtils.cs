using UnityEngine;

public static class MathUtils
{
    public static float GetJumpVelocityY(float jumpHeight)
    {
        return Mathf.Sqrt(Physics.gravity.magnitude * 2f * jumpHeight);
    }

    public enum CompareResult { Less, Equal, Greater }
    public static CompareResult Compare(float a, float b, float epsilon = 0.0001f)
    {
        var diff = a - b;
        if (Mathf.Abs(diff) < epsilon)
        {
            return CompareResult.Equal;
        }
        else if (diff < 0)
        {
            return CompareResult.Less;
        }
        else
        {
            return CompareResult.Greater;
        }
    }

    /// <summary>
    /// Direction vector DOES NOT have to be normalized!
    /// </summary>
    public static void NearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point, out float t)
    {
        t = Vector3.Dot(point - origin, direction) / direction.sqrMagnitude;
    }

    /// <summary>
    /// Direction vectors DO NOT have to be normalized!
    /// https://math.stackexchange.com/questions/846054/closest-points-on-two-line-segments
    /// </summary>
    public static void NearestPointsBetweenLines(
        Vector3 origin1,
        Vector3 direction1,
        Vector3 origin2,
        Vector3 direction2,
        out float t1,
        out float t2)
    {
        var V21 = origin2 - origin1;

        var v22 = Vector3.Dot(direction2, direction2);
        var v11 = Vector3.Dot(direction1, direction1);
        var v21 = Vector3.Dot(direction2, direction1);
        var v21_1 = Vector3.Dot(V21, direction1);
        var v21_2 = Vector3.Dot(V21, direction2);
        var denom = v21 * v21 - v22 * v11;

        if (Mathf.Approximately(denom, 0))
        {
            t1 = 0;
            t2 = (v11 * t1 - v21_1) / v21;
        }
        else
        {
            t1 = (v21_2 * v21 - v22 * v21_1) / denom;
            t2 = (-v21_1 * v21 + v11 * v21_2) / denom;
        }
    }

}