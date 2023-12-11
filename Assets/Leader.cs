using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Leader : MonoBehaviour
{
    public enum MoveMode
    {
        FreeMove,
        NoFreeMove,
    }

    public MoveMode moveMode = MoveMode.FreeMove;

    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    public float jumpForce = 8f;

    public LayerMask whatIsGround;

    public bool isGrounded;

    new Rigidbody rigidbody;
    Animator animator;
    Transform groundCheckPoint;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        groundCheckPoint = transform.Find("GroundCheckPoint");
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void FreeMoveUpdate()
    {
        Vector3 newVelocity = new(0, rigidbody.velocity.y, 0);

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        newVelocity.z = Input.GetAxis("Vertical") * speed;
        rigidbody.velocity = newVelocity;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidbody.velocity += new Vector3(0f, jumpForce, 0f);
        }

        animator.SetFloat("Speed", newVelocity.magnitude);

        if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out var _, .3f, whatIsGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        animator.SetBool("OnGround", isGrounded);

        if (Mathf.Abs(newVelocity.x) > 0.1f)
        {
            spriteRenderer.flipX = newVelocity.x < 0;
        }
    }

    void Update()
    {
        if (moveMode == MoveMode.FreeMove)
            FreeMoveUpdate();
    }
}
