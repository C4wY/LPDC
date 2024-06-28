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
    public Sprite SpriteNPC1;

    // Variables Privées
    private Story story;
    private TextMeshProUGUI dialogueText;
    private Image textPanel;
    private GameObject textPanelNPC;
    private GameObject textPanelDooms;
    private GameObject textPanelSora;
    private GameObject playersIcon;
    private GameObject npcIcon;
    private Animator portraitAnimator;
    private Array buttonArray;
    private List<Color> buttonColorList;
    private string displayText;
    private string leaderSpeaker;
    private float updateCooldown;
    private bool skip = false;

    // Booléens de Contrôle
    private bool isDialogueActive = false;
    private bool dialogueTriggered = false;
    private bool isDialoguePlaying = false;
    
 

    private void Start()
    {

        // On récupère les différents éléments de l'UI depuis le canvas

        dialogueText = canvas.GetComponentInChildren<TextMeshProUGUI>(); // On récupère la zone de texte dans le canvas.
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

        portraitAnimator = playersIcon.GetComponentInChildren<Animator>();

        buttonArray = canvas.GetComponentsInChildren<Button>(); // On récupère les différents boutons dans le canvas.

        // On récupère le leader pour lancer l'animator

        var (sora, dooms) = Avatar.Avatar.GetSoraDooms();
        
        //if (sora.IsLeader)
        {
            //portraitAnimator.SetTrigger("EntrySoraLeader"); 
        }
            
        //if (dooms.IsLeader)
        {
            //portraitAnimator.SetTrigger("EntryDoomsLeader"); 
        }
        
        // Initialisations

        buttonColorList = new List<Color>();
        for (int j = 0; j < 4; j++)
            buttonColorList.Add(Color.white);

        // On crée une boucle pour rattacher les boutons à leur fonction dédiée.
        int i = 0; 

        foreach (Button button in buttonArray)
        {
            if (i == 0)
            {
                button.GetComponent<Button>().onClick.AddListener(Button0);
            }

            if (i == 1)
            {
                button.GetComponent<Button>().onClick.AddListener(Button1);
            }

            if (i == 2)
            {
                button.GetComponent<Button>().onClick.AddListener(Button2);
            }

            if (i == 3)
            {
                button.GetComponent<Button>().onClick.AddListener(Button3);
            }

            button.gameObject.SetActive(false);

            i += 1;
        }
    }


    void Update()
    {
        // On crée un cooldown pour éviter que le jeu ne perçoive un input durant plusieurs frmùaes consécutives, car le joueur garde le doit appuyé sur la touche plus longtemps qu'une frame.

        updateCooldown += Time.deltaTime;

        // On vérifie si le joueur appuie sur F et si le dialogue n'est pas déjà en train de se jouer.

        if ((Input.GetKey(activationKey)) && (!isDialoguePlaying)) 
        {
            // Si le joueur se trouve dans la zone de détection du NPC, on active le dialogue.
            
            if (dialogueTriggered == true) 
            {
                StartDialogue();
                dialogueTriggered = false;
                updateCooldown = 0;
            }

            // Si le dialogue a déjà été lancé, on avance dans le dialogue.

            else
            {
                if ((story.currentChoices.Count == 0) && (updateCooldown > 0.2f))
                {
                    ContinueDialogue();
                    updateCooldown = 0;
                }

            }
        }

        // Si le dialogue est en cours, alors le joueur veut skip l'animation de dialogue.

        if ((Input.GetKey(activationKey)) && (isDialoguePlaying) && (updateCooldown > 0.2f)) 
        {
            SkipDialogue();
            updateCooldown = 0;
            
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
                    // On affiche le texte de départ, proposant au joueur de lancer le dialogue.
                    npcIcon.gameObject.SetActive(true);
                    Image image = npcIcon.GetComponent<Image>();
                    image.sprite = SpriteNPC1;
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
        // On fige les personnages et pièges pendant le dialogue
        SetPauseForDialogue();

        // Initialisations
        story = new Story(globalsInkJSON.text);
        isDialogueActive = true;

        playersIcon.gameObject.SetActive(true);

        // Si Sora commence le dialogue

        //

        // On affiche la première ligne de dialogue
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
            // Si le dialogue est terminé, finito pipo
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        // On désactive les UI de dialogue et on remet à zéro les booléens.

        textPanelNPC.gameObject.SetActive(false);
        textPanelDooms.gameObject.SetActive(false);
        textPanelSora.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        playersIcon.gameObject.SetActive(false);
        npcIcon.gameObject.SetActive(false);
        isDialogueActive = false;
        StopPauseForDialogue();
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
                case "choice":
                    
                    for (int i = 0; i<4; i++)
                    {
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
                    {
                        textPanelDooms.gameObject.SetActive(true);
                        if (leaderSpeaker != param)
                        {
                            portraitAnimator.SetTrigger("SoraToDooms");
                        }
                        leaderSpeaker = param;
                    }
                        

                    if (param == "Sora")
                    {
                        textPanelSora.gameObject.SetActive(true);
                        if (leaderSpeaker != param)
                        {
                            portraitAnimator.SetTrigger("DoomsToSora");

                        }
                        leaderSpeaker = param;
                    }
                        

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
            // On actualise le texte à afficher
            displayText = story.Continue();
            dialogueText.text = "";

            // On affiche les lettres une par une
            foreach (char letter in displayText.ToCharArray())
            {
                // Si le joueur veut skip le dialogue, on casse la boucle pour arrêter la coroutine.

                if (skip)
                {
                    skip = false;
                    break;
                }
                    
                dialogueText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }

            isDialoguePlaying = false;

            if (story.currentChoices.Count != 0) // Si le joueur se trouve à un embranchement, on affiche les différents choix possibles.
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
        // On récupère les différents choix proposés
        
        List<Choice> _choices = story.currentChoices;

        // On crée une boucle pour activer les différents boutons

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
    
    // Les 4 fonctions suivantes servent à initialiser les boutons

    public void Button0 ()
    {
        ChoiceButtonClick(0);
    }

    public void Button1()
    {
        ChoiceButtonClick(1);
    }

    public void Button2()
    {
        ChoiceButtonClick(2);
    }

    public void Button3()
    {
        ChoiceButtonClick(3);
    }

    public void ChoiceButtonClick(int index)
    {
        // On remet le cooldown à 0
        updateCooldown = 0; 

        // On récupère l'index du bouton sur lequel le joueur a cliqué
        int choiceSelected = index;
        story.ChooseChoiceIndex(choiceSelected);

        // On fait disparaitre les boutons
        foreach (Button button in buttonArray)
        {
            button.gameObject.SetActive(false);
        }

        // On réinitialise la variable
        choiceSelected = 99;

        // On peut désormais continuer le dialogue
        ContinueDialogue();
    }

    private void SkipDialogue()
    {
        // On arrête la coroutine pour que le texte cesse de se taper lettre par lettre
        StopCoroutine(DisplayNextLine());

        // En réalité, ça ne fonctionne pas, donc on utilise une méthode alternative pour arrêter la coroutine de manière plus directe (cf le code de la coroutine DisplayNextLine)
        skip = true;

        // On affiche directement la ligne de dialogue entière
        dialogueText.text = displayText;
    }

    public void SetPauseForDialogue()
    {
        // on récupère tous les objets de la scène
        GameObject[] gameObjects = FindObjectsOfType<GameObject>() as GameObject[];

        // On déclenche la fonction OnPauseForDialogue pour chaque objet en possédant une
        foreach (GameObject actor in gameObjects)
        {
            actor.SendMessage("OnPauseForDialogue", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void StopPauseForDialogue()
    {
        // on récupère tous les objets de la scène
        GameObject[] gameObjects = FindObjectsOfType<GameObject>() as GameObject[];

        // On déclenche la fonction OffPauseForDialogue pour chaque objet en possédant une
        foreach (GameObject actor in gameObjects)
        {
            actor.SendMessage("OffPauseForDialogue", SendMessageOptions.DontRequireReceiver);
        }
    }

}
