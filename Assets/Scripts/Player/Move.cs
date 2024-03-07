using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class MoveParameters
    {
        public float jumpHeight = 1.33f;
        public float jumpCooldown = 0.33f;
        public float speed = 5;

        [Tooltip("The time in seconds to wait before being able to go backward again (backward in Unity, is going foreground in a theater).")]
        public float goBackwardCooldown = 0.33f;
    }

    [ExecuteAlways]
    public class Move : MonoBehaviour
    {
        public bool forceDown;

        public float JumpTime { get; private set; } = -1;

        Player player;

        MoveParameters Parameters =>
            player.SafeParameters.move;

        /// <summary>
        /// Returns the velocity required to reach the jump height.
        /// </summary>
        public float GetJumpVelocityY()
        {
            return Mathf.Sqrt(Physics.gravity.magnitude * 2f * Parameters.jumpHeight);
        }

        void Jump()
        {
            var velocity = player.Rigidbody.velocity;
            velocity.y = GetJumpVelocityY();
            player.Rigidbody.velocity = velocity;

            JumpTime = Time.time;
        }

        public bool TryToJump()
        {
            if (player.Ground.IsGrounded && Time.time > JumpTime + Parameters.jumpCooldown)
            {
                Jump();
                return true;
            }

            return false;
        }

        public void HorizontalMoveUpdate(float inputX)
        {
            var x = inputX * Parameters.speed;
            var y = player.Rigidbody.velocity.y;
            player.Rigidbody.velocity = new(x, y, 0);
        }

        public void UpdateGroundPoint()
        {
            if (player.Ground.HasGroundPoint)
            {
                var (x, y, z) = player.Rigidbody.position;
                z = Mathf.Lerp(z, player.Ground.GroundPoint.z, 0.33f);
                player.Rigidbody.position = new(x, y, z);
            }
        }

        public float GoForegroundTime { get; private set; } = -1;

        public void GoForegroundUpdate()
        {
            var cooldown = Time.time < GoForegroundTime + Parameters.goBackwardCooldown;

            if (cooldown == false)
            {
                player.Ground.lockLayerIndex = -1;

                var wants = forceDown || Input.GetAxis("Vertical") < -0.1f;
                if (wants && player.Ground.TryGetReachableForegroundLayerIndex(out var layerIndex))
                {
                    GoForegroundTime = Time.time;
                    player.Ground.lockLayerIndex = layerIndex; // Lock the layer to avoid going foreground.
                }
            }
        }

        void OnEnable()
        {
            player = GetComponentInParent<Player>();
        }
    }
}