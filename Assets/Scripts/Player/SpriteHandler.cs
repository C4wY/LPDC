using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    Animator animator;
    new Rigidbody rigidbody;
    SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        var moveXInput = Input.GetAxis("Horizontal");
        animator.SetFloat("HSpeed", Mathf.Abs(moveXInput));

        var velocity = rigidbody.velocity;
        if (Mathf.Abs(velocity.x) > 0.1f)
            spriteRenderer.flipX = velocity.x < 0;
    }
}
