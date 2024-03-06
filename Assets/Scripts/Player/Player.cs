using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Player
{
    [ExecuteAlways]
    public class Player : MonoBehaviour
    {
        public enum PairRole
        {
            Leader,
            Follower,
        }

        public PairRole role;

        public bool IsLeader =>
            role == PairRole.Leader;

        public bool IsFollower =>
            role == PairRole.Follower;

        LeaderController leaderController;
        FollowerController followerController;

        void RoleUpdate()
        {
            leaderController.enabled = role == PairRole.Leader;
            followerController.enabled = role == PairRole.Follower;
            gameObject.name = $"{GetType().Name} ({role})";
        }

        void OnEnable()
        {
            leaderController = GetComponent<LeaderController>();
            followerController = GetComponent<FollowerController>();

            RoleUpdate();
        }

        void Update()
        {
            RoleUpdate();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                // Debug.Log($"leader: {leader}, follower: {follower}");
                if (leaderController != null)
                {
                    RoleUpdate();

                    foreach (var player in FindObjectsByType<Player>(FindObjectsSortMode.None))
                    {
                        if (player == this)
                            continue;

                        player.role = role == PairRole.Leader ? PairRole.Follower : PairRole.Leader;
                        player.RoleUpdate();
                    }
                }
            };
        }
#endif
    }
}
