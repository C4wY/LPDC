using System.Collections;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextAsset globalsInkJSON;
    public KeyCode activationKey = KeyCode.F;

    private Story story;
    private bool isDialogueActive = false;
    private bool isLeaderInRange = true;

    void Start()
    {
        dialogueText.gameObject.SetActive(false);
        InitializeGlobals();
    }

    void Update()
    {
        if (isLeaderInRange && Input.GetKeyDown(activationKey))
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else
            {
                ContinueDialogue();
            }
        }
    }

    public void SetStory(TextAsset inkJSONAsset)
    {
        story = new Story(inkJSONAsset.text);
        ApplyGlobals();
    }

    private void StartDialogue()
    {
        if (story.canContinue)
        {
            isDialogueActive = true;
            dialogueText.gameObject.SetActive(true);
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
            dialogueText.text = text;

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
        isDialogueActive = false;
        if (!isLeaderInRange)
        {
            dialogueText.gameObject.SetActive(false);
        }
        else
        {
            dialogueText.text = "Appuyez sur 'F' pour parler";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsLeader(other.gameObject))
        {
            isLeaderInRange = true;
            dialogueText.gameObject.SetActive(true);
            dialogueText.text = "Appuyez sur 'F' pour parler";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsLeader(other.gameObject))
        {
            isLeaderInRange = false;
            if (!isDialogueActive)
            {
                dialogueText.gameObject.SetActive(false);
            }
        }
    }

    private bool IsLeader(GameObject obj)
    {
        return obj.name.Contains("(Leader)");
    }

    private void InitializeGlobals()
    {
        // Initialize the global variables story
        var globalsStory = new Story(globalsInkJSON.text);
        // Optionally, you can process globalsStory here if needed
    }

    private void ApplyGlobals()
    {
        // Transfer the variables from the globals story to the current story
        var globalsVariables = new Story(globalsInkJSON.text).variablesState;
        foreach (string variableName in globalsVariables)
        {
            story.variablesState[variableName] = globalsVariables[variableName];
        }
    }
}

