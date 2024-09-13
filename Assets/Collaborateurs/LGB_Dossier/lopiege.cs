using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lopiege : MonoBehaviour
{
    public int dégâts = 1;
    public GameObject spikePrefab; // Variable to hold the spike prefab

    HashSet<LPDC.Avatar> avatarsEnContact = new HashSet<LPDC.Avatar>(); // Fixed initialization

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
            if (!avatarsEnContact.Contains(avatar)) // Simplified condition
            {
                avatar.Santé.FaireDégâts(dégâts);
                avatarsEnContact.Add(avatar);

                // Instantiate the spike prefab at the position of the trigger
                Destroy(Instantiate(spikePrefab, transform.position, Quaternion.identity), 2f); // 2 seconds
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
            if (avatarsEnContact.Contains(avatar))
            {
                avatarsEnContact.Remove(avatar);

            }
        }
    }
}
