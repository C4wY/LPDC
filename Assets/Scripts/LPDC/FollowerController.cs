using System.Collections;
using System.Linq;
using UnityEngine;
using NUnit.Framework.Internal.Filters;


#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.EditorGUILayout;
#endif

namespace LPDC
{
    [System.Serializable]
    public class FollowerControllerParameters
    {
        public float distanceToLeaderMin = 1.0f;
        public float distanceToLeaderMax = 3.0f;

        [Tooltip("If the \"y\" distance to the leader is greater than this value, the follower will be teleported to the leader (debug solution).")]
        public float yDistanceToLeaderMaxBeforeTeleporting = 30.0f;

        [Tooltip("The width in world units of the nav graph samples (samples are centered on the x position of the avatar).")]
        public int navGraphSampleWidth = 100;

        [Tooltip("The time in seconds to wait before refreshing the navigation graph (and the path).")]
        public float navGraphObsolenceTime = 1.0f;

        [System.Flags]
        public enum GizmosMode
        {
            DistanceToLeader = 1 << 0,
            NavGraph = 1 << 1,
            Agent = 1 << 2,
        }

        public GizmosMode gizmos = (GizmosMode)~0;
    }

    [ExecuteAlways]
    public class FollowerController : MonoBehaviour
    {
        public Color navGraphColor = new(0, 0.5f, 1);

        Avatar avatar, leaderAvatar;

        FollowerControllerParameters Parameters =>
            avatar.parameters.followerController;

        Vector3 deltaToLeader;
        float distanceToLeader;
        TracePoint? tracePoint;

        Vector3 deltaToAgent;
        float distanceToAgent;

        Vector3 deltaPathToLeader;
        float distancePathToLeader;

        float navGraphTime = -1;
        readonly NavGraph navGraph = new();

        readonly NavGraph.Agent agent = new();

        enum Phases
        {
            NONE,

            JUMP_PROGRESS,
            JUMP_POST_WAITING,
            JUMP_CANNOT,

            STOP,
            MOVE,
            FALL,
        }

        Phases phases = Phases.NONE;

        class JumpState
        {
            public const float POST_JUMP_TIMER_LIMIT = 0.2f;
            public readonly NavGraph.Agent agent;

            public int segmentIndex = -1;
            public float segmentProgress = 0;
            public float postJumpTimer = 0;

            public bool Jumping => segmentIndex != -1;
            public float PostJumpTimerProgress => postJumpTimer / POST_JUMP_TIMER_LIMIT;

            public NavGraph.Agent.AgentSegment AgentSegment =>
                segmentIndex != -1 ? agent.segments[segmentIndex] : null;

            public JumpState(NavGraph.Agent agent) { this.agent = agent; }

            public bool PostJumpTimerComplete()
            {
                return postJumpTimer > POST_JUMP_TIMER_LIMIT;
            }

            public bool IncrementPostJumpTimer(float deltaTime)
            {
                postJumpTimer += deltaTime;
                return PostJumpTimerComplete();
            }

            public void Reset()
            {
                segmentIndex = -1;
                segmentProgress = 0;
                postJumpTimer = 0;
            }

            public override string ToString()
            {
                return (AgentSegment?.ToString() ?? "None")
                    + $"\nProgress: {100 * segmentProgress:F1}%"
                    + $"\nPost Jump Timer: {postJumpTimer:F3} {100 * PostJumpTimerProgress:F1}%";
            }
        }
        readonly JumpState jumpState;

        float horizontalInput = 0;
        float verticalInput = 0;

        string followDebugInfo;

        FollowerController()
        {
            jumpState = new(agent);
        }

        public void TeleportBehindLeader()
        {
            var dx = (leaderAvatar.Move.IsFacingRight ? -1 : 1) * 0.25f;
            var dy = 0.5f;
            var position = leaderAvatar.Ground.LastGroundPosition + new Vector3(dx, dy, 0);
            avatar.Move.TeleportTo(position);
        }

        void RefreshNavGraph()
        {
            var x = Mathf.FloorToInt(avatar.Ground.FeetPosition.x);
            var width = Parameters.navGraphSampleWidth;

            navGraph.Clear();
            navGraph.SampleWorld(x - width / 2, x + width / 2, 0, 5);
            navGraph.RemoveDangerNodes();
            navGraph.CreateGroundSegments();
            navGraph.CreateAirSegments();

            navGraphTime = Time.time;
        }

