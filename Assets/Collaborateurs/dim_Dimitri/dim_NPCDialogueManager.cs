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
    public Canvas canvas; // Le canvas contenant les �l�ments d'UI.

    // Variables Priv�es
    private Story story;
    private TextMeshProUGUI dialogueText;
    private Image textPanel;
    private Array buttonArray;

    // Bool�ens de Contr�le
    private bool isDialogueActive = false;
    private bool dialogueTriggered = false;
    private bool isDialoguePlaying = false;
    
 

    private void Start()
    {
        // Initialisations
        
        dialogueText = canvas.GetComponentInChildren<TextMeshProUGUI>(); // On r�cup�re la zone de texte dans le canvas.
        dialogueText.gameObject.SetActive(false);

        textPanel = canvas.GetComponentInChildren<Image>();
        textPanel.gameObject.SetActive(false);

        buttonArray = canvas.GetComponentsInChildren<Button>(); // On r�cup�re les diff�rents boutons dans le canvas.

        // On cr�e une boucle pour rattacher les boutons � leur fonction d�di�e.
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
        // On v�rifie si le joueur appuie sur F et si le dialogue n'est pas d�j� en train de se jouer.
        if ((Input.GetKey(activationKey)) && (!isDialoguePlaying)) 
        {
            if (dialogueTriggered == true) // Si le joueur se trouve dans la zone de d�tection du NPC, on active le dialogue.
            {
                StartDialogue();
                dialogueTriggered = false;
            }

            else // Si le dialogue a d�j� �t� lanc�, on avance dans le dialogue.
            {
                ContinueDialogue();

                if (story.currentChoices.Count != 0) // Si le joueur se trouve � un embranchement, on affiche les diff�rents choix possibles.
                {
                    StartCoroutine(ShowChoices());
                }

            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        // Test : le collider correspond au tag recherch�
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

        // On affiche la premi�re ligne de dialogue
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
            // Si le dialogue est termin�, finito pipo
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
        // On r�cup�re les tags
        var tags = story.currentTags;
        foreach (string t in tags)
        {
            // On s�pare le pr�fix du param�tre (#color blue -> prefix = color, param = blue)
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
            // On actualise le texte � afficher
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
