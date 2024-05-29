using UnityEngine;

public partial class NavGraph
{
    public class Segment
    {
        [System.Flags]
        public enum Type
        {
            Ground = 1 << 0,
            Air = 1 << 1,
        }

        readonly NavGraph navGraph;

        public readonly int n0;
        public readonly int n1;
        public readonly float length;
        public readonly Vector3 direction;
        public readonly Type type;

        public bool IsGround => type.HasFlag(Type.Ground);
        public bool IsAir => type.HasFlag(Type.Air);

        public Node Node0 => navGraph.nodes[n0];
        public Vector3 Position0 => Node0.position;

        public Node Node1 => navGraph.nodes[n1];
        public Vector3 Position1 => Node1.position;

        public Vector3 Delta => Position1 - Position0;

        public Segment(NavGraph navGraph, int n0, int n1, float length, Vector3 direction, Type type = Type.Ground)
        {
            this.navGraph = navGraph;
            this.n0 = n0;
            this.n1 = n1;
            this.length = length;
            this.direction = direction;
            this.type = type;
        }

        public Vector3 Position(float t)
        {
            return Vector3.Lerp(
                navGraph.nodes[n0].position,
                navGraph.nodes[n1].position,
                t);
        }

        public bool HasNode(int nodeId)
        {
            return n0 == nodeId || n1 == nodeId;
        }

        public int OtherNode(int nodeId)
        {
            return n0 == nodeId ? n1 : n0;
        }

        public override string ToString()
        {
            return $"{n0} - {n1} ({length:F3})";
        }
    }
}
