using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Avatar
{
    [ExecuteAlways]
    public class Avatar : MonoBehaviour
    {
        public enum AvatarRole
        {
            Leader,
            Follower,
        }

        public enum AvatarName
        {
            Sora,
            Dooms,
        }

        public AvatarParameters parameters;

        public AvatarParameters SafeParameters =>
            parameters == null ? parameters = ScriptableObject.CreateInstance<AvatarParameters>() : parameters;

        public AvatarRole avatarRole;
        public bool IsLeader => avatarRole == AvatarRole.Leader;
        public bool IsFollower => avatarRole == AvatarRole.Follower;

        public AvatarName avatarName;
        public bool IsSora => avatarName == AvatarName.Sora;
        public bool IsDooms => avatarName == AvatarName.Dooms;

        public LeaderController LeaderController { get; private set; }
        public FollowerController FollowerController { get; private set; }
        public Move Move { get; private set; }
        public Ground Ground { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public OneSidedPlatformAgent OneSidedPlatformAgent { get; private set; }
        public Santé Santé { get; private set; }

        void RoleUpdate()
        {
            LeaderController.enabled = avatarRole == AvatarRole.Leader;
            FollowerController.enabled = avatarRole == AvatarRole.Follower;

            var soraIsLeader = GetLeaderFollower().leader.avatarName == AvatarName.Sora;

#if UNITY_EDITOR
            // Do not change the name of the object in prefab mode.
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                gameObject.name = $"{GetType().Name}-{avatarName} ({avatarRole})";
#endif
        }

        void OnEnable()
        {
            LeaderController = GetComponent<LeaderController>();
            FollowerController = GetComponent<FollowerController>();
            Move = GetComponent<Move>();
            Ground = GetComponent<Ground>();
            Rigidbody = GetComponent<Rigidbody>();
            OneSidedPlatformAgent = GetComponentInChildren<OneSidedPlatformAgent>();
            Santé = GetComponent<Santé>();

            RoleUpdate();
        }

        void Update()
        {
            RoleUpdate();
        }

        #region Static methods

        public static Avatar[] GetAllAvatars()
        {
            return FindObjectsByType<Avatar>(FindObjectsSortMode.None);
        }

        public static (Avatar leader, Avatar follower) GetLeaderFollower()
        {
            var avatars = GetAllAvatars();
            return (
                avatars.FirstOrDefault(a => a.IsLeader),
                avatars.FirstOrDefault(a => a.IsFollower));
        }

        public static (Avatar sora, Avatar Dooms) GetSoraDooms()
        {
            var avatars = GetAllAvatars();
            return (
                avatars.FirstOrDefault(a => a.avatarName == AvatarName.Sora),
                avatars.FirstOrDefault(a => a.avatarName == AvatarName.Dooms));
        }

        public static Avatar GetLeader()
        {
            var (leader, _) = GetLeaderFollower();
            return leader;
        }

        public static Avatar GetFollower()
        {
            var (_, follower) = GetLeaderFollower();
            return follower;
        }

        public static Avatar GetTheOther(Avatar avatar)
        {
            var (leader, follower) = GetLeaderFollower();
            return avatar == leader ? follower : leader;
        }

        public static void UpdateAllAvatar()
        {
            foreach (var avatar in GetAllAvatars())
            {
                avatar.RoleUpdate();
#if UNITY_EDITOR
                EditorUtility.SetDirty(avatar);
#endif
            }
        }

        #endregion

#if UNITY_EDITOR
        void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                // Debug.Log($"leader: {leader}, follower: {follower}");
                if (LeaderController != null)
                    UpdateAllAvatar();
            };
        }

        public static string AvatarPositionPath =>
            System.IO.Path.Join(Application.persistentDataPath, "tmp", "avatar-positions.json");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            // Load avatars positions when entering edit mode.
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredEditMode)
                {
                    var positions = SerializableDictionary<int, Vector3>.FromJsonFile(AvatarPositionPath);

                    foreach (var avatar in GetAllAvatars())
                    {
                        var id = avatar.gameObject.GetInstanceID();
                        if (positions.TryGetValue(id, out var position))
                            avatar.transform.position = position;
                    }

                    System.IO.File.Delete(AvatarPositionPath);
                }
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

                var otherRole = Target.avatarRole == AvatarRole.Leader ? AvatarRole.Follower : AvatarRole.Leader;
                if (GUILayout.Button("Switch Roles (Leader/Follower)"))
                {
                    Undo.RecordObjects(GetAllAvatars(), "Switch role");
                    var (leader, follower) = GetLeaderFollower();
                    (leader.avatarRole, follower.avatarRole) = (follower.avatarRole, leader.avatarRole);
                    UpdateAllAvatar();
                }

                if (GUILayout.Button("Regroup all Avatar"))
                {
                    foreach (var avatar in GetAllAvatars())
                    {
                        if (avatar == Target)
                            continue;

                        avatar.transform.position = Target.transform.position + Vector3.right * 0.2f;
                    }
                }

                GUI.enabled = Application.isPlaying;
                if (GUILayout.Button("Save Avatars Positions"))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(AvatarPositionPath));

                    var avatars = GetAllAvatars();
                    SerializableDictionary
                        .Create(avatars.Select(a => (a.gameObject.GetInstanceID(), a.transform.position)))
                        .ToJsonFile(AvatarPositionPath, prettyPrint: true);
                }
            }
        }
#endif
    }
}
