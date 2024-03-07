using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Avatar
{
    [System.Serializable]
    public class FollowerControllerParameters
    {
        public float distanceToLeaderMin = 1.0f;
        public float distanceToLeaderMax = 5.0f;
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

        void Follow()
        {
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

            foreach (var (index, tracePoint) in leaderAvatar.LeaderController.trace.Entries())
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
            TryGetTracePointFromLeader();
        }

        void FixedUpdate()
        {
            if (leaderAvatar == null)
                return;

            deltaToLeader = leaderAvatar.transform.position - avatar.transform.position;
            distanceToLeader = deltaToLeader.magnitude;

            if (distanceToLeader > Parameters.distanceToLeaderMax)
                Follow();
        }

        void OnDrawGizmos()
        {
            if (leaderAvatar == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(avatar.transform.position, leaderAvatar.transform.position);
            GizmosUtils.DrawCircle(transform.position, Vector3.back, Parameters.distanceToLeaderMin);
            GizmosUtils.DrawCircle(transform.position, Vector3.back, Parameters.distanceToLeaderMax);

            if (tracePoint.HasValue)
                Gizmos.DrawWireSphere(tracePoint.Value.position, avatar.parameters.leaderController.traceIntervalDistanceMax * 1.1f);
        }
    }
}

