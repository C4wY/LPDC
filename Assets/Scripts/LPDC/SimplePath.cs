using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace LPDC
{
    public struct SimplePathPoint
    {
        public Vector3 position;
        public float time;
    }

    public class SimplePath
    {
        public readonly int capacity;
        public readonly float distanceThreshold;
        public int Count { get; private set; } = 0;

        readonly SimplePathPoint[] points;
        int index;

        public SimplePath(int capacity = 200, float distanceThreshold = 0.25f)
        {
            this.capacity = capacity;
            this.distanceThreshold = distanceThreshold;

            points = new SimplePathPoint[capacity];
            index = 0;
            Count = 0;
        }

        public void Clear()
        {
            Count = 0;
        }

        public SimplePathPoint Current =>
            points[(index - 1 + capacity) % capacity];

        public SimplePathPoint GetOld(int historyIndex)
        {
            historyIndex = Mathf.Clamp(historyIndex, 0, Count - 1);
            return points[(index - 1 - historyIndex + capacity) % capacity];
        }

        public IEnumerable<(int index, SimplePathPoint point)> Entries()
        {
            for (int i = 0; i < Count; i++)
                yield return (i, GetOld(i));
        }

        public bool TryAdd(Vector3 position)
        {
            return TryAdd(new SimplePathPoint()
            {
                position = position,
                time = Time.time,
            });
        }

        public bool TryAdd(SimplePathPoint movePoint)
        {
            var canAdd = Count == 0 || Vector3.Distance(Current.position, movePoint.position) > distanceThreshold;

            if (!canAdd)
                return false;

            points[index] = movePoint;
            index = (index + 1) % capacity;
            Count = Mathf.Min(Count + 1, capacity);

            return true;
        }

        public void DrawGizmos()
        {
            var last = Current;
            Gizmos.DrawSphere(last.position, 0.05f);
            for (int i = 1; i < Count; i++)
            {
                var current = GetOld(i);
                Gizmos.DrawSphere(current.position, 0.05f);
                Gizmos.DrawLine(last.position, current.position);
                last = current;
            }
        }

        public SimplePathPoint FromEnd(float distance)
        {
            if (Count == 0)
                return new SimplePathPoint();

            var last = Current;
            for (int i = 1; i < Count; i++)
            {
                var current = GetOld(i);
                var d = Vector3.Distance(last.position, current.position);
                if (d > distance)
                {
                    var t = (distance - d) / (d - distance);
                    return new SimplePathPoint()
                    {
                        position = Vector3.Lerp(current.position, last.position, t),
                        time = Mathf.Lerp(current.time, last.time, t),
                    };
                }
                distance -= d;
                last = current;
            }

            return last;
        }
    }
}
