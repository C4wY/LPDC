using Unity.Cinemachine;
using UnityEngine;

namespace Avatar
{
    public struct InputEntry
    {
        public float time;
        public float x;
        public bool foreground, background;
        public bool jump;
    }

    [System.Serializable]
    public class LeaderControllerParameters
    {
        public float movePointHistoryDistance = 0.25f;

        public bool drawGizmos = true;
    }

    [ExecuteAlways]
    public class LeaderController : MonoBehaviour
    {
        public readonly Trace trace = new();

        public InputEntry input;

        Avatar avatar;

        public LeaderControllerParameters Parameters =>
            avatar.SafeParameters.leaderController;

        void JumpUpdate()
        {
            if (input.jump)
            {
                if (avatar.Move.TryToJump())
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

        void TraceUpdate()
        {
            var delta = transform.position - trace.Current.position;
            if (delta.sqrMagnitude > Parameters.movePointHistoryDistance * Parameters.movePointHistoryDistance)
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
            avatar = GetComponent<Avatar>();
        }

        bool wannaJump;

        void Update()
        {
            wannaJump |= Input.GetButtonDown("Jump");

            JumpUpdate();
            avatar.Move.GoForegroundUpdate();
            FollowUpdate();
        }

        void FixedUpdate()
        {
            input = new InputEntry
            {
                time = Time.time,
                jump = wannaJump,
                x = Input.GetAxis("Horizontal"),
                foreground = Input.GetAxis("Vertical") < -0.1f,
                background = Input.GetAxis("Vertical") > 0.1f,
            };

            wannaJump = false;

            avatar.Move.HorizontalMoveUpdate(input.x);
            avatar.Move.UpdateGroundPoint();
            TraceUpdate();
        }

        void OnDrawGizmos()
        {
            if (Parameters.drawGizmos)
            {
                trace.DrawGizmos();
            }
        }
    }
}
