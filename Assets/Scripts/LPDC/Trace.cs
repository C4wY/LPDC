using System.Collections.Generic;
using UnityEngine;

namespace LPDC
{
    public struct TracePoint
    {
        [System.Flags]
        public enum Action
        {
            None = 0,
            GoForeground = 1,
            Jump = 2,
        }

        public Vector3 position;
        public float time;
        public InputEntry input;
        public Action actions;

        override public readonly string ToString() =>
            $"position: {position}, time: {time}, actions: {actions}";
    }

    public class Trace
    {
        public readonly int capacity;
        public int Count { get; private set; } = 0;

        readonly TracePoint[] points;
        int index;

        public Trace(int capacity = 200)
        {
            this.capacity = capacity;

            points = new TracePoint[capacity];
            index = 0;
            Count = 0;
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Add(TracePoint movePoint)
        {
            points[index] = movePoint;
            index = (index + 1) % capacity;
            Count = Mathf.Min(Count + 1, capacity);
        }

        public TracePoint Current =>
            points[(index - 1 + capacity) % capacity];

        public TracePoint GetOld(int historyIndex)
        {
            historyIndex = Mathf.Clamp(historyIndex, 0, Count - 1);
            return points[(index - 1 - historyIndex + capacity) % capacity];
        }

        public IEnumerable<(int index, TracePoint point)> Entries()
        {
            for (int i = 0; i < Count; i++)
                yield return (i, GetOld(i));
        }

        public TracePoint GetLatestBefore(float time)
        {
            // Initialize left and right pointers
            int left = 0;
            int right = Count - 1;

            // Perform binary search
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                var movePoint = GetOld(mid);
                if (movePoint.time < time)
                {
                    // If current movePoint's time is less than the specified time,
                    // check if the next movePoint's time is greater than or equal to the specified time.
                    // If true, return the current movePoint; otherwise, continue searching.
                    if (mid == Count - 1 || GetOld(mid + 1).time >= time)
                    {
                        return movePoint;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }
                else
                {
                    right = mid - 1;
                }
            }

            // If no movePoint with time less than the specified time is found,
            // return the movePoint at index 0.
            return GetOld(0);
        }

        public void DrawGizmos(Color? color1 = null, Color? color2 = null)
        {
            if (Count == 0)
                return;

            color1 ??= Colors.cyan;
            color2 ??= Colors.blue;

            var movePoint = GetOld(0);
            void DrawPoint()
            {
                Gizmos.DrawSphere(movePoint.position, 0.033f);
                if (movePoint.actions.HasFlag(TracePoint.Action.Jump))
                    GizmosUtils.DrawCircle(movePoint.position, Vector3.back, 0.1f);
            }

            var prevMovePoint = movePoint;
            Gizmos.color = color1.Value;
            DrawPoint();

            for (int i = 1; i < Count; i++)
            {
                movePoint = GetOld(i);

                Gizmos.color = Color.Lerp(color1.Value, color2.Value, (float)i / Count);
                DrawPoint();

                Gizmos.color = Color.Lerp(color1.Value, color2.Value, (i - 0.5f) / Count);
                Gizmos.DrawLine(prevMovePoint.position, movePoint.position);

                prevMovePoint = movePoint;
            }
        }
    }
}
