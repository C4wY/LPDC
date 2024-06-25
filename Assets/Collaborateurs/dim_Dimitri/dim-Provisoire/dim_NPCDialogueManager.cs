using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.VersionControl;

public class dim_NPCDialogueManager : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.F; 
    private Story story;
    public TextAsset globalsInkJSON; // Le Json contenant les dialogues.
    private bool isDialogueActive = false;
    public TextMeshProUGUI dialogueText; // Le TMP de la boite de dialogue
    private bool dialogueTriggered = false;
    public const string DialogueStart = "Press F to talk";

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKey(activationKey))
        {
            if (dialogueTriggered == true)
            {
                StartDialogue();
                dialogueTriggered = false;
            }

            else
            {
                ContinueDialogue();

                if (story.currentChoices.Count != 0)
                {
                    //Debug.Log("Show Choices");
                    // StartCoroutine(ShowChoices());
                    // ShowChoices();
                }

            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        // Test : le collider correspond au tag recherché
        if (collider.gameObject.tag == "Player")
        {
            // Test : le collider est bien le leader
            var isLeader = Avatar.Avatar.GetLeader() == collider.GetComponentInParent<Avatar.Avatar>();
            if (isLeader)
            {
                dialogueText.text = DialogueStart;
                dialogueTriggered = true;
            }
        }


    }

    void StartDialogue()
    {
        // Initialisations
        story = new Story(globalsInkJSON.text);
        isDialogueActive = true;

        // On affiche la première ligne de dialogue
        if (story.canContinue)
        {
            StartCoroutine(DisplayNextLine());
        }
    }

    private void ContinueDialogue()
    {
        if (story.canContinue)
        {
            // On verifie les tags
            checkTags();

            // On lance l'affichage du texte
            StartCoroutine(DisplayNextLine());
        }
        else
        {
            // Si le dialogue est terminé, finito pipo
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
    }

    void checkTags()
    {
        // On récupère les tags
        var tags = story.currentTags;
        foreach (string t in tags)
        {
            // On sépare le préfix du paramètre (#color blue -> prefix = color, param = blue)
            string prefix = t.Split (' ')[0];
            string param = t.Split (' ')[1];

            switch (prefix.ToLower())
            {
                case "color":
                    // SetTextColor(param);
                    break;
                case "speaker":
                    // Guess who speaks
                    break;
                case "portrait":
                    break;
                case "layout":
                    break;

            }

        }
    }

    // private void SetTextColor(string param) => dialogueText.color = param;

    IEnumerator DisplayNextLine()
    {
        if (story.canContinue)
        {
            // On actualise le texte à afficher
            string text = story.Continue();
            dialogueText.text = "";

            // On affiche les lettres une par une
            foreach (char letter in text.ToCharArray())
            {
                dialogueText.text += letter;
                float timer = 0f;
                while (timer < 10f)
                {
                    timer += Time.deltaTime;
                    Debug.Log(timer);
                }

                yield return null;
            }
            
        }
        else
        {
            EndDialogue();
        }
    }

}
