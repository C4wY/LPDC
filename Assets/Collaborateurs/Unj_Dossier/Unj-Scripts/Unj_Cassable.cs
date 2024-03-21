using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unj_Cassable : MonoBehaviour
{
    public GameObject cassé;
    public float breakForce;

    void Update()
    {
        if (Input.GetKeyDown("f"))
            BreakTheThing();

    }

    public void BreakTheThing()
    {
        GameObject frac = Instantiate(cassé, transform.position, transform.rotation);

        foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position).normalized * breakForce;
            rb.AddForce(force);
        }

        Destroy(gameObject);
    }
}




