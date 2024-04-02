using UnityEngine;

public static class MatrixUtils
{
    public static Matrix4x4 TRS(
        float x = 0,
        float y = 0,
        float z = 0,
        float rx = 0,
        float ry = 0,
        float rz = 0,
        float scale = 1,
        float scaleX = 1,
        float scaleY = 1,
        float scaleZ = 1)
    {
        return Matrix4x4.TRS(new(x, y, z), Quaternion.Euler(rx, ry, rz), new(scale * scaleX, scale * scaleY, scale * scaleZ));
    }

    /// <summary>
    /// Transform a point by a 4x4 matrix interpreted as a 3x3 matrix (the translation part is ignored).
    /// </summary>
    public static Vector3 Transform3x3(Matrix4x4 matrix, Vector3 point)
    {
        var (x, y, z) = point;
        var m00 = matrix[4 * 0 + 0];
        var m01 = matrix[4 * 1 + 0];
        var m02 = matrix[4 * 2 + 0];
        var m10 = matrix[4 * 0 + 1];
        var m11 = matrix[4 * 1 + 1];
        var m12 = matrix[4 * 2 + 1];
        var m20 = matrix[4 * 0 + 2];
        var m21 = matrix[4 * 1 + 2];
        var m22 = matrix[4 * 2 + 2];
        return new(
            m00 * x + m01 * y + m02 * z,
            m10 * x + m11 * y + m12 * z,
            m20 * x + m21 * y + m22 * z);
    }

    /// <summary>
    /// Transform a point by a regular matrix(made of position, scale & rotation).
    /// Equivalent to (Vector3)(matrix * new Vector4(point.x, point.y, point.z, 1)) 
    /// with maybe an small optimization (since "w" is ignored).
    /// </summary>
    public static Vector3 Transform4x3(Matrix4x4 matrix, Vector3 point)
    {
        var (x, y, z) = point;
        var m00 = matrix[4 * 0 + 0];
        var m01 = matrix[4 * 1 + 0];
        var m02 = matrix[4 * 2 + 0];
        var t_x = matrix[4 * 3 + 0];
        var m10 = matrix[4 * 0 + 1];
        var m11 = matrix[4 * 1 + 1];
        var m12 = matrix[4 * 2 + 1];
        var t_y = matrix[4 * 3 + 1];
        var m20 = matrix[4 * 0 + 2];
        var m21 = matrix[4 * 1 + 2];
        var m22 = matrix[4 * 2 + 2];
        var t_z = matrix[4 * 3 + 2];
        return new(
            m00 * x + m01 * y + m02 * z + t_x,
            m10 * x + m11 * y + m12 * z + t_y,
            m20 * x + m21 * y + m22 * z + t_z);
    }

    /// <summary>
    /// Matrix4x4.GetHashCode() is not safe, it can return same values for different matrices.
    /// </summary>
    public static int StateHash(Matrix4x4 matrix)
    {
        var hash = 0;
        for (int i = 0; i < 16; i++)
            hash = MathUtils.ShiftLeft(hash) ^ matrix[i].GetHashCode();
        return hash;
    }
}
