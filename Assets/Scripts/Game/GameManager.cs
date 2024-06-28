using System;
using System.Collections;
using System.Collections.Generic;
using Avatar;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const float SWITCH_COOLDOWN = 0.5f;
    float lastSwitchTime = 0;
    void DoSwitch()
    {
        if (Time.time - lastSwitchTime < SWITCH_COOLDOWN)
            return;

        var (leader, follower) = Avatar.Avatar.GetLeaderFollower();

        // Role switching
        leader.avatarRole = Avatar.Avatar.AvatarRole.Follower;
        follower.avatarRole = Avatar.Avatar.AvatarRole.Leader;

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
        Avatar.Avatar.UpdateAllAvatar();
    }

    void Update()
    {
        if (InputManager.Instance.TheSwitch())
        {
            DoSwitch();
        }
    }
}
