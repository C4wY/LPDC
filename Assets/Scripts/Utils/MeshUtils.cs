using System.Linq;
using UnityEngine;
using static MatrixUtils;

public static partial class MeshUtils
{
    // X (+/-):
    public static Vector3 R = new(+1, 0, 0);
    public static Vector3 L = new(-1, 0, 0);

    // Y (+/-):
    public static Vector3 U = new(0, +1, 0);
    public static Vector3 D = new(0, -1, 0);

    // Z (+/-):
    public static Vector3 F = new(0, 0, +1);
    public static Vector3 B = new(0, 0, -1);

    // public struct Parameters
    // {
    //     public HideFlags hideFlags;

    //     public Parameters(
    //         HideFlags hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor)
    //     {
    //         this.hideFlags = hideFlags;
    //     }
    // }

    static Mesh ToMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uv, int[] triangles)
    {
        var mesh = new Mesh
        {
            vertices = vertices,
            normals = normals,
            uv = uv,
            triangles = triangles,
        };
        return mesh;
    }

    static Mesh ToMesh(Vector3[] vertices, Vector2[] uv, int[] triangles)
    {
        var mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            triangles = triangles,
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    static Mesh ToMesh(Vector3[] vertices, int[] triangles)
    {
        var mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    static Mesh Combine(CombineInstance[] combineInstances)
    {
        var mesh = new Mesh();
        mesh.CombineMeshes(combineInstances, mergeSubMeshes: true);
        return mesh;
    }

    public static void Transform(Mesh mesh, Matrix4x4 matrix)
    {
        var vertices = mesh.vertices;
        var normals = mesh.normals;
        var p = new Vector4();
        for (int i = 0, max = mesh.vertexCount; i < max; i++)
        {
            var v = vertices[i];
            p.Set(v.x, v.y, v.z, 1);
            p = matrix * p;
            vertices[i] = new(p.x, p.y, p.z);
            normals[i] = matrix * normals[i];
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.RecalculateBounds();
    }

    public static void Transform(Mesh mesh, Quaternion rotation)
    {
        var vertices = mesh.vertices;
        var normals = mesh.normals;
        for (int i = 0, max = mesh.vertexCount; i < max; i++)
        {
            vertices[i] = rotation * vertices[i];
            normals[i] = rotation * normals[i];
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.RecalculateBounds();
    }

    public static void Transform(Mesh mesh, Vector3 offset)
    {
        var vertices = mesh.vertices;
        for (int i = 0, max = mesh.vertexCount; i < max; i++)
        {
            vertices[i] += offset;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    public static void Transform(Mesh mesh, Vector3 pivotOffset, Vector3 scale)
    {
        var vertices = mesh.vertices;
        var (ox, oy, oz) = pivotOffset;
        var (sx, sy, sz) = scale;
        for (int i = 0, max = mesh.vertexCount; i < max; i++)
        {
            var (x, y, z) = vertices[i];
            vertices[i] = new((x + ox) * sx, (y + oy) * sy, (z + oz) * sz);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    static void Quad(
        ref Vector3[] vertices,
        ref Vector3[] normals,
        ref Vector2[] uvs,
        ref int[] triangles,
        Vector3 position,
        Vector3 tangent,
        Vector3 bitangent,
        int vertexOffset,
        int triangleOffset)
    {
        var t2 = tangent / 2;
        var bt2 = bitangent / 2;

        var voff = vertexOffset;
        var toff = triangleOffset;

        var normal = Vector3.Cross(tangent, bitangent);

        vertices[voff + 0] = position - t2 - bt2;
        vertices[voff + 1] = position + t2 - bt2;
        vertices[voff + 2] = position + t2 + bt2;
        vertices[voff + 3] = position - t2 + bt2;

        normals[voff + 0] = normal;
        normals[voff + 1] = normal;
        normals[voff + 2] = normal;
        normals[voff + 3] = normal;

        uvs[voff + 0] = new(0, 0);
        uvs[voff + 1] = new(1, 0);
        uvs[voff + 2] = new(1, 1);
        uvs[voff + 3] = new(0, 1);

        triangles[toff + 0] = voff + 0;
        triangles[toff + 1] = voff + 2;
        triangles[toff + 2] = voff + 1;

        triangles[toff + 3] = voff + 0;
        triangles[toff + 4] = voff + 3;
        triangles[toff + 5] = voff + 2;
    }

    public static Mesh Quad()
    {
        var vertices = new Vector3[4]
        {
                new(-.5f, -.5f, 0),
                new(+.5f, -.5f, 0),
                new(+.5f, +.5f, 0),
                new(-.5f, +.5f, 0),
        };

        var uv = new Vector2[4]
        {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1),
        };

        var triangles = new int[6]
        {
                0, 2, 1,
                0, 3, 2,
        };

        return ToMesh(vertices, uv, triangles);
    }

    public static Mesh SimpleCube()
    {
        var vertices = new Vector3[6 * 4];
        var normals = new Vector3[6 * 4];
        var uvs = new Vector2[6 * 4];
        var triangles = new int[6 * 2 * 3];

        Quad(ref vertices, ref normals, ref uvs, ref triangles, F / 2, L, U, 4 * 0, 6 * 0);
        Quad(ref vertices, ref normals, ref uvs, ref triangles, B / 2, R, U, 4 * 1, 6 * 1);
        Quad(ref vertices, ref normals, ref uvs, ref triangles, U / 2, R, F, 4 * 2, 6 * 2);
        Quad(ref vertices, ref normals, ref uvs, ref triangles, D / 2, L, F, 4 * 3, 6 * 3);
        Quad(ref vertices, ref normals, ref uvs, ref triangles, R / 2, F, U, 4 * 4, 6 * 4);
        Quad(ref vertices, ref normals, ref uvs, ref triangles, L / 2, B, U, 4 * 5, 6 * 5);

        return ToMesh(vertices, normals, uvs, triangles);
    }

    public static Mesh SimpleBox(Vector3 size, Vector3 pivot)
    {
        var mesh = SimpleCube();
        Transform(mesh, pivot - Vector3.one / 2, size);
        return mesh;
    }

    public static Mesh SimpleBox(Vector3 size) =>
        SimpleBox(size, Vector3.one / 2);

    public static Mesh Disc(
        float radius = 1f,
        int segments = 32)
    {
        int vertexCount = segments + 1;

        var vertices = new Vector3[vertexCount];
        var triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float deltaTheta = 2f * Mathf.PI / segments;
        float theta = 0f;

        for (int i = 1; i < vertexCount; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);

            vertices[i] = new Vector3(x, y, 0f);

            theta += deltaTheta;
        }

        for (int i = 0, ti = 0; i < segments; i++, ti += 3)
        {
            triangles[ti] = 0;
            triangles[ti + 1] = i + 1;
            triangles[ti + 2] = i + 2 <= segments ? i + 2 : 1;
        }

        return ToMesh(vertices, triangles);
    }

    public static Mesh Ring(
        float outerRadius = 1f,
        float innerRadius = 0.5f,
        int segments = 32)
    {
        int vertexCount = segments * 2;

        var vertices = new Vector3[vertexCount];
        var triangles = new int[segments * 6];

        float deltaTheta = 2f * Mathf.PI / segments;
        float theta = 0f;

        for (int i = 0, vi = 0, ti = 0; i < segments; i++, vi += 2, ti += 6)
        {
            float xIn = innerRadius * Mathf.Cos(theta);
            float yIn = innerRadius * Mathf.Sin(theta);
            float xOut = outerRadius * Mathf.Cos(theta);
            float yOut = outerRadius * Mathf.Sin(theta);

            vertices[vi] = new Vector3(xIn, yIn, 0f);
            vertices[vi + 1] = new Vector3(xOut, yOut, 0f);

            triangles[ti] = vi;
            triangles[ti + 1] = vi + 1;
            triangles[ti + 2] = (vi + 2) % vertexCount;

            triangles[ti + 3] = (vi + 2) % vertexCount;
            triangles[ti + 4] = vi + 1;
            triangles[ti + 5] = (vi + 3) % vertexCount;

            theta += deltaTheta;
        }

        return ToMesh(vertices, triangles);
    }

    [System.Serializable]
    public class ArcParameters
    {
        [Range(0, 10)]
        public float outerRadius = 1f;
        [Range(0, 10)]
        public float innerRadius = 0.5f;
        [Range(0, 1)]
        public float turnCompletion = 1;
        [Range(0, 1)]
        public float turnStart = 0;
        [Range(3, 128)]
        public int resolution = 32;

        public int StateHash()
        {
            return outerRadius.GetHashCode()
                ^ innerRadius.GetHashCode()
                ^ turnCompletion.GetHashCode()
                ^ turnStart.GetHashCode()
                ^ resolution.GetHashCode();
        }
    }

    public static Mesh Arc() =>
        Arc(new());

    public static Mesh Arc(ArcParameters parameters) =>
        Arc(
            parameters.outerRadius,
            parameters.innerRadius,
            parameters.turnCompletion,
            parameters.turnStart,
            parameters.resolution);

    public static Mesh Arc(
        float outerRadius,
        float innerRadius,
        float turnCompletion,
        float turnStart,
        int resolution)
    {
        if (turnCompletion == 0)
            return ToMesh(new Vector3[0], new int[0]);

        var segments = Mathf.Max(2, Mathf.CeilToInt(resolution * turnCompletion));

        var vertexCount = (segments + 1) * 2;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[segments * 6];
        var thetaStart = turnStart * MathUtils.PI2;
        var thetaMax = turnCompletion * MathUtils.PI2;

        float theta = thetaStart;
        float xIn = innerRadius * Mathf.Cos(theta);
        float yIn = innerRadius * Mathf.Sin(theta);
        float xOut = outerRadius * Mathf.Cos(theta);
        float yOut = outerRadius * Mathf.Sin(theta);

        vertices[0] = new Vector3(xIn, yIn, 0f);
        vertices[1] = new Vector3(xOut, yOut, 0f);

        for (int i = 1, vi = 0, ti = 0; i <= segments; i++, vi += 2, ti += 6)
        {
            theta = thetaStart + thetaMax * i / segments;
            xIn = innerRadius * Mathf.Cos(theta);
            yIn = innerRadius * Mathf.Sin(theta);
            xOut = outerRadius * Mathf.Cos(theta);
            yOut = outerRadius * Mathf.Sin(theta);

            vertices[vi + 2 + 0] = new Vector3(xIn, yIn, 0f);
            vertices[vi + 2 + 1] = new Vector3(xOut, yOut, 0f);

            triangles[ti + 0] = vi + 0;
            triangles[ti + 1] = vi + 2;
            triangles[ti + 2] = vi + 1;

            triangles[ti + 3] = vi + 2;
            triangles[ti + 4] = vi + 3;
            triangles[ti + 5] = vi + 1;
        }

        return ToMesh(vertices, triangles);
    }

    public static Mesh Cone(
        float radius = 1f,
        float height = 2f,
        int segments = 32,
        bool cap = true)
    {
        int vertexCount = segments + 1; // One extra vertex for the tip

        var vertices = new Vector3[vertexCount];
        var triangles = new int[segments * 3];

        float deltaTheta = 2f * Mathf.PI / segments;
        float theta = 0f;

        // Vertex at the tip
        vertices[0] = new Vector3(0f, height, 0f);

        // Vertices for the sides
        for (int i = 1; i <= segments; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            vertices[i] = new Vector3(x, 0f, z);

            theta += deltaTheta;
        }

        // Triangles for the sides
        for (int i = 0, ti = 0; i < segments; i++, ti += 3)
        {
            triangles[ti] = 0;
            triangles[ti + 1] = (i + 1) % segments + 1;
            triangles[ti + 2] = i + 1;
        }

        var mesh = ToMesh(vertices, triangles);

        if (cap)
        {
            mesh = Combine(new CombineInstance[]{
                new()
                {
                    mesh = mesh,
                    transform = Matrix4x4.identity,
                },
                new()
                {
                    mesh = Disc(radius, segments),
                    transform = Matrix4x4.TRS(new(0, 0, 0), Quaternion.Euler(90, 0, 0), Vector3.one),
                },
            });
        }
        return mesh;
    }

    public static Mesh Cylinder(
        float radiusTop = 1f,
        float radiusBottom = 1f,
        float height = 2f,
        int radialSegments = 32,
        int heightSegments = 1,
        bool capTop = false,
        bool capBottom = false)
    {
        int verticesCount = (radialSegments + 1) * (heightSegments + 1);
        var vertices = new Vector3[verticesCount];
        var triangles = new int[radialSegments * heightSegments * 6];

        int vertexIndex = 0;
        float heightStep = height / heightSegments;

        for (int i = 0; i <= heightSegments; i++)
        {
            float y = i * heightStep - height / 2;
            float radius = radiusBottom + (radiusTop - radiusBottom) * i / heightSegments;

            for (int j = 0; j <= radialSegments; j++)
            {
                float theta = 2.0f * Mathf.PI * j / radialSegments;
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);

                vertices[vertexIndex] = new Vector3(x, y, z);
                vertexIndex++;
            }
        }

        int triangleIndex = 0;
        for (int i = 0; i < heightSegments; i++)
        {
            for (int j = 0; j < radialSegments; j++)
            {
                int current = i * (radialSegments + 1) + j;
                int next = current + radialSegments + 1;

                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = next;
                triangles[triangleIndex++] = current + 1;

                triangles[triangleIndex++] = next;
                triangles[triangleIndex++] = next + 1;
                triangles[triangleIndex++] = current + 1;
            }
        }

        var mesh = ToMesh(vertices, triangles);

        if (capTop)
        {
            mesh = Combine(new CombineInstance[]{
                    new()
                    {
                        mesh = mesh,
                        transform = Matrix4x4.identity,
                    },
                    new()
                    {
                        mesh = Disc(radiusTop, radialSegments),
                        transform = Matrix4x4.TRS(new(0, height / 2f, 0), Quaternion.Euler(-90, 0, 0), Vector3.one),
                    },
                });
        }

        if (capBottom)
        {
            mesh = Combine(new CombineInstance[]{
                    new()
                    {
                        mesh = mesh,
                        transform = Matrix4x4.identity,
                    },
                    new()
                    {
                        mesh = Disc(radiusBottom, radialSegments),
                        transform = Matrix4x4.TRS(new(0, -height / 2f, 0), Quaternion.Euler(90, 0, 0), Vector3.one),
                    },
                });
        }

        return mesh;
    }

    public static Mesh Arrow(
        float length = 1f,
        float radius = .07f,
        float bodyRadiusRatio = .5f,
        float arrowLengthRatio = .2f,
        int radialSegments = 12)
    {
        float bodyRadius = radius * bodyRadiusRatio;
        float bodyLength = length * (1f - arrowLengthRatio);
        float arrowLength = length * arrowLengthRatio;
        return Combine(new CombineInstance[]{
                new()
                {
                    mesh = Cylinder(bodyRadius, bodyRadius, bodyLength, radialSegments, 1, false, true),
                    transform = TRS(rz: -90f, x: (length - arrowLength) / 2f),
                },
                new()
                {
                    mesh = Cone(radius, arrowLength, radialSegments),
                    transform = TRS(rz: -90f, x: length - arrowLength),
                },
                new()
                {
                    mesh = Ring(radius, bodyRadius, radialSegments),
                    transform = TRS(ry: -90f, rz: -90f, x: length - arrowLength),
                }
            });
    }

    public static Mesh Frame(
        float x,
        float y,
        float width,
        float height,
        float borderSize,
        float borderOutside = 1,
        bool doubleSided = true)
    {
        int i;

        var wi = width / 2f + borderSize * (borderOutside - 1f);
        var wo = width / 2f + borderSize * borderOutside;
        var hi = height / 2f + borderSize * (borderOutside - 1f);
        var ho = height / 2f + borderSize * borderOutside;

        var vertices = new Vector3[doubleSided ? 16 : 8];
        i = 0;
        vertices[i++] = new(x - wo, y - ho, 0);
        vertices[i++] = new(x + wo, y - ho, 0);
        vertices[i++] = new(x + wo, y + ho, 0);
        vertices[i++] = new(x - wo, y + ho, 0);
        vertices[i++] = new(x - wi, y - hi, 0);
        vertices[i++] = new(x + wi, y - hi, 0);
        vertices[i++] = new(x + wi, y + hi, 0);
        vertices[i++] = new(x - wi, y + hi, 0);

        var normals = Enumerable
            .Repeat(Vector3.back, 8)
            .ToArray();

        if (doubleSided)
            normals = normals
                .Concat(normals.Select(v => -v)).
                ToArray();

        // 3 ──────────────── 2
        // │                  │
        // │    7 ────── 6    │
        // │    │        │    │
        // │    │        │    │
        // │    4 ────── 5    │
        // │                  │
        // 0 ──────────────── 1

        var triangles = new int[(doubleSided ? 16 : 8) * 3];
        i = 0;
        // 0451
        (triangles[i++], triangles[i++], triangles[i++]) = (0, 4, 1);
        (triangles[i++], triangles[i++], triangles[i++]) = (4, 5, 1);
        // 1562
        (triangles[i++], triangles[i++], triangles[i++]) = (1, 5, 2);
        (triangles[i++], triangles[i++], triangles[i++]) = (5, 6, 2);
        // 2673
        (triangles[i++], triangles[i++], triangles[i++]) = (2, 6, 3);
        (triangles[i++], triangles[i++], triangles[i++]) = (6, 7, 3);
        // 3740
        (triangles[i++], triangles[i++], triangles[i++]) = (3, 7, 0);
        (triangles[i++], triangles[i++], triangles[i++]) = (7, 4, 0);

        if (doubleSided)
        {
            // 0451
            (triangles[i++], triangles[i++], triangles[i++]) = (0, 1, 4);
            (triangles[i++], triangles[i++], triangles[i++]) = (4, 1, 5);
            // 1562
            (triangles[i++], triangles[i++], triangles[i++]) = (1, 2, 5);
            (triangles[i++], triangles[i++], triangles[i++]) = (5, 2, 6);
            // 2673
            (triangles[i++], triangles[i++], triangles[i++]) = (2, 3, 6);
            (triangles[i++], triangles[i++], triangles[i++]) = (6, 3, 7);
            // 3740
            (triangles[i++], triangles[i++], triangles[i++]) = (3, 0, 7);
            (triangles[i++], triangles[i++], triangles[i++]) = (7, 0, 4);
        }

        return new Mesh()
        {
            vertices = vertices,
            normals = normals,
            triangles = triangles,
        };
    }

    public static Mesh CubeFrame(float borderSize = 1f / 8)
    {
        var frame = Frame(
            x: 0,
            y: 0,
            width: 1,
            height: 1,
            borderSize,
            borderOutside: 0,
            doubleSided: true);

        return Combine(new CombineInstance[]{
            new()
            {
                mesh = frame,
                transform = TRS(x: .5f, ry: 90),
            },
            new()
            {
                mesh = frame,
                transform = TRS(x: -.5f, ry: -90),
            },
            new()
            {
                mesh = frame,
                transform = TRS(y: .5f, rx: 90),
            },
            new()
            {
                mesh = frame,
                transform = TRS(y: -.5f, rx: -90),
            },
            new()
            {
                mesh = frame,
                transform = TRS(z: -.5f),
            },
            new()
            {
                mesh = frame,
                transform = TRS(z: .5f, ry: 180),
            },
        });
    }
}