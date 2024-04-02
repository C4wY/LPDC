using System.Collections.Generic;
using UnityEngine;

public partial class NavGraph
{
    public class Node
    {
        static int nextId = 0;

        public readonly int id = nextId++;

        public Vector3 position;

        public Vector2 PositionXY =>
            new(Mathf.Round(position.x / 0.1f) * 0.1f, Mathf.Round(position.y / 0.1f) * 0.1f);

        public List<Segment> segments = new();
    }
}


