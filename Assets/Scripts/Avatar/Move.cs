using System;
using TMPro;
using UnityEngine;

namespace Avatar
{
    [System.Serializable]
    public class MoveParameters
    {
        public float jumpHeight = 2.33f;
        public float jumpCooldown = 0.33f;
        public float jumpHeightRatioWhileDashing = 2;
        public float walkVelocity = 3.5f;
        public float runVelocity = 7;

        [Tooltip("The time in seconds to wait before being able to dash again.")]
        public float dashDuration = 0.15f;
        public float dashCooldown = 0.58f;
        public float dashVelocity = 30;

        public float switchVelocity = 15;

        [Tooltip("The time in seconds to wait before being able to go backward again (backward in Unity, is going foreground in a theater).")]
        public float goBackwardCooldown = 0.33f;
    }

    public enum MoveMode
    {
        Normal,
        Switching,
    }

    public enum MoveFacing
    {
        Left,
        Right,
    }

    [ExecuteAlways]
    public class Move : MonoBehaviour
    {
        public bool forceDown;

        public float JumpTime { get; private set; } = -1;
        public Vector3 JumpVelocityAtJumpTime { get; private set; }
        public bool IsJumping =>
            Time.time < JumpTime + Parameters.jumpCooldown;

        public float DashTime { get; private set; } = -1;
        public float DashDirection { get; private set; } = 1;
        public bool IsDashing =>
            Time.time < DashTime + Parameters.dashDuration;

        public MoveFacing facing = MoveFacing.Right;
        public bool IsFacingRight => facing == MoveFacing.Right;

        public MoveMode mode = MoveMode.Normal;

        public Vector3 switchSourcePosition;
        public Vector3 switchTargetPosition;

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
            var h = Parameters.jumpHeight;
            var g = Physics.gravity.magnitude;
            // https://calculis.net/chute-libre#:~:text=La%20formule%20pour%20calculer%20la,%2Fs%20ou%20m.s%2D1.&text=v%20est%20la%20vitesse%20th%C3%A9orique,seconde%20(m%2Fs).

            if (IsDashing)
                h *= Parameters.jumpHeightRatioWhileDashing;

            return Mathf.Sqrt(g * 2f * h);
        }

        public void TeleportTo(Vector3 position)
        {
            avatar.Rigidbody.position = position;
            avatar.Rigidbody.velocity = Vector3.zero;
        }

        /// <summary>
        /// Warning: this method does not check if the avatar is grounded and will
        /// perform a jump even if the avatar is in the air.
        /// <br/><br/>
        /// For a grounded jump, use TryToJump() instead. 
        /// </summary>
        public void Jump()
        {
            var velocity = avatar.Rigidbody.velocity;
            velocity.y = GetJumpVelocityY();
            avatar.Rigidbody.velocity = velocity;
            JumpVelocityAtJumpTime = velocity;

            JumpTime = Time.time;
        }

        public bool CanJump()
        {
            return avatar.Ground.IsGrounded && Time.time > JumpTime + Parameters.jumpCooldown;
        }

        public bool TryToJump()
        {
            if (CanJump())
            {
                Jump();
                return true;
            }

            return false;
        }

        void Dash()
        {
            DashTime = Time.time;
            DashDirection = facing == MoveFacing.Left
                ? -1
                : 1;

            Avatar.Santé.compteurInvincibilité = 0.15f;
        }

        public bool TryToDash()
        {
            var cooldownOk = Time.time > DashTime + Parameters.dashCooldown;
            if (cooldownOk)
            {
                Dash();
                return true;
            }

            return false;
        }

        public void HorizontalUpdate(float horizontalInput)
        {
            if (enabled == false)
                return;

            if (mode == MoveMode.Switching)
            {
                // Switching mode.
                var dx = switchTargetPosition.x - switchSourcePosition.x;
                var x = Mathf.Sign(dx) * Parameters.switchVelocity;

                var isTooFarOnRight = dx > 0 && transform.position.x > switchTargetPosition.x;
                var isTooFarOnLeft = dx < 0 && transform.position.x < switchTargetPosition.x;
                if (isTooFarOnRight || isTooFarOnLeft)
                {
                    // Arrived at the target position.
                    avatar.Rigidbody.position = switchTargetPosition;
                    avatar.Rigidbody.velocity = Vector3.zero;
                    mode = MoveMode.Normal;
                }
                else
                {
                    // Dashing to the target position.
                    var y = avatar.Rigidbody.velocity.y;
                    avatar.Rigidbody.velocity = new(x, y, 0);
                }
            }
            else
            {
                // Normal mode.
                var x = IsDashing
                    ? DashDirection * Parameters.dashVelocity
                    : horizontalInput * Parameters.runVelocity;

                var y = avatar.Rigidbody.velocity.y;
                avatar.Rigidbody.velocity = new(x, y, 0);

                if (horizontalInput > 0.1f)
                    facing = MoveFacing.Right;
                else if (horizontalInput < -0.1f)
                    facing = MoveFacing.Left;
            }
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