using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    public Transform target = null;
    public float chaseSpeed = 2;
    public float idleAmplitude = 0.25f;
    public float idleFrequence = 0.5f;
    public float chaseRange = 4f;

    Vector3 initialPosition;
    float idleTime = 0;
    new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        initialPosition = rigidbody.position;
    }

    void Idle()
    {
        idleTime += Time.fixedDeltaTime;

        var x = initialPosition.x;
        var y = initialPosition.y + idleAmplitude + Mathf.Sin(idleTime * 2 * Mathf.PI * idleFrequence);
        var z = initialPosition.z;

        rigidbody.position = new Vector3(x, y, z);
    }

    void Chase()
    {
        var p = target.position;
        p.z = transform.position.z;
        rigidbody.velocity = Vector3.zero;
        rigidbody.position = Vector3.MoveTowards(rigidbody.position, target.position, chaseSpeed * Time.fixedDeltaTime);
    }

    void CheckForTarget()
    {
        var leader = Avatar.Avatar.GetLeader();

        if (leader != null)
        {
            var d = leader.transform.position - transform.position;
            var distanceToLeader = d.magnitude;
            if (distanceToLeader < chaseRange)
            {
                target = leader.transform;
            }
            else
            {
                target = null;
            }
        }
    }

    void FixedUpdate()
    {
        CheckForTarget();

        if (target == null)
        {
            Idle();
        }
        else
        {
            Chase();
        }
    }
}
