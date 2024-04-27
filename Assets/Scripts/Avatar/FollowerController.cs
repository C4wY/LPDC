using System.Collections;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Avatar
{
    [System.Serializable]
    public class FollowerControllerParameters
    {
        public float distanceToLeaderMin = 1.0f;
        public float distanceToLeaderMax = 3.0f;

        [Tooltip("If the \"y\" distance to the leader is greater than this value, the follower will be teleported to the leader (debug solution).")]
        public float yDistanceToLeaderMaxBeforeTeleporting = 30.0f;

        [Tooltip("The width in world units of the nav graph samples (samples are centered on the x position of the avatar).")]
        public int navGraphSampleWidth = 60;

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
        Avatar avatar, leaderAvatar;

        FollowerControllerParameters Parameters =>
            avatar.parameters.followerController;

        Vector3 deltaToLeader;
        float distanceToLeader;
        TracePoint? tracePoint;

        float navGraphTime = -1;
        readonly NavGraph navGraph = new();

        readonly NavGraph.Agent agent = new();

        float horizontalInput = 0;
        float verticalInput = 0;

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

            agent.UpdatePosition(avatar.Ground.FeetPosition);
        }

        void UpdateFollow()
        {
            if (agent.HasPath == false)
                return;

            var segmentIsAir = agent.CurrentSegment.graphSegment?.type.HasFlag(NavGraph.Segment.Type.Air) ?? false;
            if (agent.CurrentSegment.requiresJump || segmentIsAir)
                avatar.Move.TryToJump();

            bool HasToJumpButCant() => agent.CurrentSegment.requiresJump && avatar.Move.IsJumping == false;

            var dir = agent.CurrentSegment.Direction;
            horizontalInput =
                segmentIsAir
                    ? avatar.Move.JumpVelocityAtJumpTime.x > 0 ? 1 : -1
                    : (agent.RemainingDistance < 1f || HasToJumpButCant())
                        ? 0
                        : avatar.Ground.IsGrounded
                            ? (dir.x > 0 ? 1 : -1)
                            : horizontalInput; // Keep the last input if not grounded.

            var wannaGoForeground = dir.z < 0 && dir.y < 0;
            verticalInput = wannaGoForeground ? -1 : 0;

            avatar.Move.HorizontalUpdate(horizontalInput);
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
                if (avatar.Ground.IsGrounded)
                {
                    RefreshNavGraph();
                    agent.ClearPath();
                }
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
                    UpdateFollow();
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
                navGraph.DrawGizmos();

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
                base.OnInspectorGUI();

                EditorGUILayout.LabelField("Horizontal Input", $"{Target.horizontalInput}");
                EditorGUILayout.LabelField("Vertical Input", $"{Target.verticalInput}");

                EditorGUILayout.LabelField("Agent", EditorStyles.boldLabel);
                var agent = Target.agent;
                if (agent != null)
                {
                    EditorGUILayout.LabelField("Segment:", $"{agent.segmentIndex} / {agent.segments.Length} ({agent.segmentProgress * 100:F1})%");
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

