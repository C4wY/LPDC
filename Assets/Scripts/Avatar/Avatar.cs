using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Avatar
{
    [ExecuteAlways]
    public class Avatar : MonoBehaviour
    {
        public enum PairRole
        {
            Leader,
            Follower,
        }

        public AvatarParameters parameters;

        public PairRole role;

        public AvatarParameters SafeParameters =>
            parameters == null ? parameters = ScriptableObject.CreateInstance<AvatarParameters>() : parameters;

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
        void UpdateAllAvatar()
        {
            foreach (var player in FindObjectsByType<Avatar>(FindObjectsSortMode.None))
            {
                if (player != this)
                    player.role = role == PairRole.Leader ? PairRole.Follower : PairRole.Leader;

                player.RoleUpdate();
            }
        }
        void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                // Debug.Log($"leader: {leader}, follower: {follower}");
                if (LeaderController != null)
                    UpdateAllAvatar();
            };
        }

        [CustomEditor(typeof(Avatar))]
        public class AvatarEditor : Editor
        {
            Avatar Target =>
                (Avatar)target;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var otherRole = Target.role == PairRole.Leader ? PairRole.Follower : PairRole.Leader;
                if (GUILayout.Button($"Switch to {otherRole}"))
                {
                    Target.role = otherRole;
                    Target.UpdateAllAvatar();
                }

                if (GUILayout.Button("Regroup all Avatar"))
                {
                    foreach (var player in FindObjectsByType<Avatar>(FindObjectsSortMode.None))
                    {
                        if (player == Target)
                            continue;

                        player.transform.position = Target.transform.position + Vector3.right * 0.2f;
                    }
                }
            }
        }
#endif
    }
}
