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
        public OneSidedPlatformAgent OneSidedPlatformAgent { get; private set; }
        public Santé Santé { get; private set; }

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
            OneSidedPlatformAgent = GetComponentInChildren<OneSidedPlatformAgent>();
            Santé = GetComponent<Santé>();

            RoleUpdate();
        }

        void Update()
        {
            RoleUpdate();
        }

#if UNITY_EDITOR
        static Avatar[] GetAllAvatars() =>
            FindObjectsByType<Avatar>(FindObjectsSortMode.None);

        void UpdateAllAvatar()
        {
            foreach (var avatar in GetAllAvatars())
            {
                if (avatar != this)
                    avatar.role = role == PairRole.Leader ? PairRole.Follower : PairRole.Leader;

                avatar.RoleUpdate();
                EditorUtility.SetDirty(avatar);
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

                var otherRole = Target.role == PairRole.Leader ? PairRole.Follower : PairRole.Leader;
                if (GUILayout.Button($"Switch to {otherRole}"))
                {
                    Undo.RecordObjects(GetAllAvatars(), "Switch role");
                    Target.role = otherRole;
                    Target.UpdateAllAvatar();
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
