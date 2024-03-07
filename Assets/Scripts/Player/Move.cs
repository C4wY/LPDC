using UnityEngine;

namespace Player
{
    public class Move : MonoBehaviour
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

        public MoveParameters parameters = new();

        new Rigidbody rigidbody;
        Ground ground;

        public bool forceDown;

        public float JumpTime { get; private set; } = -1;

        /// <summary>
        /// Returns the velocity required to reach the jump height.
        /// </summary>
        public float GetJumpVelocityY()
        {
            return Mathf.Sqrt(Physics.gravity.magnitude * 2f * parameters.jumpHeight);
        }

        void Jump()
        {
            var velocity = rigidbody.velocity;
            velocity.y = GetJumpVelocityY();
            rigidbody.velocity = velocity;

            JumpTime = Time.time;
        }

        public bool TryToJump()
        {
            if (ground.IsGrounded && Time.time > JumpTime + parameters.jumpCooldown)
            {
                Jump();
                return true;
            }

            return false;
        }

        public void HorizontalMoveUpdate(float inputX)
        {
            var x = inputX * parameters.speed;
            var y = rigidbody.velocity.y;
            rigidbody.velocity = new(x, y, 0);
        }

        public void UpdateGroundPoint()
        {
            if (ground.HasGroundPoint)
            {
                var (x, y, z) = rigidbody.position;
                z = Mathf.Lerp(z, ground.GroundPoint.z, 0.33f);
                rigidbody.position = new(x, y, z);
            }
        }

        public float GoForegroundTime { get; private set; } = -1;

        public void GoForegroundUpdate()
        {
            var cooldown = Time.time < GoForegroundTime + parameters.goBackwardCooldown;

            if (cooldown == false)
            {
                ground.lockLayerIndex = -1;

                var wants = forceDown || Input.GetAxis("Vertical") < -0.1f;
                if (wants && ground.TryGetReachableForegroundLayerIndex(out var layerIndex))
                {
                    GoForegroundTime = Time.time;
                    ground.lockLayerIndex = layerIndex; // Lock the layer to avoid going foreground.
                }
            }
        }

        void OnEnable()
        {
            rigidbody = GetComponent<Rigidbody>();
            ground = GetComponent<Ground>();
        }
    }
}