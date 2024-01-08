using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine;

namespace Fossils
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody rb;
        public Transform head;


        [Header("Config")]

        public float walkSpeed;
        public float runSpeed;
        public float jumpForce;

        public LayerMask whatIsGround;
        public Transform groundPoint;
        private bool isGrounded;

        public Animator anim;

        public SpriteRenderer theSR;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            Vector3 newVelocity = Vector3.up * rb.velocity.y;
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            newVelocity.x = Input.GetAxis("Horizontal") * speed;
            newVelocity.z = Input.GetAxis("Vertical") * speed;
            rb.velocity = newVelocity;

            rb.velocity = new Vector3(newVelocity.x * walkSpeed, rb.velocity.z, newVelocity.z * walkSpeed);

            anim.SetFloat("Speed", rb.velocity.magnitude);

            if (Physics.Raycast(groundPoint.position, Vector3.down, out var hit, .3f, whatIsGround))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity += new Vector3(0f, jumpForce, 0f);
            }

            anim.SetBool("onGround", isGrounded);

            if (theSR.flipX && newVelocity.x < 0)
            {
                theSR.flipX = true;
            }
            else if (theSR.flipX && newVelocity.x > 0)
            {
                theSR.flipX = false;
            }
        }
    }
}
