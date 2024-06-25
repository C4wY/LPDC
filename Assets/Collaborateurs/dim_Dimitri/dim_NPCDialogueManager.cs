using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using System;
using UnityEditor.Rendering;

public class dim_NPCDialogueManager : MonoBehaviour
{
    // Variables Publiques
    public KeyCode activationKey = KeyCode.F; // La touche pour activer les dialogues.
    public TextAsset globalsInkJSON; // Le Json contenant les dialogues.
    public string DialogueStart = "Press F to talk";
    public Canvas canvas; // Le canvas contenant les éléments d'UI.

    // Variables Privées
    private Story story;
    private TextMeshProUGUI dialogueText;
    private Image textPanel;
    private Array buttonArray;

    // Booléens de Contrôle
    private bool isDialogueActive = false;
    private bool dialogueTriggered = false;
    private bool isDialoguePlaying = false;
    
 

    private void Start()
    {
        // Initialisations
        
        dialogueText = canvas.GetComponentInChildren<TextMeshProUGUI>(); // On récupère la zone de texte dans le canvas.
        dialogueText.gameObject.SetActive(false);

        textPanel = canvas.GetComponentInChildren<Image>();
        textPanel.gameObject.SetActive(false);

        buttonArray = canvas.GetComponentsInChildren<Button>(); // On récupère les différents boutons dans le canvas.

        // On crée une boucle pour rattacher les boutons à leur fonction dédiée.
        int i = 0;
        foreach (Button button in buttonArray)
        {
            button.GetComponent<Button>().onClick.AddListener(() => { choiceButtonClick(i); });
            button.gameObject.SetActive(false);
            i += 1;
        }
    }


    void Update()
    {
        // On vérifie si le joueur appuie sur F et si le dialogue n'est pas déjà en train de se jouer.
        if ((Input.GetKey(activationKey)) && (!isDialoguePlaying)) 
        {
            if (dialogueTriggered == true) // Si le joueur se trouve dans la zone de détection du NPC, on active le dialogue.
            {
                StartDialogue();
                dialogueTriggered = false;
            }

            else // Si le dialogue a déjà été lancé, on avance dans le dialogue.
            {
                ContinueDialogue();

                if (story.currentChoices.Count != 0) // Si le joueur se trouve à un embranchement, on affiche les différents choix possibles.
                {
                    StartCoroutine(ShowChoices());
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
                if (!isDialogueActive)
                {
                    textPanel.gameObject.SetActive(true); 
                    dialogueText.gameObject.SetActive(true); 
                    dialogueText.text = DialogueStart;
                    dialogueTriggered = true;
                }
                    
            }
        }


    }

    void OnTriggerExit (Collider collider)
    {
        EndDialogue();
    }

    void StartDialogue()
    {
        // Initialisations
        story = new Story(globalsInkJSON.text);
        isDialogueActive = true;

        // On affiche la première ligne de dialogue
        if (story.canContinue)
        {
            ContinueDialogue();
        }
    }

    private void ContinueDialogue()
    {
        if (story.canContinue)
        {
            isDialoguePlaying = true;
                
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
        textPanel.gameObject.SetActive(false); 
        dialogueText.gameObject.SetActive(false); 
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
                yield return new WaitForSeconds(0.05f);
            }

            isDialoguePlaying = false;

        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator ShowChoices()
    {
        List<Choice> _choices = story.currentChoices;
        int i = 0;

        foreach (Button button in buttonArray)
        {
            if (i < _choices.Count)
            {
                button.gameObject.SetActive(true); 
                TextMeshProUGUI buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonText.text = _choices[i].text;
                yield return null;
            }

            i += 1;

        }

        
    }
    
    public void choiceButtonClick(int index)
    {
        int choiceSelected = index;
        story.ChooseChoiceIndex(choiceSelected);
        foreach (Button button in buttonArray)
        {
            button.gameObject.SetActive(false);
        }
        choiceSelected = 99;
        ContinueDialogue();
    }




}
