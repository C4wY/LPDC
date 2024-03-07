using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public struct InputEntry
    {
        public float time;
        public float x;
        public bool foreground, background;
        public bool jump;
    }

    public class LeaderController : MonoBehaviour
    {
        [System.Serializable]
        public class LeaderControllerParameters
        {
            public float movePointHistoryDistance = 0.25f;

            public bool drawGizmos = true;
        }

        public LeaderControllerParameters parameters = new();

        readonly Trace trace = new();

        InputEntry input;

        Move move;

        void JumpUpdate()
        {
            if (input.jump)
            {
                if (move.TryToJump())
                {
                    var movePoint = new TracePoint
                    {
                        position = transform.position,
                        time = Time.time,
                        input = input,
                        actions = TracePoint.Action.Jump,
                    };
                    trace.Add(movePoint);
                }
            }
        }

        void FollowUpdate()
        {
            if (Time.frameCount % 10 == 0)
            {
                var camera = FindAnyObjectByType<CinemachineCamera>();
                if (camera != null)
                    camera.Follow = transform;
            }
        }

        void MoveHistoryUpdate()
        {
            var delta = transform.position - trace.Current.position;
            if (delta.sqrMagnitude > parameters.movePointHistoryDistance * parameters.movePointHistoryDistance)
            {
                var movePoint = new TracePoint
                {
                    position = transform.position,
                    time = Time.time,
                    input = input,
                    actions = TracePoint.Action.None,
                };
                trace.Add(movePoint);
            }
        }

        void OnEnable()
        {
            move = GetComponent<Move>();
        }

        void Update()
        {
            JumpUpdate();
            move.GoForegroundUpdate();
            FollowUpdate();
        }

        void FixedUpdate()
        {
            input = new InputEntry
            {
                time = Time.time,
                jump = Input.GetButtonDown("Jump"),
                x = Input.GetAxis("Horizontal"),
                foreground = Input.GetAxis("Vertical") < -0.1f,
                background = Input.GetAxis("Vertical") > 0.1f,
            };

            move.HorizontalMoveUpdate(input.x);
            move.UpdateGroundPoint();
            MoveHistoryUpdate();
        }

        void OnDrawGizmos()
        {
            if (parameters.drawGizmos)
            {
                trace.DrawGizmos();
            }
        }
    }
}
