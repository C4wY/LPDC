using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

namespace LPDC
{
    [System.Serializable]
    public class SpriteHandlerParameters
    {
        public float groundDistanceMax = 0.95f;

        [Tooltip("Time to wait after leaving the ground before setting the IsGrounded parameter to false.")]
        public float postLeaveGroundWaiting = 0.1f;
    }

    public class SpriteHandler : MonoBehaviour
    {
        public bool dialoguePause = false; // Pour la fonction de pause, ajouté par Dim
        bool dialoguePauseOld = false;

        Avatar avatar;
        Animator animator;
        SpriteRenderer spriteRenderer;

        SpriteHandlerParameters Parameters =>
            avatar.SafeParameters.sprite;

        bool realIsGrounded;
        float leaveGroundTime;

        /// <summary>
        /// Returns true if the sprite is grounded or if it left the ground recently (see postLeaveGroundWaiting parameter).
        /// </summary>
        public bool SpriteIsGrounded =>
            realIsGrounded || Time.time - leaveGroundTime < Parameters.postLeaveGroundWaiting;

        void OnEnable()
        {
            avatar = GetComponentInParent<Avatar>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void FixedUpdate()
        {
            var realIsGroundedNew = avatar.Ground.GroundDistance < Parameters.groundDistanceMax;
            if (realIsGroundedNew != realIsGrounded)
            {
                if (realIsGroundedNew == false)
                    leaveGroundTime = Time.time;

                realIsGrounded = realIsGroundedNew;
            }

            if (dialoguePause)
            {
                animator.SetFloat("HSpeed", 0);
                animator.SetBool("IsGrounded", true);
                animator.SetBool("IsDashing", false);
            }
            else
            {
                // Sinon, je le laisse se déplacer (Dim)

                var x = avatar.IsLeader
                 ? avatar.LeaderController.input.horizontal
                 : avatar.Rigidbody.velocity.x;

                animator.SetFloat("HSpeed", Mathf.Abs(x));
                animator.SetBool("IsGrounded", SpriteIsGrounded);
                animator.SetBool("IsDashing", avatar.Move.IsDashing);

                spriteRenderer.flipX = !avatar.Move.IsFacingRight;
            }

            dialoguePauseOld = dialoguePause;
        }

        // J'ajoute une fonction qui se d�clenche quand un dialogue se d�clenche, et une autre quand le dialogue se termine. (Dim)

        void OnPauseForDialogue()
        {
            dialoguePause = true;
        }

        void OffPauseForDialogue()
        {
            dialoguePause = false;
        }
    }
}
