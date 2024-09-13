using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplodingFeedback : MonoBehaviour
{
    public GameObject explosionParts;
    public float explosionForce = 10f;

    void OnDestroy()
    {
        if (explosionParts != null)
        {
            foreach (Transform child in explosionParts.transform)
            {
                var part = Instantiate(child, child.position, child.rotation);
                part.localScale = child.localScale * explosionParts.transform.localScale.x;

                var rigibdody = part.AddComponent<Rigidbody>();
                rigibdody.AddForce(Random.onUnitSphere * Random.Range(0.5f, 1.5f) * explosionForce, ForceMode.Impulse);

                var collider = part.AddComponent<MeshCollider>();
                collider.convex = true;

                Destroy(part.gameObject, Random.Range(1f, 2f));
            }
        }
    }
}
