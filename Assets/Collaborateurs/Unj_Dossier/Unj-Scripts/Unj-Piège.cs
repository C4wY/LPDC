using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnjPiège : MonoBehaviour
{
    public int dégâts = 1;

    HashSet<Avatar.Avatar> avatarsEnContact = new();

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<Avatar.Avatar>();
            if (avatarsEnContact.Contains(avatar) == false)
            {
                avatar.Santé.FaireDégâts(dégâts);
                avatarsEnContact.Add(avatar);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<Avatar.Avatar>();
            if (avatarsEnContact.Contains(avatar) == true)
            {
                avatarsEnContact.Remove(avatar);
            }
        }
    }
}