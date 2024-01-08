using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeconstructExtensions
{
    public static void Deconstruct(this Color value, out float r, out float g, out float b, out float a)
    {
        r = value.r;
        g = value.g;
        b = value.b;
        a = value.a;
    }

    public static void Deconstruct(this Color value, out float r, out float g, out float b)
    {
        r = value.r;
        g = value.g;
        b = value.b;
    }

    public static void Deconstruct(this Color32 value, out byte r, out byte g, out byte b, out byte a)
    {
        r = value.r;
        g = value.g;
        b = value.b;
        a = value.a;
    }

    public static void Deconstruct(this Color32 value, out byte r, out byte g, out byte b)
    {
        r = value.r;
        g = value.g;
        b = value.b;
    }

    public static void Deconstruct(this Vector2 value, out float x, out float y)
    {
        x = value.x;
        y = value.y;
    }

    public static void Deconstruct(this Vector2Int value, out int x, out int y)
    {
        x = value.x;
        y = value.y;
    }

    public static void Deconstruct(this Vector3 value, out float x, out float y, out float z)
    {
        x = value.x;
        y = value.y;
        z = value.z;
    }

    public static void Deconstruct(this Vector3Int value, out int x, out int y, out int z)
    {
        x = value.x;
        y = value.y;
        z = value.z;
    }

    public static void Deconstruct(this Vector4 value, out float x, out float y, out float z, out float w)
    {
        x = value.x;
        y = value.y;
        z = value.z;
        w = value.w;
    }

    public static void Deconstruct(this Quaternion value, out float x, out float y, out float z, out float w)
    {
        x = value.x;
        y = value.y;
        z = value.z;
        w = value.w;
    }

    public static void Deconstruct(this Bounds value, out Vector3 center, out Vector3 size)
    {
        center = value.center;
        size = value.size;
    }

    public static void Deconstruct(this BoundsInt value, out Vector3Int position, out Vector3Int size)
    {
        position = value.position;
        size = value.size;
    }

    public static void Deconstruct(this BoundsInt value,
        out int px, out int py, out int pz,
        out int sx, out int sy, out int sz)
    {
        (px, py, pz) = value.position;
        (sx, sy, sz) = value.size;
    }

    public static void Deconstruct(this Ray value, out Vector3 origin, out Vector3 direction)
    {
        origin = value.origin;
        direction = value.direction;
    }
}
