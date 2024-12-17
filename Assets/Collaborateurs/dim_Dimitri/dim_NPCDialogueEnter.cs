using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dim_NPCDialogueEnter : MonoBehaviour
{
    public bool triggerOnlyOnce = true;
    public TextAsset inkDialogue;

    int triggerCount = 0;

    void OnTriggerEnter(Collider collider)
    {
        if (triggerOnlyOnce && triggerCount > 0)
            return;

        // Test : le collider correspond au tag recherché
        if (collider.gameObject.CompareTag("Player"))
        {
            // Test : le colliderest bien le leader
            var isLeader = LPDC.Avatar.GetLeader() == collider.GetComponentInParent<LPDC.Avatar>();
            if (isLeader)
            {
                var dialogueManager = FindFirstObjectByType<dim_NPCDialogueManager>();
                if (dialogueManager != null)
                {
                    dialogueManager.EnterDialogue(inkDialogue);
                    triggerCount++;
                }
            }
        }
    }
}
