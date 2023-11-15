using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Turrel : MonoBehaviour
{
    public float cooldown = 2f;
    public GameObject projectileGO;
    public float projectileSpeed = 4;

    bool isActive = false;
    float time = 0;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
            TurrelUpdate();
    }

    void TurrelUpdate()
    {
        time += -Time.deltaTime;

        if (time <= 0f)
            Fire();
    }

    void Fire()
    {
        time += cooldown;
        var projectile = GameObject.Instantiate(projectileGO, transform.position, Quaternion.identity);
        var rb = projectile.GetComponent<Rigidbody>();
        var direction = (target.position - transform.position).normalized;
        var velocity = direction * projectileSpeed;
        rb.velocity = velocity;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            isActive = true;
            target = collider.transform;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            isActive = false;
            target = null;
        }
    }
}
