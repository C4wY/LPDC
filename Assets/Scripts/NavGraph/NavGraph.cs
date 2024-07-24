using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class NavGraph
{
    readonly Dictionary<int, Node> nodesDict = new();
    readonly Dictionary<(int n0, int n1), Segment> segmentsDict = new();
    public Node[] Nodes { get; private set; }

    static readonly RaycastHit[] raycastHits = new RaycastHit[10];
    static readonly Collider[] overlaps = new Collider[10];

    static float DistanceXY(Vector3 a, Vector3 b) =>
        Vector2.Distance(a, b);

    static class AirConfig
    {
        public const float horizontalDistanceMax = 5;
        public const float verticalDistanceMax = 2;
        public const float radius = 1.05f;
        public const float verticalOffset = 1.2f;

        public static float RayMaxDistance =>
            Mathf.Sqrt(horizontalDistanceMax * horizontalDistanceMax + verticalDistanceMax * verticalDistanceMax);
    }

    public void Clear()
    {
        nodesDict.Clear();
        segmentsDict.Clear();
        Nodes = Array.Empty<Node>();
    }

    public Node GetNode(int id) =>
        nodesDict[id];

    public (Node, Node) GetNodes(int id0, int id1) =>
        (nodesDict[id0], nodesDict[id1]);

    public (Node, Node) GetNodes(Segment segment) =>
        (nodesDict[segment.n0], nodesDict[segment.n1]);

    public Segment GetSegment(int n0, int n1)
    {
        if (n0 > n1)
            (n0, n1) = (n1, n0);

        return segmentsDict[(n0, n1)];
    }

    public void SampleWorld(int xMin, int xMax, int zMin, int zMax, float originY = 100, float distance = 200)
    {
        var oneSidedLayer = LayerMask.NameToLayer("OneSidedPlatform");
        var mask = LayerMask.GetMask("LevelBlock", "OneSidedPlatform");

        var nodeTable = new Dictionary<Vector2, Node>();

        for (var iz = zMin; iz < zMax; iz++)
            for (var ix = xMin; ix < xMax; ix++)
            {
                var x = ix + 0.5f;
                var z = iz + 0.5f;
                var origin = new Vector3(x, originY, z);
                var ray = new Ray(origin, Vector3.down);
                var raycastHitCount = Physics.RaycastNonAlloc(ray, raycastHits, distance, mask);
                for (var i = 0; i < raycastHitCount; i++)
                {
                    var hit = raycastHits[i];
                    var overlapCount = hit.collider.gameObject.layer == oneSidedLayer
                        ? 0 // Ignoring one-sided platforms overlaps.
                        : Physics.OverlapCapsuleNonAlloc(hit.point + Vector3.up * 0.5f, hit.point + Vector3.up * 1.5f, 0.45f, overlaps, mask);
                    if (overlapCount == 0)
                    {
                        var node = new Node { position = new(x, hit.point.y, z) };
                        var xy = node.RoundedPositionXY;

                        if (nodeTable.TryGetValue(xy, out var existingNode))
                        {
                            if (node.position.z < existingNode.position.z)
                                nodeTable[xy] = node;
                        }
                        else
                        {
                            nodeTable.Add(xy, node);
                        }
                    }
                }
            }

        foreach (var node in nodeTable.Values)
            nodesDict.Add(node.id, node);

        Nodes = nodesDict.Values.ToArray();
    }

    public void RemoveDangerNodes()
    {
        var dangerNodes = nodesDict.Values
            .Where(node =>
            {
                var count = Physics.OverlapSphereNonAlloc(node.position, 0.25f, overlaps, LayerMask.GetMask("Danger"));
                return count > 0;
            })
            .ToArray();

        foreach (var node in dangerNodes)
            nodesDict.Remove(node.id);

        Nodes = nodesDict.Values.ToArray();
    }

    public bool TryGetSegment(int n0, int n1, out Segment link)
    {
        if (n0 > n1)
            (n0, n1) = (n1, n0);

        return segmentsDict.TryGetValue((n0, n1), out link);
    }

    Segment CreateSegment(Node node0, Node node1, float distance, Vector3 direction, Segment.Type type = Segment.Type.Ground)
    {
        var n0 = node0.id;
        var n1 = node1.id;

        if (n0 > n1)
            (n0, n1) = (n1, n0);

        var segment = new Segment(this, n0, n1, distance, direction, type);

        node0.segments.Add(segment);
        node1.segments.Add(segment);

        segmentsDict.Add((n0, n1), segment);

        return segment;
    }

    public void CreateGroundSegments(float maxDistance = 1.5f)
    {
        var nodeArray = nodesDict.Values.ToArray();
        var count = nodeArray.Length;
        for (var i = 0; i < count; i++)
        {
            var node0 = nodeArray[i];
            for (var j = i + 1; j < count; j++)
            {
                var node1 = nodeArray[j];
                var delta = node1.position - node0.position;
                var (dx, dy, dz) = delta;
                var distance = Mathf.Sqrt(dx * dx + dy * dy); // Ignoring Z axis for distance calculation.

                if (Mathf.Abs(dz) > 0.5f)
                {
                    var node0BeforeCount = Physics.RaycastNonAlloc(new Ray(node0.position + Vector3.up * 0.5f, Vector3.back), raycastHits, 10.0f, LayerMask.GetMask("LevelBlock"));
                    var node1BeforeCount = Physics.RaycastNonAlloc(new Ray(node1.position + Vector3.up * 0.5f, Vector3.back), raycastHits, 10.0f, LayerMask.GetMask("LevelBlock"));

                    if (node0BeforeCount > 0 || node1BeforeCount > 0)
                        continue;
                }

                if (distance < maxDistance)
                    CreateSegment(node0, node1, distance, delta / distance);
            }
        }
    }

    /// <summary>
    /// Radius and vertical offset are used to check if there is an obstacle 
    /// between the nodes based on a sphere cast.    
    /// </summary>
    /// <param name="distanceMax"></param>
    /// <param name="radius"></param>
    /// <param name="verticalOffset"></param>
    public void CreateAirSegments()
    {
        var nodeArray = nodesDict.Values.ToArray();
        var nodeCount = nodeArray.Length;
        var layer = LayerMask.GetMask("LevelBlock");

        for (int i = 0; i < nodeCount; i++)
        {
            var node0 = nodeArray[i];
            for (int j = i + 1; j < nodeCount; j++)
            {
                var node1 = nodeArray[j];
                var delta = node1.RoundedPositionXY - node0.RoundedPositionXY;
                var (dx, dy) = delta.Abs();

                if (dx > AirConfig.horizontalDistanceMax + 0.1f || dy > AirConfig.verticalDistanceMax + 0.1f)
                    continue;

                // Ignore if there is an obstacle between the nodes.
                var p0 = node0.position + Vector3.up * AirConfig.verticalOffset;
                var p1 = node1.position + Vector3.up * AirConfig.verticalOffset;
                var v01 = p1 - p0;
                var v01Magnitude = v01.magnitude;
                var hitCount = Physics.RaycastNonAlloc(
                    ray: new Ray(p0, v01 / v01Magnitude),
                    raycastHits, v01Magnitude, layer);

                if (hitCount > 0)
                    continue;

                // Create a jump segment only if there is no direct path between the nodes.
                if (TryFindPath(node0, node1, out var _, Segment.Type.Ground) == false)
                {
                    var distance = DistanceXY(p0, p1);
                    CreateSegment(node0, node1, distance, delta / distance, Segment.Type.Air);
                }
            }
        }
    }

    public Memory<RaycastHit> AirRaycast(int node0, int node1) =>
        AirRaycast(Nodes[node0], Nodes[node1]);

    public Memory<RaycastHit> AirRaycast(Node node0, Node node1)
    {
        var layer = LayerMask.GetMask("LevelBlock");

        var p0 = node0.position + Vector3.up * AirConfig.verticalOffset;
        var p1 = node1.position + Vector3.up * AirConfig.verticalOffset;
        var v01 = p1 - p0;
        var v01Magnitude = v01.magnitude;
        var count = Physics.RaycastNonAlloc(
            ray: new Ray(p0, v01 / v01Magnitude),
            raycastHits, v01Magnitude, layer);

        return new Memory<RaycastHit>(raycastHits, 0, count);
    }

    public void DrawAirRaycast(int node0, int node1) =>
        DrawAirRaycast(Nodes[node0], Nodes[node1]);

    public void DrawAirRaycast(Node node0, Node node1)
    {
        var p0 = node0.position + Vector3.up * AirConfig.verticalOffset;
        var p1 = node1.position + Vector3.up * AirConfig.verticalOffset;
        Gizmos.DrawLine(p0, p1);
        foreach (var hit in AirRaycast(node0, node1).Span)
            Gizmos.DrawSphere(hit.point, 0.1f);
    }

    public Node FindNearestNode(Vector3 position)
    {
        if (nodesDict.Count == 0)
            return null;

        return nodesDict.Values
            .OrderBy(n => (n.position - position).sqrMagnitude)
            .First();
    }

    public (Segment segment, float t) FindNearestSegment(Vector3 position)
    {
        if (segmentsDict.Count == 0)
            return (null, float.NaN);

        return segmentsDict.Values
            .Select(segment =>
            {
                var n0 = nodesDict[segment.n0];
                var n1 = nodesDict[segment.n1];
                var d = n1.position - n0.position;
                MathUtils.NearestPointOnLine(n0.position, d, position, out var t);
                return (segment, t);
            })
            .OrderBy(item =>
            {
                var (segment, t) = item;
                var n0 = nodesDict[segment.n0];
                var n1 = nodesDict[segment.n1];
                var d = n1.position - n0.position;
                var p = n0.position + d * Mathf.Clamp01(t);
                return (p - position).sqrMagnitude;
            })
            .First();
    }

    public struct Path
    {
        public Node[] nodes;
        public float[] distances;

        public Path(Node[] nodes, float[] distances)
        {
            this.nodes = nodes;
            this.distances = distances;
        }
    }

    /// <summary>
    /// A* pathfinding algorithm (where Z axis is ignored).
    /// </summary>
    public bool TryFindPath(Node from, Node to, out Path path, Segment.Type segmentTypeMask = (Segment.Type)~0)
    {
        var openSet = new HashSet<int> { from.id };
        var cameFrom = new Dictionary<int, int>();
        var gScore = new Dictionary<int, float> { [from.id] = 0 };
        var fScore = new Dictionary<int, float> { [from.id] = DistanceXY(from.position, to.position) };

        (Node[], float[]) ReconstructPath(int current)
        {
            var path = new List<Node> { nodesDict[current] };
            var distances = new List<float>();
            while (cameFrom.TryGetValue(current, out var previous))
            {
                distances.Add(DistanceXY(nodesDict[current].position, nodesDict[previous].position));
                path.Add(nodesDict[previous]);
                current = previous;
            }
            path.Reverse();
            distances.Reverse();
            return (path.ToArray(), distances.ToArray());
        }

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(id => fScore[id]).First();

            if (current == to.id)
            {
                var (nodesPath, distancesPath) = ReconstructPath(current);
                path = new Path(nodesPath, distancesPath);
                return true;
            }

            openSet.Remove(current);

            foreach (var segment in nodesDict[current].segments)
            {
                if (segmentTypeMask.HasFlag(segment.type) == false)
                    continue;

                var neighbor = segment.n0 == current ? segment.n1 : segment.n0;
                var tentativeGScore = gScore[current] + segment.length;

                if (!gScore.TryGetValue(neighbor, out var gScoreNeighbor) || tentativeGScore < gScoreNeighbor)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + DistanceXY(nodesDict[neighbor].position, to.position);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        path = default;
        return false;
    }

    public bool TryFindPath(Vector3 from, Vector3 to, out Path path)
    {
        var fromNode = FindNearestNode(from);
        var toNode = FindNearestNode(to);
        return TryFindPath(fromNode, toNode, out path);
    }

    public void DrawGizmos() =>
        DrawGizmos(new Color(0, 0.5f, 1));

    public void DrawGizmos(Color color)
    {
        Gizmos.color = color;

        foreach (var node in nodesDict.Values)
            Gizmos.DrawSphere(node.position, 0.033f);

        foreach (var link in segmentsDict.Values)
        {
            var n0 = nodesDict[link.n0];
            var n1 = nodesDict[link.n1];

            if (link.type == Segment.Type.Ground)
            {
                Gizmos.DrawLine(n0.position, n1.position);
            }
            else
            {
                GizmosUtils.DrawParabola(n0.position, n1.position, (n0.position - n1.position).magnitude * 0.5f);
            }
        }
    }
}


