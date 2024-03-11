using System.Linq;
using UnityEngine;

public partial class NavGraph
{
    public class Agent
    {
        public class PathPoint
        {
            public readonly Vector3 position;
            public readonly Node node;

            public PathPoint(Vector3 position, Node node)
            {
                this.position = position;
                this.node = node;
            }
        }

        public class PathSegment
        {
            public readonly PathPoint a, b;
            public readonly float length;
            public readonly float previouslyAccumulatedLength;

            public readonly bool requiresJump;

            public Vector3 AB => b.position - a.position;
            public Vector3 Direction => AB / length;
            public Vector3 MidPoint => a.position + AB / 2;

            public bool IsNodeToNode => a.node != null && b.node != null;

            public PathSegment(PathPoint a, PathPoint b, float length, float previouslyAccumulatedLength)
            {
                this.a = a;
                this.b = b;
                this.length = length;
                this.previouslyAccumulatedLength = previouslyAccumulatedLength;

                bool ComputeRequiresJump()
                {
                    if (b.position.y > a.position.y)
                    {
                        const float MAX_ANGLE = 30;
                        var angle = Mathf.Atan2(AB.y, Mathf.Abs(AB.x)) * Mathf.Rad2Deg; // Z is ignored

                        if (angle > MAX_ANGLE)
                            return true;

                        var ray = new Ray(a.position + Vector3.up * 0.2f, Direction);
                        if (Physics.Raycast(ray, length, LayerMask.GetMask("LevelBlock")))
                            return true;
                    }
                    return false;
                }

                requiresJump = ComputeRequiresJump();
            }

            public float DistanceAt(float t) =>
                previouslyAccumulatedLength + length * Mathf.Clamp01(t);

            public Vector3 PositionAt(float t) =>
                a.position + AB * Mathf.Clamp01(t);
        }

        public NavGraph graph;

        public PathPoint[] points = { };
        public PathSegment[] segments = { };
        public float TotalLength { get; private set; }

        public (Segment segment, float t) startSegment, endSegment;

        public int segmentIndex;
        public float segmentProgress;

        public bool HasPath =>
            segments.Length > 0;

        public PathSegment CurrentSegment =>
            segments[segmentIndex];

        public float CurrentDistance =>
            CurrentSegment.DistanceAt(segmentProgress);

        public float RemainingDistance =>
            TotalLength - CurrentDistance;

        public float CurrentProgress =>
            CurrentDistance / TotalLength;

        public Vector3 CurrentPosition =>
            segments[segmentIndex].PositionAt(segmentProgress);

        public Vector3 CurrentDirection =>
            segments[segmentIndex].Direction;

        public void ClearPath()
        {
            graph = null;
            points = new PathPoint[] { };
            segments = new PathSegment[] { };
            TotalLength = 0;
        }

        public void FindPath(NavGraph graph, Vector3 from, Vector3 to)
        {
            this.graph = graph;

            startSegment = graph.FindNearestSegment(from);
            endSegment = graph.FindNearestSegment(to);

            var found = graph.TryFindPath(
                startSegment.t < 0.5 ? startSegment.segment.Node0 : startSegment.segment.Node1,
                endSegment.t < 0.5 ? endSegment.segment.Node0 : endSegment.segment.Node1,
                out var nodes);

            if (found == false)
                return;

            var points = nodes
                .Select(node => new PathPoint(node.position, node))
                .ToList();

            MathUtils.NearestPointOnLine(
                points[0].position,
                points[1].position - points[0].position,
                from, out var fromT);

            if (fromT > 0 && fromT < 1)
                points = points
                    .Skip(1)
                    .ToList();

            MathUtils.NearestPointOnLine(
                points[^2].position,
                points[^1].position - points[^2].position,
                to, out var toT);

            if (toT > 0 && toT < 1)
                points = points
                    .Take(points.Count - 1)
                    .ToList();

            points = points
                .Prepend(new PathPoint(from, null))
                .Append(new PathPoint(to, null))
                .ToList();

            this.points = points.ToArray();

            TotalLength = 0f;
            segments = points
                .Pairwise()
                .Select(pair =>
                {
                    var (a, b) = pair;
                    var length = Vector3.Distance(a.position, b.position);
                    var segment = new PathSegment(a, b, length, TotalLength);
                    TotalLength += length;
                    return segment;
                })
                .ToArray();
        }

        public void UpdatePosition(Vector3 sourcePosition)
        {
            if (segments.Length == 0)
                return;

            var smallestSqrDistance = float.MaxValue;
            foreach (var (index, segment) in segments.Entries())
            {
                MathUtils.NearestPointOnLine(
                    segment.a.position,
                    segment.AB,
                    sourcePosition,
                    out var t);
                var segmentPoint = segment.a.position + segment.AB * Mathf.Clamp01(t);
                var sqrDistance = (segmentPoint - sourcePosition).sqrMagnitude;
                if (sqrDistance < smallestSqrDistance)
                {
                    smallestSqrDistance = sqrDistance;
                    segmentIndex = index;
                    segmentProgress = t;
                }
            }

            if (segmentProgress > 0.99f)
            {
                if (segmentIndex + 1 < segments.Length)
                {
                    segmentIndex++;
                    segmentProgress = 0;
                }
                else
                {
                    segmentProgress = 1;
                }
            }
        }

        public void DrawGizmos()
        {
            if (points.Length == 0)
                return;

            Gizmos.color = Colors.red;
            Gizmos.DrawWireSphere(startSegment.segment.Position0, 0.1f);
            Gizmos.DrawWireSphere(startSegment.segment.Position1, 0.1f);

            Gizmos.matrix = Matrix4x4.identity;
            foreach (var segment in segments)
            {
                Gizmos.color = segment.IsNodeToNode
                    ? Colors.Hex("FF0")
                    : Colors.Hex("F60");

                GizmosUtils.DrawArrow(segment.a.position, segment.AB);

                if (segment.requiresJump)
                {
                    GizmosUtils.DrawLabel(segment.MidPoint, "JUMP");
                }
            }

            Gizmos.color = Colors.Hex("6FF");
            Gizmos.DrawSphere(segments[segmentIndex].PositionAt(segmentProgress), 0.1f);
        }
    }
}


