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

        Vector3 newVelocity = new(0, rigidbody.velocity.y, 0);
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        rigidbody.velocity = newVelocity;
    }
}
