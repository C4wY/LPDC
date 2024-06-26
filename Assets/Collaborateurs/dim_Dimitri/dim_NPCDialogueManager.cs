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
    private GameObject textPanelNPC;
    private GameObject textPanelDooms;
    private GameObject textPanelSora;
    private GameObject playersIcon;
    private GameObject npcIcon;
    private Array buttonArray;
    private List<Color> buttonColorList;
    private string displayText;
    private float updateCooldown;
    private bool skip = false;

    // Bool�ens de Contr�le
    private bool isDialogueActive = false;
    private bool dialogueTriggered = false;
    private bool isDialoguePlaying = false;
    
 

    private void Start()
    {
        // Initialisations
        
        dialogueText = canvas.GetComponentInChildren<TextMeshProUGUI>(); // On r�cup�re la zone de texte dans le canvas.
        dialogueText.gameObject.SetActive(false);

        textPanelNPC = GameObject.Find("TextPanelNPC");
        textPanelNPC.gameObject.SetActive(false);
        textPanelDooms = GameObject.Find("TextPanelDooms");
        textPanelDooms.gameObject.SetActive(false);
        textPanelSora = GameObject.Find("TextPanelSora");
        textPanelSora.gameObject.SetActive(false);

        playersIcon = GameObject.Find("PlayersIcon");
        playersIcon.gameObject.SetActive(false);
        npcIcon = GameObject.Find("NPC_Icon");
        npcIcon.gameObject.SetActive(false);

        buttonColorList = new List<Color>();
        for (int j = 0; j < 4; j++)
            buttonColorList.Add(Color.white);

        buttonArray = canvas.GetComponentsInChildren<Button>(); // On r�cup�re les diff�rents boutons dans le canvas.

        // On cr�e une boucle pour rattacher les boutons � leur fonction d�di�e.
        int i = 0; 

        foreach (Button button in buttonArray)
        {
            if (i == 0)
            {
                button.GetComponent<Button>().onClick.AddListener(button0);
            }

            if (i == 1)
            {
                button.GetComponent<Button>().onClick.AddListener(button1);
            }

            if (i == 2)
            {
                button.GetComponent<Button>().onClick.AddListener(button2);
            }

            if (i == 3)
            {
                button.GetComponent<Button>().onClick.AddListener(button3);
            }

            button.gameObject.SetActive(false);

            i += 1;
        }
    }


    void Update()
    {
        updateCooldown += Time.deltaTime;
        Debug.Log(updateCooldown);

        // On v�rifie si le joueur appuie sur F et si le dialogue n'est pas d�j� en train de se jouer.
        if ((Input.GetKey(activationKey)) && (!isDialoguePlaying)) 
        {
            if (dialogueTriggered == true) // Si le joueur se trouve dans la zone de d�tection du NPC, on active le dialogue.
            {
                StartDialogue();
                dialogueTriggered = false;
                updateCooldown = 0;
            }

            else // Si le dialogue a d�j� �t� lanc�, on avance dans le dialogue.
            {
                if ((story.currentChoices.Count == 0) && (updateCooldown > 0.5))
                {
                    ContinueDialogue();
                    updateCooldown = 0;
                }

            }
        }

        if ((Input.GetKey(activationKey)) && (isDialoguePlaying) && (updateCooldown > 0.5))
        {
            skipDialogue();
            updateCooldown = 0;
            
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
                    npcIcon.gameObject.SetActive(true); 
                    textPanelNPC.gameObject.SetActive(true); 
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

        playersIcon.gameObject.SetActive(true);

        // On affiche la premi�re ligne de dialogue
        ContinueDialogue();
    }

    private void ContinueDialogue()
    {
        if (story.canContinue)
        {
            isDialoguePlaying = true;
                
            // On lance l'affichage du texte
            StartCoroutine(DisplayNextLine());

            // On verifie les tags
            checkTags();
        }
        else
        {
            // Si le dialogue est termin�, finito pipo
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        textPanelNPC.gameObject.SetActive(false);
        textPanelDooms.gameObject.SetActive(false);
        textPanelSora.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        playersIcon.gameObject.SetActive(false);
        npcIcon.gameObject.SetActive(false);
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
                case "choice":
                    
                    Debug.Log(t);
                    for (int i = 0; i<4; i++)
                    {
                        Debug.Log(t.Split(' ')[i + 1]);
                        if ((t.Split(' ')[i+1]) == "Dooms")
                            buttonColorList[i] = Color.green;

                        if ((t.Split(' ')[i + 1]) == "Sora")
                            buttonColorList[i] = Color.magenta;

                        if ((t.Split(' ')[i + 1]) == "Neutral")
                            buttonColorList[i] = Color.white;
                    }
                    break;

                case "color":
                    // SetTextColor(param);
                    break;
                case "speaker":

                    textPanelNPC.gameObject.SetActive(false);
                    textPanelDooms.gameObject.SetActive(false);
                    textPanelSora.gameObject.SetActive(false);

                    if (param == "Dooms")
                        textPanelDooms.gameObject.SetActive(true);

                    if (param == "Sora")
                        textPanelSora.gameObject.SetActive(true);

                    if ((param != "Sora") && (param != "Dooms"))
                        textPanelNPC.gameObject.SetActive(true);

                    break;

                case "portrait":
                    break;
                case "debug":
                    Debug.Log(param);
                    break;

            }

        }
    }



    IEnumerator DisplayNextLine()
    {
        if (story.canContinue)
        {
            // On actualise le texte � afficher
            displayText = story.Continue();
            dialogueText.text = "";

            // On affiche les lettres une par une
            foreach (char letter in displayText.ToCharArray())
            {
                if (skip)
                {
                    skip = false;
                    break;
                }
                    
                dialogueText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }

            isDialoguePlaying = false;

            if (story.currentChoices.Count != 0) // Si le joueur se trouve � un embranchement, on affiche les diff�rents choix possibles.
            {
                StartCoroutine(ShowChoices());
            }

        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator ShowChoices()
    {
        Debug.Log("ChoicesDisplay");
        List<Choice> _choices = story.currentChoices;
        int i = 0;

        foreach (Button button in buttonArray)
        {
            if (i < _choices.Count)
            {
                button.gameObject.SetActive(true);
                button.image.color = buttonColorList[i];
                TextMeshProUGUI buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonText.text = _choices[i].text;
                yield return null;
            }

            i += 1;

        }

        


    }
    
    public void button0 ()
    {
        choiceButtonClick(0);
    }

    public void button1()
    {
        choiceButtonClick(1);
    }

    public void button2()
    {
        choiceButtonClick(2);
    }

    public void button3()
    {
        choiceButtonClick(3);
    }

    public void choiceButtonClick(int index)
    {
        updateCooldown = 0; 
        int choiceSelected = index;
        story.ChooseChoiceIndex(choiceSelected);
        foreach (Button button in buttonArray)
        {
            button.gameObject.SetActive(false);
        }
        choiceSelected = 99;
        ContinueDialogue();
    }

    private void skipDialogue()
    {
        StopCoroutine(DisplayNextLine());
        skip = true;
        dialogueText.text = displayText;
    }


}
