using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Avatar
{
    public class NavGraph
    {
        public class Node
        {
            static int nextId = 0;

            public readonly int id = nextId++;

            public Vector3 position;

            public Vector2 PositionXY =>
                new(Mathf.Round(position.x / 0.1f) * 0.1f, Mathf.Round(position.y / 0.1f) * 0.1f);

            public List<Link> links = new();
        }

        public class Link
        {
            public int n0;
            public int n1;
            public float cost;
        }

        readonly Dictionary<int, Node> nodes = new();
        readonly Dictionary<(int n0, int n1), Link> links = new();

        static readonly RaycastHit[] raycastHits = new RaycastHit[10];
        static readonly Collider[] overlaps = new Collider[10];

        public void Clear()
        {
            nodes.Clear();
            links.Clear();
        }

        public void SampleWorld(int xMin, int xMax, int zMin, int zMax, float originY = 100, float distance = 200)
        {
            var mask = LayerMask.GetMask("LevelBlock");

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
                        var overlapCount = Physics.OverlapCapsuleNonAlloc(hit.point + Vector3.up * 0.5f, hit.point + Vector3.up * 1.5f, 0.45f, overlaps, mask);
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

        void CreateLink(Node node0, Node node1, float distance)
        {
            var n0 = node0.id;
            var n1 = node1.id;

            if (n0 > n1)
                (n0, n1) = (n1, n0);

            var link = new Link
            {
                n0 = n0,
                n1 = n1,
                cost = distance,
            };

            node0.links.Add(link);
            node1.links.Add(link);

            links.Add((n0, n1), link);
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
                    var (dx, dy, dz) = node1.position - node0.position;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy); // Ignoring Z axis for distance calculation.

                    if (Mathf.Abs(dz) > 0.5f)
                    {
                        var node0BeforeCount = Physics.RaycastNonAlloc(new Ray(node0.position + Vector3.up * 0.5f, Vector3.back), raycastHits, 10.0f, LayerMask.GetMask("LevelBlock"));
                        var node1BeforeCount = Physics.RaycastNonAlloc(new Ray(node1.position + Vector3.up * 0.5f, Vector3.back), raycastHits, 10.0f, LayerMask.GetMask("LevelBlock"));

                        if (node0BeforeCount > 0 || node1BeforeCount > 0)
                            continue;
                    }

                    if (distance < maxDistance)
                        CreateLink(node0, node1, distance);
                }
            }
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

                foreach (var link in nodes[current].links)
                {
                    var neighbor = link.n0 == current ? link.n1 : link.n0;
                    var tentativeGScore = gScore[current] + link.cost;

                    if (!gScore.TryGetValue(neighbor, out var gScoreNeighbor) || tentativeGScore < gScoreNeighbor)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + Vector3.Distance(nodes[neighbor].position, to.position);

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
            var fromNode = nodes.Values.OrderBy(n => (n.position - from).sqrMagnitude).First();
            var toNode = nodes.Values.OrderBy(n => (n.position - to).sqrMagnitude).First();
            return TryFindPath(fromNode, toNode, out path);
        }


        public void DrawGizmos()
        {
            foreach (var node in nodes.Values)
                Gizmos.DrawSphere(node.position, 0.033f);

            foreach (var link in links.Values)
            {
                var n0 = nodes[link.n0];
                var n1 = nodes[link.n1];
                Gizmos.DrawLine(n0.position, n1.position);
            }
        }
    }

    [System.Serializable]
    public class FollowerControllerParameters
    {
        public float distanceToLeaderMin = 1.0f;
        public float distanceToLeaderMax = 5.0f;
    }

    [ExecuteAlways]
    public class FollowerController : MonoBehaviour
    {
        Avatar avatar, leaderAvatar;

        FollowerControllerParameters Parameters =>
            avatar.parameters.followerController;

        Vector3 deltaToLeader;
        float distanceToLeader;
        TracePoint? tracePoint;

        readonly NavGraph navGraph = new();
        NavGraph.Node[] path;

        void Follow()
        {
        }

        void OnEnable()
        {
            avatar = GetComponent<Avatar>();

            leaderAvatar = FindObjectsByType<Avatar>(FindObjectsSortMode.None)
                .Where(a => a != avatar)
                .FirstOrDefault();

            if (leaderAvatar == null)
                Debug.LogError("FollowerController: No leader found");
        }

        bool TryGetTracePointFromLeader()
        {
            var distanceMax = avatar.parameters.leaderController.traceIntervalDistanceMax * 1.1f;
            var sqrDistanceMax = distanceMax * distanceMax;

            foreach (var (index, tracePoint) in leaderAvatar.LeaderController.trace.Entries())
            {
                // Ignoring Z axis for distance calculation.
                var (dx, dy, _) = transform.position - tracePoint.position;
                if (dx * dx + dy * dy < sqrDistanceMax)
                {
                    this.tracePoint = tracePoint;
                    return true;
                }
            }

            return false;
        }

        void Update()
        {
            TryGetTracePointFromLeader();
        }

        void FixedUpdate()
        {
            if (leaderAvatar == null)
                return;

            deltaToLeader = leaderAvatar.transform.position - avatar.transform.position;
            distanceToLeader = deltaToLeader.magnitude;

            if (distanceToLeader > Parameters.distanceToLeaderMax)
                Follow();
        }

        void OnDrawGizmos()
        {
            if (enabled == false || leaderAvatar == null)
                return;

            Gizmos.color = Colors.FromHex("6FF");
            Gizmos.DrawLine(avatar.transform.position, leaderAvatar.transform.position);
            GizmosUtils.DrawCircle(transform.position, Vector3.back, Parameters.distanceToLeaderMin);
            GizmosUtils.DrawCircle(transform.position, Vector3.back, Parameters.distanceToLeaderMax);

            if (tracePoint.HasValue)
                Gizmos.DrawWireSphere(tracePoint.Value.position, avatar.parameters.leaderController.traceIntervalDistanceMax * 1.1f);

            navGraph.DrawGizmos();

            if (path != null)
            {
                Gizmos.color = Colors.FromHex("F6F");
                GizmosUtils.DrawPath(path.Select(n => n.position), drawIntermediateSpheres: true);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(FollowerController))]
        public class FollowerControllerEditor : Editor
        {
            FollowerController Target =>
                (FollowerController)target;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Refresh NavGraph"))
                {
                    Target.navGraph.Clear();
                    Target.navGraph.SampleWorld(-30, 30, -10, 10);
                    Target.navGraph.ConnectNodes();
                    EditorUtility.SetDirty(Target);
                }

                if (GUILayout.Button("Find Path"))
                {
                    var from = Target.avatar.Ground.FeetPosition;
                    var to = Target.leaderAvatar.Ground.FeetPosition;
                    Target.navGraph.TryFindPath(from, to, out Target.path);
                    EditorUtility.SetDirty(Target);
                }
            }
        }
#endif
    }
}

