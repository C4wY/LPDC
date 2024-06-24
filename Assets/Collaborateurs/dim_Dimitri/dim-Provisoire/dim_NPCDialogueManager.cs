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
    private Story story;
    public TextAsset globalsInkJSON; // Le Json contenant les dialogues.
    private bool isDialogueActive = false;
    public TextMeshProUGUI dialogueText; // Le TMP de la boite de dialogue

    void Start()
    {

    }


    void Update()
    {

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

    private void EndDialogue()
    {
        isDialogueActive = false;
    }

    private void checkTags()
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

    private IEnumerator DisplayNextLine()
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
                yield return null;
            }
            
        }
        else
        {
            EndDialogue();
        }
    }

}
