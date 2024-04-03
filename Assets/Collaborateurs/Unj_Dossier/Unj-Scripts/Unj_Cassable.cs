using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unj_Cassable : MonoBehaviour
{
    public GameObject cassé;
    public float explosionForce;

    void Update()
    {
        if (Input.GetKeyDown("e"))
            BreakTheThing();

    }

    public void BreakTheThing()
    {
        GameObject frac = Instantiate(cassé, transform.position, transform.rotation);

        foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position).normalized * explosionForce;
            rb.AddForce(force);
        }

        Destroy(gameObject);
    }
}




