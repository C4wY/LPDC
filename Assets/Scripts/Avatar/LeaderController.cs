using Unity.Cinemachine;
using UnityEngine;

namespace Avatar
{
    public struct InputEntry
    {
        public float time;
        public float horizontal, vertical;
        public bool foreground, background;
        public bool jump;
    }

    [System.Serializable]
    public class LeaderControllerParameters
    {
        public float traceIntervalDistanceMax = 0.25f;

        public bool drawGizmos = true;
    }

    [ExecuteAlways]
    public class LeaderController : MonoBehaviour
    {
        public readonly Trace trace = new();

        public InputEntry input;

        Avatar avatar;
        Avatar Avatar =>
            avatar != null ? avatar : avatar = GetComponent<Avatar>();

        public LeaderControllerParameters Parameters =>
            Avatar.SafeParameters.leaderController;

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

        void CameraFollowUpdate()
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
            if (delta.sqrMagnitude > Parameters.traceIntervalDistanceMax * Parameters.traceIntervalDistanceMax)
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
            CameraFollowUpdate();
        }

        void FixedUpdate()
        {
            input = new InputEntry
            {
                time = Time.time,
                jump = wannaJump,
                horizontal = Input.GetAxis("Horizontal"),
                vertical = Input.GetAxis("Vertical"),
                foreground = Input.GetAxis("Vertical") < -0.1f,
                background = Input.GetAxis("Vertical") > 0.1f,
            };

            wannaJump = false;

            avatar.Move.UpdateHorizontal(input.horizontal);
            avatar.Move.GoForegroundUpdate(input.vertical);
            avatar.Move.UpdateZ();
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
