using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unj_Cassable : MonoBehaviour
{
    public GameObject préfabSuperCassé;
    public float explosionForce = 2000;
    public float décalage = 1.5f;

    void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody.gameObject.CompareTag("Player"))
        {
            Avatar.Move ausecours = other.rigidbody.GetComponent<Avatar.Move>();

            if (ausecours.IsDashing)
            {
                Explose(ausecours.transform.position + Vector3.down * décalage);
            }
        }

    }


    public void Explose(Vector3 centreExplosion)
    {
        GameObject instanceBlocCassé = Instantiate(préfabSuperCassé, transform.position, transform.rotation);

        foreach (Rigidbody rb in instanceBlocCassé.GetComponentsInChildren<Rigidbody>())
        {
            var direction2D = rb.transform.position - centreExplosion;
            direction2D.z = 0;
            direction2D = direction2D.normalized;

            Vector3 force = direction2D * explosionForce;
            rb.AddForce(force);
            var tempsDespawnCubes = UnityEngine.Random.Range(2, 4);
            Destroy(rb.gameObject, tempsDespawnCubes);

        }

        Destroy(gameObject);
    }
}




