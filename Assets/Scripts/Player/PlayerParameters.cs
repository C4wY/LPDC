using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerParameters", menuName = "LPDC/PlayerParameters", order = 1)]
    public class PlayerParameters : ScriptableObject
    {
        public GroundParameters ground = new();

        public SpriteHandlerParameters sprite = new();

        public MoveParameters move = new();

        public LeaderControllerParameters leaderController = new();
    }
}
