using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    public enum State
    {
        InitialIdle,
        Chase,
        Destroyed,
    }


    public float chaseSpeed = 2;
    public float idleAmplitude = 0.25f;
    public float idleFrequence = 0.5f;
    public float chaseRange = 4f;
    public int damage = 1;
    public float destroyTimer = 3f;

    State state = State.InitialIdle;
    Transform target = null;
    Vector3 initialPosition;
    float idleTime = 0;
    new Rigidbody rigidbody;

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
        rigidbody.velocity = (p - rigidbody.position).normalized * chaseSpeed;
    }

    void CheckForTarget()
    {
        var leader = LPDC.Avatar.GetLeader();

        if (leader != null)
        {
            var d = leader.transform.position - transform.position;
            var distanceToLeader = d.magnitude;
            if (distanceToLeader < chaseRange)
            {
                target = leader.transform;
                state = State.Chase;
            }
        }
    }

    void TrySelfDestroy(LPDC.Avatar avatar)
    {
        if (state == State.Destroyed)
            return;

        state = State.Destroyed;
        rigidbody.useGravity = true;

        if (avatar != null)
        {
            avatar.Santé.FaireDégâts(damage);
        }

        Destroy(gameObject, destroyTimer);
    }


    // Unity Message:

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        initialPosition = rigidbody.position;
    }

    void FixedUpdate()
    {

        switch (state)
        {
            case State.InitialIdle:
                Idle();
                CheckForTarget();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var avatar = other.GetComponent<LPDC.Avatar>();
        TrySelfDestroy(avatar);
    }
}
