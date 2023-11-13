using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Follower : MonoBehaviour
{

    public float speed = 5f;
    public float targetDistanceMax = 3f;
    float targetDistanceMin = 0.5f;
    new Rigidbody rigidbody;
    Animator animator;
    Transform target;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();


    }


    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Targeting();
        }
        TargetFollow();
        FlipSprite();
    }

    private void Targeting()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            Debug.Log("Player found!");
            target = players[0].transform;
        }
        else
        {
            Debug.Log("No player found!");
        }
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
            transform.localScale = new(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new(-1f, 1, 1);
        }
    }

}


