using LPDC;
using UnityEngine;

[ExecuteAlways]
public class GameManager : MonoBehaviour
{
    public Animator portraitAnimator;

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

        //update UI
        if (portraitAnimator != null)
        {
            var trigger = leader.avatarName == LPDC.Avatar.Name.Sora ? "Sora > Dooms" : "Dooms > Sora";
            portraitAnimator.SetTrigger(trigger);
        }
    }

    void Start()
    {
        if (portraitAnimator != null)
        {
            var leader = LPDC.Avatar.GetLeader();
            var soraIsLeader = leader.avatarName == LPDC.Avatar.Name.Sora;
            portraitAnimator.SetBool("Sora Is Leader On Start", soraIsLeader);
        }
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
    }
}
