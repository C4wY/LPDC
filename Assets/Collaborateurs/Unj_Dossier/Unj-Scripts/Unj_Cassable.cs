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
            LPDC.Move ausecours = other.rigidbody.GetComponent<LPDC.Move>();

            if (ausecours.IsDashing)
            {
                Debug.Log("Collision avec le joueur, explosion déclenchée");
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

            var tempsDespawnCubes = UnityEngine.Random.Range(0.5f, 4f);
            StartCoroutine(ScaleDownAndDestroy(rb.gameObject, tempsDespawnCubes));
        }

        // Destroy(gameObject); // Casser tronc 

        var components = GetComponents<Component>();
        foreach (var component in components)
        {
            if (component is Unj_Cassable)
                continue;

            Destroy(component);
        }
        Destroy(gameObject, 5f);
    }

    private IEnumerator ScaleDownAndDestroy(GameObject fragment, float delay)
    {
        yield return new WaitForSeconds(delay);

        float scaleDuration = 1f;
        float elapsed = 0f;

        Vector3 initialScale = fragment.transform.localScale;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;

            float scale = Mathf.Lerp(1f, 0f, elapsed / scaleDuration);
            fragment.transform.localScale = initialScale * scale;

            yield return null;
        }

        Destroy(fragment); // Casser fragments
    }
}





