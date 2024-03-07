using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

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

        public PlayerParameters parameters;

        public PairRole role;

        public PlayerParameters SafeParameters =>
            parameters == null ? parameters = ScriptableObject.CreateInstance<PlayerParameters>() : parameters;

        public bool IsLeader =>
            role == PairRole.Leader;

        public bool IsFollower =>
            role == PairRole.Follower;

        public LeaderController LeaderController { get; private set; }
        public FollowerController FollowerController { get; private set; }
        public Move Move { get; private set; }
        public Ground Ground { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        void RoleUpdate()
        {
            LeaderController.enabled = role == PairRole.Leader;
            FollowerController.enabled = role == PairRole.Follower;

#if UNITY_EDITOR
            // Do not change the name of the object in prefab mode.
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                gameObject.name = $"{GetType().Name} ({role})";
#endif
        }

        void OnEnable()
        {
            LeaderController = GetComponent<LeaderController>();
            FollowerController = GetComponent<FollowerController>();
            Move = GetComponent<Move>();
            Ground = GetComponent<Ground>();
            Rigidbody = GetComponent<Rigidbody>();

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
                if (LeaderController != null)
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
