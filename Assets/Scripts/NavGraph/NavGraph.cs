using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class NavGraph
{
    readonly Dictionary<int, Node> nodes = new();
    readonly Dictionary<(int n0, int n1), Segment> segments = new();

    static readonly RaycastHit[] raycastHits = new RaycastHit[10];
    static readonly Collider[] overlaps = new Collider[10];

    public void Clear()
    {
        nodes.Clear();
        segments.Clear();
    }

    public Node GetNode(int id) =>
        nodes[id];

    public (Node, Node) GetNodes(int id0, int id1) =>
        (nodes[id0], nodes[id1]);

    public (Node, Node) GetNodes(Segment segment) =>
        (nodes[segment.n0], nodes[segment.n1]);

    public Segment GetSegment(int n0, int n1)
    {
        if (n0 > n1)
            (n0, n1) = (n1, n0);

        return segments[(n0, n1)];
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
                        var xy = node.PositionXY;

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
            nodes.Add(node.id, node);
    }

    public bool TryGetSegment(int n0, int n1, out Segment link)
    {
        if (n0 > n1)
            (n0, n1) = (n1, n0);

        return segments.TryGetValue((n0, n1), out link);
    }

    void CreateSegment(Node node0, Node node1, float distance, Vector3 direction)
    {
        var n0 = node0.id;
        var n1 = node1.id;

        if (n0 > n1)
            (n0, n1) = (n1, n0);

        var link = new Segment(this, n0, n1, distance, direction);

        node0.segments.Add(link);
        node1.segments.Add(link);

        segments.Add((n0, n1), link);
    }

    public void ConnectNodes(float maxDistance = 1.5f)
    {
        var nodeArray = nodes.Values.ToArray();
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

    public Node FindNearestNode(Vector3 position)
    {
        if (nodes.Count == 0)
            return null;

        return nodes.Values.OrderBy(n => (n.position - position).sqrMagnitude).First();
    }

    public (Segment segment, float t) FindNearestSegment(Vector3 position)
    {
        if (segments.Count == 0)
            return (null, float.NaN);

        return segments.Values
            .Select(segment =>
            {
                var n0 = nodes[segment.n0];
                var n1 = nodes[segment.n1];
                var d = n1.position - n0.position;
                MathUtils.NearestPointOnLine(n0.position, d, position, out var t);
                return (segment, t);
            })
            .OrderBy(item =>
            {
                var (segment, t) = item;
                var n0 = nodes[segment.n0];
                var n1 = nodes[segment.n1];
                var d = n1.position - n0.position;
                var p = n0.position + d * Mathf.Clamp01(t);
                return (p - position).sqrMagnitude;
            })
            .First();
    }

    public bool TryFindPath(Node from, Node to, out Node[] path)
    {
        var openSet = new HashSet<int> { from.id };
        var cameFrom = new Dictionary<int, int>();
        var gScore = new Dictionary<int, float> { [from.id] = 0 };
        var fScore = new Dictionary<int, float> { [from.id] = Vector3.Distance(from.position, to.position) };

        Node[] ReconstructPath(int current)
        {
            var path = new List<Node> { nodes[current] };
            while (cameFrom.TryGetValue(current, out var previous))
            {
                path.Add(nodes[previous]);
                current = previous;
            }
            path.Reverse();
            return path.ToArray();
        }

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(id => fScore[id]).First();

            if (current == to.id)
            {
                path = ReconstructPath(current);
                return true;
            }

            openSet.Remove(current);

            foreach (var link in nodes[current].segments)
            {
                var neighbor = link.n0 == current ? link.n1 : link.n0;
                var tentativeGScore = gScore[current] + link.length;

                if (!gScore.TryGetValue(neighbor, out var gScoreNeighbor) || tentativeGScore < gScoreNeighbor)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    // fScore[neighbor] = gScore[neighbor] + Vector3.Distance(nodes[neighbor].position, to.position);
                    fScore[neighbor] = gScore[neighbor] + link.length;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        path = null;
        return false;
    }

    public bool TryFindPath(Vector3 from, Vector3 to, out Node[] path)
    {
        var fromNode = FindNearestNode(from);
        var toNode = FindNearestNode(to);
        return TryFindPath(fromNode, toNode, out path);
    }

    public void DrawGizmos()
    {
        foreach (var node in nodes.Values)
            Gizmos.DrawSphere(node.position, 0.033f);

        foreach (var link in segments.Values)
        {
            var n0 = nodes[link.n0];
            var n1 = nodes[link.n1];
            Gizmos.DrawLine(n0.position, n1.position);
        }
    }
}


