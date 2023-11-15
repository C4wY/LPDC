using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Follower : MonoBehaviour
{

    public Transform target;
    public float speed = 5f;
    public float targetDistanceMax = 3f;

    float targetDistanceMin = 0.5f;
    new Rigidbody rigidbody;
    Animator animator;
    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        TargetFollow();
        FlipSprite();
    }


    void TargetFollow()
    {
        var velocity = rigidbody.velocity;
        animator.SetFloat("Speed", velocity.magnitude);
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) > targetDistanceMax)
            {
                velocity.x = target.position.x > transform.position.x ? speed : -speed;
                rigidbody.velocity = velocity;
            }
            else if (Vector3.Distance(transform.position, target.position) < targetDistanceMin)
            {
                velocity.x = target.position.x > transform.position.x ? -speed : speed;
                rigidbody.velocity = velocity;
            }

        }
    }

    void FlipSprite()
    {
        if (target.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

}


