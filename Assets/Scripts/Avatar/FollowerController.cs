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

        void RefreshNavGraph()
        {
            var x = Mathf.FloorToInt(avatar.Ground.FeetPosition.x);
            var width = Parameters.navGraphSampleWidth;
            navGraph.Clear();
            navGraph.SampleWorld(x - width / 2, x + width / 2, 0, 5);
            navGraph.ConnectNodes();

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
            if (agent.CurrentSegment.requiresJump)
                avatar.Move.TryToJump();

            bool HasToJumpButCant() => agent.CurrentSegment.requiresJump && avatar.Move.IsJumping == false;

            var dir = agent.CurrentSegment.Direction;
            horizontalInput = agent.RemainingDistance < 1f || HasToJumpButCant()
                ? 0
                : avatar.Ground.IsGrounded
                    ? (dir.x > 0 ? 1 : -1)
                    : horizontalInput; // Keep the last input if not grounded.

            var wannaGoForeground = dir.z < 0 && dir.y < 0;
            verticalInput = wannaGoForeground ? -1 : 0;

            avatar.Move.UpdateHorizontal(horizontalInput);
            avatar.Move.GoForegroundUpdate(verticalInput);
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

            if (Time.time > navGraphTime + Parameters.navGraphObsolenceTime)
            {
                RefreshNavGraph();
                agent.ClearPath();
            }

            if (distanceToLeader > Parameters.distanceToLeaderMax)
            {
                UpdateAgent();
                UpdateFollow();
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

