using UnityEngine;
using UnityEngine.Windows;

namespace Avatar
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

            if (!dialoguePause) // J'empêche le personnage de se déplacer pendant les dialogues (Dim)
            {
                var x = avatar.IsLeader
                 ? avatar.LeaderController.input.horizontal
                 : avatar.Rigidbody.velocity.x;
                animator.SetFloat("HSpeed", Mathf.Abs(x));
                animator.SetBool("IsGrounded", SpriteIsGrounded);

                spriteRenderer.flipX = !avatar.Move.IsFacingRight;
            }
            
        }

        // J'ajoute une fonction qui se déclenche quand un dialogue se déclenche, et une autre quand le dialogue se termine. (Dim)

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
