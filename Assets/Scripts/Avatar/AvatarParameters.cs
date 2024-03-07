using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Avatar
{
    [CreateAssetMenu(fileName = "PlayerParameters", menuName = "LPDC/PlayerParameters", order = 1)]
    public class AvatarParameters : ScriptableObject
    {
        public GroundParameters ground = new();

        public SpriteHandlerParameters sprite = new();

        public MoveParameters move = new();

        public LeaderControllerParameters leaderController = new();
    }
}
