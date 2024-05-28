using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class MIC_Interaction_Dialogue_Ink : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.F;
    public float interactionDistance = 3f;
    public TextAsset inkJSONAsset;
    public Text uiText;

    private Story story;
    private bool isLeaderInRange = false;
    private bool dialogueActive = false;

    void Start()
    {
        story = new Story(inkJSONAsset.text);
        uiText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (IsLeaderInRange() && Input.GetKeyDown(activationKey))
        {
            if (!dialogueActive)
            {
                StartDialogue();
            }
            else
            {
                ContinueDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsLeader(other.gameObject))
        {
            isLeaderInRange = true;
            uiText.gameObject.SetActive(true);
            uiText.text = "Press 'F' to talk";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsLeader(other.gameObject))
        {
            isLeaderInRange = false;
            if (!dialogueActive)
            {
                uiText.gameObject.SetActive(false);
            }
        }
    }

    private bool IsLeaderInRange()
    {
        return isLeaderInRange;
    }

    private bool IsLeader(GameObject obj)
    {
        return obj.name.Contains("(Leader)");
    }

    private void StartDialogue()
    {
        if (story.canContinue)
        {
            dialogueActive = true;
            uiText.gameObject.SetActive(true);
            StartCoroutine(DisplayNextLine());
        }
    }

    private void ContinueDialogue()
    {
        if (story.canContinue)
        {
            StartCoroutine(DisplayNextLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator DisplayNextLine()
    {
        if (story.canContinue)
        {
            string text = story.Continue();
            uiText.text = text;

            // Attendre 2 secondes ou jusqu'à ce que le joueur appuie sur une touche pour continuer le dialogue
            float timer = 0f;
            while (timer < 2f && !Input.GetKeyDown(activationKey))
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        if (!isLeaderInRange)
        {
            uiText.gameObject.SetActive(false);
        }
        else
        {
            uiText.text = "Press 'F' to talk";
        }
    }
} 
