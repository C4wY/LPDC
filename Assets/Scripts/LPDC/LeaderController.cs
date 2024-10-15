using Unity.Cinemachine;
using UnityEngine;

namespace LPDC
{
    public struct InputEntry
    {
        public float time;
        public float horizontal, vertical;
        public bool foreground, background;
        public bool jump;
        public bool competenceFront, competenceBack;
    }

    [System.Serializable]
    public class LeaderControllerParameters
    {
        public float traceIntervalDistanceMax = 0.25f;

        [System.Flags]
        public enum GizmosMode
        {
            Trace = 1 << 0,
        }

        public GizmosMode gizmos = (GizmosMode)~0 ^ GizmosMode.Trace;
    }

    [ExecuteAlways]
    public class LeaderController : MonoBehaviour
    {
        public bool dialoguePause = false; // Pour la fonction de pause, ajout� par Dim

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

        void DashUpdate()
        {
            if (input.competenceFront)
            {
                avatar.Move.TryToDash();
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
            if (InputManager.Instance.DebugBothRespawn())
                avatar.Move.TeleportTo(avatar.Ground.LastGroundPosition);

            wannaJump |= Input.GetButtonDown("Jump");

            if (!dialoguePause) // J'emp�che le personnage de se d�placer pendant les dialogues (Dim)
            {
                JumpUpdate();
                DashUpdate();
                CameraFollowUpdate();
            }

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
                competenceFront = InputManager.Instance.CompetenceFront(),
            };

            wannaJump = false;

            if (!dialoguePause) // J'emp�che le personnage de se d�placer pendant les dialogues (Dim)
            {
                avatar.Move.HorizontalUpdate(input.horizontal);
                avatar.Move.VerticalUpdate(input.vertical);
                avatar.Move.DepthUpdate();
                TraceUpdate();
            }

        }

        void OnDrawGizmos()
        {
            if (Parameters.gizmos.HasFlag(LeaderControllerParameters.GizmosMode.Trace))
                trace.DrawGizmos();
        }

        // J'ajoute une fonction qui se d�clenche quand un dialogue se d�clenche, et une autre quand le dialogue se termine. (Dim)

        void OnPauseForDialogue()
        {
            dialoguePause = true;
        }

        void OffPauseForDialogue()
        {
            dialoguePause = false;
        }
    }
}
