using UnityEngine;

namespace Player
{
    public class LeaderController : MonoBehaviour
    {
        Move move;

        void JumpUpdate()
        {
            if (Input.GetButtonDown("Jump"))
                move.TryToJump();
        }

        void OnEnable()
        {
            move = GetComponent<Move>();
        }

        void Update()
        {
            JumpUpdate();
            move.GoForegroundUpdate();
        }

        void FixedUpdate()
        {
            move.HorizontalMoveUpdate();
            move.UpdateGroundPoint();
        }
    }
}
