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
                if (avatar.IsLeader)
                    avatar.Santé.FaireDégâts(1);

                // Teleport the avatar to a safe location.
                var position = avatar.Ground.GroundPath.FromEnd(0.5f).position;
                avatar.Move.TeleportTo(position);
            }
        }
    }
}
