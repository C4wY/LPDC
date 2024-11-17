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
    public partial class FollowerController : MonoBehaviour
    {
        public Color navGraphColor = new(0, 0.5f, 1);

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

                if (Target.enabled == false)
                    return;

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
                    Debug.LogError($"???");
                    Debug.LogError(exception);
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

