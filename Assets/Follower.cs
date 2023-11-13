using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Follower : MonoBehaviour
{

    public float speed = 5f;
    public float targetDistance = 3f;
    public Transform PlayerTransform;
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
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    if (players.Length > 0) 
    {
        Debug.Log("Player found!");
        PlayerTransform = players[0].transform;
        target = PlayerTransform;
    }
    else
    {
        Debug.Log("No player found!");
    }
        TargetFollow();
        FlipSprite();
    }


    void TargetFollow()
{
    if (PlayerTransform != null)
    {
        if (Vector3.Distance(transform.position, PlayerTransform.position) > targetDistance)
        {
            animator.SetBool("Run", true);
            transform.position = Vector3.MoveTowards(transform.position, PlayerTransform.position, speed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Run", false);
        }
        Debug.Log("Prout");
    }
}

    void FlipSprite()
    {
        if (PlayerTransform.position.x > transform.position.x)
        {
            transform.localScale = new(1f,1f,1f);
        }
        else
        {
            transform.localScale = new(-1f,1,1);
        }
    }

}


