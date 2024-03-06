using UnityEngine;

namespace Player
{
    public class SpriteHandler : MonoBehaviour
    {
        Player player;
        Animator animator;
        new Rigidbody rigidbody;
        SpriteRenderer spriteRenderer;

        void OnEnable()
        {
            player = GetComponentInParent<Player>();
            rigidbody = GetComponentInParent<Rigidbody>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void FixedUpdate()
        {
            var velocity = rigidbody.velocity;

            var x = player.IsLeader ? Input.GetAxis("Horizontal") : velocity.x;
            animator.SetFloat("HSpeed", Mathf.Abs(x));

            if (Mathf.Abs(velocity.x) > 0.1f)
                spriteRenderer.flipX = velocity.x < 0;
        }
    }
}
