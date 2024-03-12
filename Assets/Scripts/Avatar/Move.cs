using UnityEngine;

namespace Avatar
{
    [System.Serializable]
    public class MoveParameters
    {
        public float jumpHeight = 1.33f;
        public float jumpCooldown = 0.33f;
        public float groundVelocity = 5;

        [Tooltip("The time in seconds to wait before being able to go backward again (backward in Unity, is going foreground in a theater).")]
        public float goBackwardCooldown = 0.33f;
    }

    [ExecuteAlways]
    public class Move : MonoBehaviour
    {
        public bool forceDown;

        public float JumpTime { get; private set; } = -1;

        public bool IsJumping =>
            Time.time < JumpTime + Parameters.jumpCooldown;

        Avatar avatar;
        Avatar Avatar =>
            avatar != null ? avatar : avatar = GetComponent<Avatar>();

        MoveParameters Parameters =>
            Avatar.SafeParameters.move;

        /// <summary>
        /// Returns the velocity required to reach the jump height.
        /// </summary>
        public float GetJumpVelocityY()
        {
            return Mathf.Sqrt(Physics.gravity.magnitude * 2f * Parameters.jumpHeight);
        }

        void Jump()
        {
            var velocity = avatar.Rigidbody.velocity;
            velocity.y = GetJumpVelocityY();
            avatar.Rigidbody.velocity = velocity;

            JumpTime = Time.time;
        }

        public bool TryToJump()
        {
            if (avatar.Ground.IsGrounded && Time.time > JumpTime + Parameters.jumpCooldown)
            {
                Jump();
                return true;
            }

            return false;
        }

        public void HorizontalUpdate(float horizontalInput)
        {
            if (enabled == false)
                return;

            var x = horizontalInput * Parameters.groundVelocity;
            var y = avatar.Rigidbody.velocity.y;
            avatar.Rigidbody.velocity = new(x, y, 0);
        }

        public void UpdateZ()
        {
            if (avatar.Ground.HasGroundPoint)
            {
                var (x, y, _) = avatar.Rigidbody.position;
                var z = avatar.Ground.GroundPoint.z; // No lerp here since it's a 2D game.
                avatar.Rigidbody.position = new(x, y, z);
            }
        }

        public float GoForegroundTime { get; private set; } = -1;

        public void VerticalUpdate(float verticalInput)
        {
            var cooldown = Time.time < GoForegroundTime + Parameters.goBackwardCooldown;

            if (cooldown == false)
            {
                avatar.Ground.lockLayerIndex = -1;

                var wants = forceDown || verticalInput < -0.1f;
                if (wants && avatar.Ground.TryGetReachableForegroundLayerIndex(out var layerIndex))
                {
                    GoForegroundTime = Time.time;
                    avatar.Ground.lockLayerIndex = layerIndex; // Lock the layer to avoid going foreground.
                }
            }

            // Ignore all one-sided platforms when going down.
            avatar.OneSidedPlatformAgent.ignoreAll = verticalInput < -0.1f;
        }

        void OnEnable()
        {
            avatar = GetComponentInParent<Avatar>();
        }
    }
}