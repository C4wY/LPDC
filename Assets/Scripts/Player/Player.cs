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

        LeaderController leader;
        FollowerController follower;

        void RoleUpdate()
        {
            leader.enabled = role == PairRole.Leader;
            follower.enabled = role == PairRole.Follower;
        }

        void OnEnable()
        {
            leader = GetComponent<LeaderController>();
            follower = GetComponent<FollowerController>();

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
                if (leader != null)
                    RoleUpdate();
            };
        }
#endif
    }
}
