using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_UnjPiège : MonoBehaviour
{
    public int dégâts = 1;

    HashSet<LPDC.Avatar> avatarsEnContact = new();

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
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
            var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
            if (avatarsEnContact.Contains(avatar) == true)
            {
                avatarsEnContact.Remove(avatar);
            }
        }
    }
}