using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            var avatar = other.attachedRigidbody.GetComponent<LPDC.Avatar>();
            if (avatar != null)
            {
                avatar.Santé.FaireDégâts(1);
                avatar.Move.TeleportTo(avatar.Ground.LastGroundPosition);
            }
        }
    }
}
