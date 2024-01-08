using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 1.33f;
    public float speed = 5;

    public float GetJumpVelocityY()
    {
        return Mathf.Sqrt(Physics.gravity.magnitude * 2f * jumpHeight);
    }

    void Update()
    {
        var rigidbody = GetComponent<Rigidbody>();

        if (Input.GetButtonDown("Jump"))
        {
            var velocity = rigidbody.velocity;
            velocity.y = GetJumpVelocityY();
            rigidbody.velocity = velocity;
        }
    }

    void FixedUpdate()
    {
        var rigidbody = GetComponent<Rigidbody>();

        var velocityX = Input.GetAxis("Horizontal") * speed;
        rigidbody.velocity = new(
            velocityX,
            rigidbody.velocity.y,
            0);

        var ground = GetComponent<Ground>();
        if (ground.HasGroundPoint(out var groundPoint))
        {
            var (x, y, z) = rigidbody.position;
            z = Mathf.Lerp(z, groundPoint.z, 0.33f);
            rigidbody.position = new(x, y, z);
        }
    }
}
