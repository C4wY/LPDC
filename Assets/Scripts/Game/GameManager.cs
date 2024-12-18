using LPDC;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public struct DualPortraitEventProps
{
    public LPDC.Avatar.Name mainAvatar;
}

public class DualPortraitEvent : UnityEvent<DualPortraitEventProps> { }

[ExecuteAlways]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static GameManager GetInstanceOrThrow()
    {
        if (Instance == null)
            throw new System.Exception("GameManager instance is null! Please make sure GameManager is in the scene.");

        return Instance;
    }

    public static bool TryGetInstance(out GameManager instance)
    {
        instance = Instance;
        return instance != null;
    }

    public static void DispatchDualPortraitEvent(DualPortraitEventProps props)
    {
        GetInstanceOrThrow().dualPortraitEvent.Invoke(props);
    }

    public MainSettings mainSettings;

    public DualPortraitEvent dualPortraitEvent = new();

    const float SWITCH_COOLDOWN = 0.5f;
    float lastSwitchTime = 0;
    void DoSwitch()
    {
        if (Time.time - lastSwitchTime < SWITCH_COOLDOWN)
            return;

        var (leader, follower) = LPDC.Avatar.GetLeaderFollower();

        // Role switching
        leader.avatarRole = LPDC.Avatar.Role.Follower;
        follower.avatarRole = LPDC.Avatar.Role.Leader;

        var delta = leader.transform.position - follower.transform.position;
        const float SWITCH_DISTANCE = 5;
        if (delta.magnitude < SWITCH_DISTANCE && Mathf.Abs(delta.y) < 0.5f)
        {
            leader.Move.mode = MoveMode.Switching;
            leader.Move.switchSourcePosition = leader.transform.position;
            leader.Move.switchTargetPosition = follower.transform.position;

            follower.Move.mode = MoveMode.Switching;
            follower.Move.switchSourcePosition = follower.transform.position;
            follower.Move.switchTargetPosition = leader.transform.position;

        }

        lastSwitchTime = Time.time;
        LPDC.Avatar.UpdateAllAvatar();
    }

    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManager instances detected!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("GameManager is enabled!");

        if (mainSettings == null)
        {
            Debug.LogError("MainSettings is not set!");
            return;
        }
    }

    void Start()
    {
        Debug.Log("Game Started!");
        Debug.Log($"MainSettings.Instance: {MainSettings.Instance}");
    }

    void Update()
    {
#if UNITY_EDITOR
        // In the editor for safety reasons, we make sure that the roles and names are correct.
        var avatars = LPDC.Avatar.GetAllAvatars();
        if (avatars.Length == 2)
        {
            LPDC.Avatar.TryFixAvatarRolesAndNames();
        }
        else if (avatars.Length == 1)
        {
            avatars[0].avatarRole = LPDC.Avatar.Role.Leader;
        }
#endif

        if (InputManager.Instance.TheSwitch())
        {
            DoSwitch();
        }

        if (InputManager.Instance.DebugCheat())
        {
            DebugCheat.Toggle();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var target = (GameManager)base.target;

            if (GUILayout.Button("Switch Avatars"))
                target.DoSwitch();

            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Trigger Game Over!"))
                Instantiate(MainSettings.Instance.gameOverScreen);
        }
    }
#endif
}
