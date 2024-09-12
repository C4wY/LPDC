using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace LPDC
{
    [ExecuteAlways]
    public class Avatar : MonoBehaviour
    {
        public enum Role
        {
            Leader,
            Follower,
        }

        public enum Name
        {
            Sora,
            Dooms,
        }

        public AvatarParameters parameters;

        public AvatarParameters SafeParameters =>
            parameters == null ? parameters = ScriptableObject.CreateInstance<AvatarParameters>() : parameters;

        public Role avatarRole;
        public bool IsLeader => avatarRole == Role.Leader;
        public bool IsFollower => avatarRole == Role.Follower;

        public Name avatarName;
        public bool IsSora => avatarName == Name.Sora;
        public bool IsDooms => avatarName == Name.Dooms;

        public LeaderController LeaderController { get; private set; }
        public FollowerController FollowerController { get; private set; }
        public Move Move { get; private set; }
        public Ground Ground { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public OneSidedPlatformAgent OneSidedPlatformAgent { get; private set; }
        public Santé Santé { get; private set; }
        public Attack Attack { get; private set; }

        void RoleUpdate()
        {
            LeaderController.enabled = avatarRole == Role.Leader;
            FollowerController.enabled = avatarRole == Role.Follower;

            var layerName = avatarRole == Role.Leader ? "Leader" : "Follower";
            var layer = LayerMask.NameToLayer(layerName);
            gameObject.layer = layer;
            var descendants = GetComponentsInChildren<Transform>();
            foreach (var descendant in descendants)
                descendant.gameObject.layer = layer;

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
            Attack = GetComponent<Attack>();

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

        /// <summary>
        /// Warning: If there are more than one leader or follower, the first 
        /// one will be returned. Inversely, if there are no leader or follower,
        /// the result will be null.
        /// </summary>
        public static (Avatar leader, Avatar follower) GetLeaderFollower()
        {
            var avatars = GetAllAvatars();
            return (
                avatars.FirstOrDefault(a => a.IsLeader),
                avatars.FirstOrDefault(a => a.IsFollower));
        }

        /// <summary>
        /// 
        /// </summary>
        public static (Avatar sora, Avatar dooms) GetSoraDooms()
        {
            var avatars = GetAllAvatars();
            return (
                avatars.FirstOrDefault(a => a.avatarName == Name.Sora),
                avatars.FirstOrDefault(a => a.avatarName == Name.Dooms));
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

        public static bool AvatarsAreValid()
        {
            var (leader, follower) = GetLeaderFollower();
            var (sora, dooms) = GetSoraDooms();
            return leader != null && follower != null && sora != null && dooms != null;
        }

        /// <summary>
        /// Try to fix the roles and names of the avatars. If there are more or 
        /// less than 2 avatars, an warning will be logged and nothing happens.
        /// </summary>
        public static bool TryFixAvatarRolesAndNames()
        {
            var avatars = GetAllAvatars();

            if (avatars.Length != 2)
            {
                Debug.LogError("There must be exactly 2 avatars in the scene.");
                return false;
            }

            var (leader, _) = GetLeaderFollower();
            if (leader == null)
                leader = avatars.FirstOrDefault();

            var (sora, _) = GetSoraDooms();
            if (sora == null)
                sora = avatars.FirstOrDefault();

            bool changed = false;
            foreach (var avatar in avatars)
            {
                var role = avatar == leader ? Role.Leader : Role.Follower;
                var name = avatar == sora ? Name.Sora : Name.Dooms;
                if (avatar.avatarRole != role)
                {
                    avatar.avatarRole = role;
                    changed = true;
                }
                if (avatar.avatarName != name)
                {
                    avatar.avatarName = name;
                    changed = true;
                }
            }
            return changed;
        }

        public static void SetLeader(Avatar avatar)
        {
            var (sora, dooms) = GetSoraDooms();
            if (avatar == sora)
            {
                sora.avatarRole = Role.Leader;
                dooms.avatarRole = Role.Follower;
            }
            else
            {
                sora.avatarRole = Role.Follower;
                dooms.avatarRole = Role.Leader;
            }
            UpdateAllAvatar();
        }

        /// <summary>
        /// If roles are not properly set, the first avatar will be set as leader.
        /// </summary>
        public static void EnsureRoles()
        {
            var currentLeader = GetAllAvatars().FirstOrDefault(a => a.IsLeader);
            if (currentLeader == null)
                currentLeader = GetAllAvatars().FirstOrDefault();
            SetLeader(currentLeader);
        }

        public static void SwitchRoles()
        {
            SetLeader(GetAllAvatars().FirstOrDefault(a => a.IsFollower));
        }

        public static void SwitchNames()
        {
            var (sora, dooms) = GetSoraDooms();
            (dooms.avatarName, sora.avatarName) = (sora.avatarName, dooms.avatarName);
            UpdateAllAvatar();
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

                var otherRole = Target.avatarRole == Role.Leader ? Role.Follower : Role.Leader;
                if (GUILayout.Button("Switch Roles (Leader / Follower)"))
                {
                    Undo.RecordObjects(GetAllAvatars(), "Switch Roles");
                    SwitchRoles();
                }

                if (GUILayout.Button("Switch Names (Sora / Dooms)"))
                {
                    Undo.RecordObjects(GetAllAvatars(), "Switch Names");
                    SwitchNames();
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
