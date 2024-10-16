using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public enum State
    {
        IdleMovement,
        BackToIdle,
        Chase,
        Destroyed,
    }

    public float idleSpeed = 3f;
    public int attackDamage = 1;
    public float stunTime = 3f;
    public int hitPoints = 1;
    public float chaseSpeed = 5f;
    public float chaseRange = 10f;
    public float attackCoolDown = 5f;
    public float attackRange = 5f;
    public float idleDistanceThreshold = 0.05f;
    public float patrolRange = 5f;

    public State state = State.IdleMovement;

    Transform target = null;
    Vector3 initialPosition;
    new Rigidbody rigidbody;
    float idleTime = 0;

    void Start()
    {
        initialPosition = transform.position;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckForTarget();

        switch (state)
        {
            case State.IdleMovement:
                IdleMovement();
                break;
            case State.Chase:
                Chase();
                break;
            case State.BackToIdle:
                BackToIdleMovement();
                break;
        }
    }

    void BackToIdleMovement()
    {
        var delta = initialPosition - transform.position;
        var distance = delta.magnitude;

        if (distance < idleDistanceThreshold)
        {
            state = State.IdleMovement;
            idleTime = 0;
        }
        else
        {
            var direction = delta / distance;
            rigidbody.MovePosition(rigidbody.position + direction * idleSpeed * Time.deltaTime);
        }
    }

    void IdleMovement()
    {
        float patrolX = Mathf.PingPong(idleTime * idleSpeed, patrolRange);
        rigidbody.MovePosition(new Vector3(initialPosition.x + patrolX, initialPosition.y, initialPosition.z));

        idleTime += Time.deltaTime;
    }

    void Chase()
    {
        if (target != null)
        {

            Vector3 direction = (target.position - transform.position).normalized;
            direction.z = 0;


            Vector3 newPosition = rigidbody.position + direction * chaseSpeed * Time.deltaTime;
            rigidbody.MovePosition(newPosition);
        }
    }

    void CheckForTarget()
    {

        var leader = LPDC.Avatar.GetLeader();

        if (leader != null)
        {

            float distanceToLeader = Vector3.Distance(transform.position, leader.transform.position);


            if (distanceToLeader < chaseRange)
            {
                target = leader.transform;
                state = State.Chase;
            }

            else if (state == State.Chase && distanceToLeader >= chaseRange)
            {
                state = State.BackToIdle;
                target = null;
            }
        }
    }
    void OnDrawGizmos()
    {
        var startPosition = Application.isPlaying ? initialPosition : transform.position;
        var finalPosition = startPosition + Vector3.right * patrolRange;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(finalPosition, 0.1f);
        Gizmos.DrawLine(startPosition, finalPosition);
    }
}