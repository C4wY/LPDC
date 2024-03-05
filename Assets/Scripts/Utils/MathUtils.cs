using UnityEngine;

public static class MathUtils
{
    public static float GetJumpVelocityY(float jumpHeight)
    {
        return Mathf.Sqrt(Physics.gravity.magnitude * 2f * jumpHeight);
    }

    public enum CompareResult
    {
        Less,
        Equal,
        Greater,
    }
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
}