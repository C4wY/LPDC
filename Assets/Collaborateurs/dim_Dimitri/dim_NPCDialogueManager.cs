using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using System;

public class dim_NPCDialogueManager : MonoBehaviour
{
    // Variables Publiques
    public KeyCode activationKey = KeyCode.F; // La touche pour activer les dialogues.
    public TextAsset globalsInkJSON; // Le Json contenant les dialogues.
    public string DialogueStart = "Press F to talk";
    public Canvas canvas; // Le canvas contenant les �l�ments d'UI.
    public Sprite SpriteNPC1;

    // Variables Priv�es
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

    private Sprite angryDoomsIcon;
    private Sprite classicDoomsIcon;
    private Sprite classicSoraIcon;

    // Bool�ens de Contr�le
    private bool isDialogueActive = false;
    private bool dialogueTriggered = false;
    private bool isDialoguePlaying = false;



    private void Start()
    {

        // On r�cup�re les diff�rents �l�ments de l'UI depuis le canvas

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

        portraitAnimator = playersIcon.GetComponentInChildren<Animator>();

        buttonArray = canvas.GetComponentsInChildren<Button>(); // On r�cup�re les diff�rents boutons dans le canvas.

        // Test

        angryDoomsIcon = Resources.Load<Sprite>("Sprites/dim_Tests/dim_UI_Doooms Angry Test");
        Debug.Log(angryDoomsIcon);
        classicDoomsIcon = Resources.Load<Sprite>("Sprites/dim_RessourcesDialogues/UI_PortraitsDooms");
        Debug.Log(classicDoomsIcon);
        classicSoraIcon = Resources.Load<Sprite>("Sprites/dim_RessourcesDialogues/UI_PortraitsSora");
        Debug.Log(classicSoraIcon);

        // On r�cup�re le leader pour lancer l'animator

        var (sora, dooms) = LPDC.Avatar.GetSoraDooms();

        if (sora.IsLeader)
        {
            portraitAnimator.SetTrigger("EntrySoraLeader");
        }

        if (dooms.IsLeader)
        {
            portraitAnimator.SetTrigger("EntryDoomsLeader");
        }

        // Initialisations

        buttonColorList = new List<Color>();
        for (int j = 0; j < 4; j++)
            buttonColorList.Add(Color.white);

        // On cr�e une boucle pour rattacher les boutons � leur fonction d�di�e.
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
        // On cr�e un cooldown pour �viter que le jeu ne per�oive un input durant plusieurs frm�aes cons�cutives, car le joueur garde le doit appuy� sur la touche plus longtemps qu'une frame.

        updateCooldown += Time.deltaTime;

        // On v�rifie si le joueur appuie sur F et si le dialogue n'est pas d�j� en train de se jouer.

        if ((Input.GetKey(activationKey)) && (!isDialoguePlaying))
        {
            // Si le joueur se trouve dans la zone de d�tection du NPC, on active le dialogue.

            if (dialogueTriggered == true)
            {
                StartDialogue();
                dialogueTriggered = false;
                updateCooldown = 0;
            }

            // Si le dialogue a d�j� �t� lanc�, on avance dans le dialogue.

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

        // Test : le collider correspond au tag recherch�
        if (collider.gameObject.tag == "Player")
        {
            // Test : le collider est bien le leader
            var isLeader = LPDC.Avatar.GetLeader() == collider.GetComponentInParent<LPDC.Avatar>();
            if (isLeader)
            {
                if (!isDialogueActive)
                {
                    // On affiche le texte de d�part, proposant au joueur de lancer le dialogue.
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

    void OnTriggerExit(Collider collider)
    {
        EndDialogue();
    }

    void StartDialogue()
    {
        // On fige les personnages et pi�ges pendant le dialogue
        SetPauseForDialogue();

        // Initialisations
        story = new Story(globalsInkJSON.text);
        isDialogueActive = true;

        playersIcon.gameObject.SetActive(true);

        // Si Sora commence le dialogue

        //

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
            StartCoroutine(checkTags());
        }
        else
        {
            // Si le dialogue est termin�, finito pipo
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        // On remet les sprites de portrait de base
        GameObject doomsIcon = GameObject.Find("UI_PortraitsDooms_0");
        if (doomsIcon != null)
        {
            Image image = doomsIcon.GetComponent<Image>();
            image.sprite = classicDoomsIcon;
        }

        GameObject doomsIcon2 = GameObject.Find("UI_PortraitsDoomsSecondaire_0");
        if (doomsIcon2 != null)
        {
            Image image2 = doomsIcon2.GetComponent<Image>();
            //image2.sprite = classicDoomsIcon;
        }

        GameObject soraIcon = GameObject.Find("UI_PortraitsSora_0");
        if (soraIcon != null)
        {
            Image image3 = soraIcon.GetComponent<Image>();
            image3.sprite = classicSoraIcon;
        }

        GameObject soraIcon2 = GameObject.Find("UI_PortraitsSoraSecondaire_0");
        if (soraIcon != null)
        {
            Image image4 = soraIcon2.GetComponent<Image>();
            //image4.sprite = classicSoraIcon;
        }

        // On d�sactive les UI de dialogue et on remet � z�ro les bool�ens.
        textPanelNPC.gameObject.SetActive(false);
        textPanelDooms.gameObject.SetActive(false);
        textPanelSora.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        playersIcon.gameObject.SetActive(false);
        npcIcon.gameObject.SetActive(false);
        isDialogueActive = false;

        // On d�sactive ce script
        StopPauseForDialogue();
        //Behaviour script = GetComponent<dim_NPCDialogueManager>();
        //script.enabled = false;

    }

    IEnumerator checkTags()
    {
        // On r�cup�re les tags
        var tags = story.currentTags;
        foreach (string t in tags)
        {

            // On s�pare le pr�fix du param�tre (#color blue -> prefix = color, param = blue)
            string prefix = t.Split(' ')[0];
            string param = t.Split(' ')[1];
            switch (prefix.ToLower())
            {
                case "choice":

                    for (int i = 0; i < 4; i++)
                    {
                        if ((t.Split(' ')[i + 1]) == "Dooms")
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
                            yield return new WaitForSeconds(0.25f);
                        }
                        leaderSpeaker = param;
                    }


                    if (param == "Sora")
                    {
                        textPanelSora.gameObject.SetActive(true);
                        if (leaderSpeaker != param)
                        {
                            portraitAnimator.SetTrigger("DoomsToSora");
                            yield return new WaitForSeconds(0.25f);

                        }
                        leaderSpeaker = param;
                    }


                    if ((param != "Sora") && (param != "Dooms"))
                        textPanelNPC.gameObject.SetActive(true);
                    break;

                case "portraitdooms":
                    if (param == "angry")
                    {
                        GameObject doomsIcon = GameObject.Find("UI_PortraitsDooms_0");
                        Image image = doomsIcon.GetComponent<Image>();
                        image.sprite = angryDoomsIcon;
                        image.sprite = SpriteNPC1;
                        GameObject doomsIcon2 = GameObject.Find("UI_PortraitsDoomsSecondaire_0");
                        Image image2 = doomsIcon2.GetComponent<Image>();
                        image2.sprite = SpriteNPC1;
                    }
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
                // Si le joueur veut skip le dialogue, on casse la boucle pour arr�ter la coroutine.

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
        // On r�cup�re les diff�rents choix propos�s

        List<Choice> _choices = story.currentChoices;

        // On cr�e une boucle pour activer les diff�rents boutons

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

    // Les 4 fonctions suivantes servent � initialiser les boutons

    public void Button0()
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
        // On remet le cooldown � 0
        updateCooldown = 0;

        // On r�cup�re l'index du bouton sur lequel le joueur a cliqu�
        int choiceSelected = index;
        story.ChooseChoiceIndex(choiceSelected);

        // On fait disparaitre les boutons
        foreach (Button button in buttonArray)
        {
            button.gameObject.SetActive(false);
        }

        // On r�initialise la variable
        choiceSelected = 99;

        // On peut d�sormais continuer le dialogue
        ContinueDialogue();
    }

    private void SkipDialogue()
    {
        // On arr�te la coroutine pour que le texte cesse de se taper lettre par lettre
        StopCoroutine(DisplayNextLine());

        // En r�alit�, �a ne fonctionne pas, donc on utilise une m�thode alternative pour arr�ter la coroutine de mani�re plus directe (cf le code de la coroutine DisplayNextLine)
        skip = true;

        // On affiche directement la ligne de dialogue enti�re
        dialogueText.text = displayText;
    }

    public void SetPauseForDialogue()
    {
        // on r�cup�re tous les objets de la sc�ne
        GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        // On d�clenche la fonction OnPauseForDialogue pour chaque objet en poss�dant une
        foreach (GameObject actor in gameObjects)
        {
            actor.SendMessage("OnPauseForDialogue", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void StopPauseForDialogue()
    {
        // on r�cup�re tous les objets de la sc�ne
        GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        // On d�clenche la fonction OffPauseForDialogue pour chaque objet en poss�dant une
        foreach (GameObject actor in gameObjects)
        {
            actor.SendMessage("OffPauseForDialogue", SendMessageOptions.DontRequireReceiver);
        }
    }

}
