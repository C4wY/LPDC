using UnityEngine;

public static class GeometryExtensions
{
    public static Vector2 Abs(this Vector2 value) =>
        new(Mathf.Abs(value.x), Mathf.Abs(value.y));

    public static Vector3 Abs(this Vector3 value) =>
        new(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
}