        void FindPath()
        {
            RefreshNavGraph();
            agent.FindPath(navGraph, avatar.Ground.FeetPosition, leaderAvatar.Ground.FeetPosition);
        }

        void UpdateAgent()
        {
            if (agent.HasPath == false)
                FindPath();

            if (jumpState.Jumping)
            {
                // While jumping, clamp the progression to the current single "jump" segment.
                agent.UpdatePosition(avatar.Ground.FeetPosition, jumpState.segmentIndex);
                jumpState.segmentProgress = agent.segmentProgress;
            }
            else
            {
                agent.UpdatePosition(avatar.Ground.FeetPosition);
            }

            deltaToAgent = agent.CurrentPosition - avatar.Ground.FeetPosition;
            distanceToAgent = deltaToAgent.magnitude;

            deltaPathToLeader = leaderAvatar.Ground.FeetPosition - agent.points.Last().position;
            distancePathToLeader = deltaPathToLeader.magnitude;
        }

        void UpdateFollow(float deltaTime)
        {
            if (agent.HasPath == false)
                return;

            var segmentIsAir = agent.CurrentSegment.graphSegment?.type.HasFlag(NavGraph.Segment.Type.Air) ?? false;
            var segmentIsAirOrRequireJump = agent.CurrentSegment.requiresJump || segmentIsAir;
            if (segmentIsAirOrRequireJump && agent.segmentProgress < 0.5f)
            {
                if (avatar.Move.TryToJump())
                {
                    jumpState.segmentIndex = agent.CurrentSegment.index;
                }
            }

            bool HasToJumpButCant() => agent.CurrentSegment.requiresJump && avatar.Move.IsJumping == false;

            var segmentDirection = agent.CurrentSegment.Direction;
            if (segmentIsAirOrRequireJump)
            {
                if (jumpState.Jumping)
                {
                    if (jumpState.segmentProgress < 1)
                    {
                        // Progressing along the jump direction.
                        phases = Phases.JUMP_PROGRESS;
                        var x = jumpState.AgentSegment.Direction.x;
                        horizontalInput = x > 0 ? 1 : -1;
                        followDebugInfo = $"Jumping, following the \"jump\" segment x direction ({x:F1})";
                    }
                    else
                    {
                        // Jump is done, please recover a little.
                        phases = Phases.JUMP_POST_WAITING;
                        horizontalInput = 0;
                        jumpState.IncrementPostJumpTimer(deltaTime);
                        followDebugInfo = $"Jumping done, cooldown ({jumpState.PostJumpTimerProgress * 100:F1}%).";
                        if (jumpState.PostJumpTimerComplete())
                            jumpState.Reset();
                    }
                }
                else
                {
                    phases = Phases.JUMP_CANNOT;
                    horizontalInput = 0;
                    followDebugInfo = "Not jumping, stopping";
                }
            }
            else
            {
                if (agent.RemainingDistance < 1f || HasToJumpButCant())
                {
                    phases = Phases.STOP;
                    horizontalInput = 0;
                    followDebugInfo = "Stopping";
                }
                else
                {
                    if (avatar.Ground.IsGrounded)
                    {
                        phases = Phases.MOVE;
                        horizontalInput = segmentDirection.x > 0 ? 1 : -1;
                        followDebugInfo = $"Grounded, following the segment direction {segmentDirection.x}";
                    }
                    else
                    {
                        phases = Phases.FALL;
                        followDebugInfo = "Not grounded, maintaining last input";
                    }
                }

            }
            avatar.Move.HorizontalUpdate(horizontalInput);

            var wannaGoForeground = segmentDirection.z < 0 && segmentDirection.y < 0;
            verticalInput = wannaGoForeground ? -1 : 0;
            avatar.Move.VerticalUpdate(verticalInput);
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

            foreach (var (_, tracePoint) in leaderAvatar.LeaderController.trace.Entries())
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
            if (InputManager.Instance.DebugFollowerRespawn())
                TeleportBehindLeader();

#if UNITY_EDITOR
            if (Application.isPlaying == false)
                UpdateAgent();
#endif
        }

