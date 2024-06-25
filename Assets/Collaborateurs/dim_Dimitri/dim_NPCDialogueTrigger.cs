using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;
using Ink.Parsed;

public class dim_NPCDialogueTrigger : MonoBehaviour
{
    private const string DialogueStart = "Press F to talk";
    private const string NoDialogue = "Ca fonctionne ?";
    public TMP_Text hintText;
    dim_DialogueManager dialogueManager;
    private bool dialogueTriggered = false;

    // On crée une variable qui redirige vers le dialogue du NPC concerné
    public float dialogueKey;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueTriggered == true)
            if (Input.GetKey(dialogueManager.activationKey))
            {
                dialogueManager.StartDialogue(Key);
                dialogueTriggered = false;
}

    }


    void OnTriggerEnter(Collider collider)
    {
        
        // Test : le collider correspond au tag recherché
        if (collider.gameObject.tag == "Player")
        {
            // Test : le colliderest bien le leader
            var isLeader = Avatar.Avatar.GetLeader() == collider.GetComponentInParent<Avatar.Avatar>();
            if (isLeader)
            {
                // On récupère le script DialogueManager
                hintText.text = DialogueStart;
                dialogueManager = collider.GetComponentInParent<dim_DialogueManager>();
                if (dialogueManager != null)
                    Debug.Log("DialogueManagerOK");
                dialogueTriggered = true;
            }
        }
            
        
    }

    void OnTriggerExit(Collider collider)
    {
        hintText.text = NoDialogue;
        dialogueTriggered = false;
    }
}
