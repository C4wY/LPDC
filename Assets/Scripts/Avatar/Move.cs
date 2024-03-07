using UnityEngine;

namespace Avatar
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

        Avatar avatar;

        MoveParameters Parameters =>
            avatar.SafeParameters.move;

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

        public void HorizontalMoveUpdate(float inputX)
        {
            var x = inputX * Parameters.speed;
            var y = avatar.Rigidbody.velocity.y;
            avatar.Rigidbody.velocity = new(x, y, 0);
        }

        public void UpdateGroundPoint()
        {
            if (avatar.Ground.HasGroundPoint)
            {
                var (x, y, z) = avatar.Rigidbody.position;
                z = Mathf.Lerp(z, avatar.Ground.GroundPoint.z, 0.33f);
                avatar.Rigidbody.position = new(x, y, z);
            }
        }

        public float GoForegroundTime { get; private set; } = -1;

        public void GoForegroundUpdate()
        {
            var cooldown = Time.time < GoForegroundTime + Parameters.goBackwardCooldown;

            if (cooldown == false)
            {
                avatar.Ground.lockLayerIndex = -1;

                var wants = forceDown || Input.GetAxis("Vertical") < -0.1f;
                if (wants && avatar.Ground.TryGetReachableForegroundLayerIndex(out var layerIndex))
                {
                    GoForegroundTime = Time.time;
                    avatar.Ground.lockLayerIndex = layerIndex; // Lock the layer to avoid going foreground.
                }
            }
        }

        void OnEnable()
        {
            avatar = GetComponentInParent<Avatar>();
        }
    }
}