using UnityEngine;

namespace Avatar
{
    [CreateAssetMenu(fileName = "AvatarParameters", menuName = "LPDC/AvatarParameters", order = 1)]
    public class AvatarParameters : ScriptableObject
    {
        public GroundParameters ground = new();

        public SpriteHandlerParameters sprite = new();

        public MoveParameters move = new();

        public LeaderControllerParameters leaderController = new();

        public FollowerControllerParameters followerController = new();
    }
}