        void FixedUpdate()
        {
            if (leaderAvatar == null)
                return;

            deltaToLeader = leaderAvatar.transform.position - avatar.transform.position;
            distanceToLeader = deltaToLeader.magnitude;

            if (Mathf.Abs(deltaToLeader.y) > Parameters.yDistanceToLeaderMaxBeforeTeleporting)
                // Teleport the follower to the leader (debug solution).
                TeleportBehindLeader();

            if (Time.time > navGraphTime + Parameters.navGraphObsolenceTime)
            {
                // Only if the avatar is grounded, refresh the nav graph.
                if (avatar.Ground.IsGroundedFor(3))
                {
                    RefreshNavGraph();
                    agent.ClearPath();
                }
            }

            // If the follower is too far from the path, the path should be recalculated.
            if (avatar.Ground.IsGrounded)
            {
                if (distanceToAgent > 1.5f)
                    FindPath();

                if (distancePathToLeader > 1.5f)
                    FindPath();
            }

            if (avatar.Move.mode == MoveMode.Switching)
            {
                avatar.Move.HorizontalUpdate(0);
            }
            else
            {
                var shouldFollow =
                    // If the follower is too far from the leader, it should follow the leader.
                    distanceToLeader > Parameters.distanceToLeaderMax
                    // If the follower is in the air, it should follow the leader.
                    || avatar.Ground.IsGrounded == false
                    // If the follower is on ground but on a "air" segment, it should follow the leader.
                    || (agent.HasCurrentSegment && (agent.CurrentSegment.graphSegment?.IsAir ?? false));

                if (shouldFollow)
                {
                    UpdateAgent();
                    UpdateFollow(Time.fixedDeltaTime);
                }
            }

            avatar.Move.UpdateZ();
        }

        void OnDrawGizmos()
        {
            if (enabled == false || leaderAvatar == null)
                return;

            var mode = Parameters.gizmos;

            if (mode.HasFlag(FollowerControllerParameters.GizmosMode.DistanceToLeader))
            {
                Gizmos.color = Colors.Hex("6FF");
                Gizmos.DrawLine(avatar.transform.position, leaderAvatar.transform.position);
                GizmosUtils.DrawCircle(transform.position, Vector3.back, Parameters.distanceToLeaderMin);
                GizmosUtils.DrawCircle(transform.position, Vector3.back, Parameters.distanceToLeaderMax);
            }

            if (tracePoint.HasValue)
                Gizmos.DrawWireSphere(tracePoint.Value.position, avatar.parameters.leaderController.traceIntervalDistanceMax * 1.1f);

            if (mode.HasFlag(FollowerControllerParameters.GizmosMode.NavGraph))
                navGraph.DrawGizmos(navGraphColor);

            if (mode.HasFlag(FollowerControllerParameters.GizmosMode.Agent))
                agent?.DrawGizmos();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(FollowerController))]
        public class FollowerControllerEditor : Editor
        {
            FollowerController Target =>
                (FollowerController)target;

            public override void OnInspectorGUI()
            {
                GUIStyle multilineStyle = new(EditorStyles.label) { wordWrap = true };
                base.OnInspectorGUI();

                try
                {
                    LabelField("Horizontal Input", $"{Target.horizontalInput}");
                    LabelField("Vertical Input", $"{Target.verticalInput}");

                    LabelField("Follower", EditorStyles.boldLabel);
                    LabelField("Phase", $"{Target.phases}");
                    LabelField("Debug", Target.followDebugInfo);
                    LabelField("Can jump", $"{Target.avatar.Move.CanJump()}");
                    LabelField("Jump State", $"{Target.jumpState}", multilineStyle);

                    LabelField("Agent", EditorStyles.boldLabel);
                }
                catch (System.Exception exception)
                {

                }

                var agent = Target.agent;
                if (agent != null)
                {
                    LabelField("Path:",
                        $"#{agent.segmentIndex} / {agent.segments.Length} ({agent.segmentProgress * 100:F1})% {agent.CurrentSegment?.graphSegment}"
                        + $"\nCur Seg. {agent.CurrentSegment}"
                        + $"\nRemaining Dist. {agent.RemainingDistance:F1}"
                        + $"\nDist. to agent {Target.distanceToAgent:F1}",
                        multilineStyle);
                }

                GUI.enabled = Target.enabled;
                if (GUILayout.Button("Refresh NavGraph and Path"))
                {
                    Target.FindPath();
                    EditorUtility.SetDirty(Target);
                }
            }
        }
#endif
    }
}

