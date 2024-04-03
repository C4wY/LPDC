using System.Collections;
using System.Collections.Generic;
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

        public float DashTime { get; private set; } = -1;
        public bool IsDashing =>
            Time.time < DashTime + Parameters.dashDuration;

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

        void Dash()
        {
            DashTime = Time.time;
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

            var x = IsDashing
                ? horizontalInput * Parameters.dashVelocity
                : horizontalInput * Parameters.runVelocity;
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
    
        public int PV = 3;

        public void FaireDégâts(int dégâts = 1)
        {
            PV += -dégâts;

            if (PV == 0)
            {
                Debug.Log("Jeu fini");
                Debug.Break();
            }

            if (IsDashing)
            {
                dégâts = 0
            }
        }
    }
}