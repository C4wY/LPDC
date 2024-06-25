using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class dim_DialogueManagerProvisoire : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private Story story;
    public TextAsset globalsInkJSON;
    private bool isDialogueActive = false;
    public GameObject customButton;
    public GameObject choicesPanel;
    public GameObject testButtonYes;
    public GameObject testButtonNo;
    private int choiceSelected;



    // Start is called before the first frame update
    void StartDialogue()
    {
        testButtonYes.gameObject.SetActive(false);
        testButtonNo.gameObject.SetActive(false);

        story = new Story(globalsInkJSON.text);
        isDialogueActive = true;
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
            
            // On lance l'affichage du texte
            StartCoroutine(DisplayNextLine());
        }
        else
        {
            // Si le dialogue est terminé, finito pipo
            EndDialogue();
        }
    }

    private IEnumerator DisplayNextLine()
    {
        if (story.canContinue)
        {
            // On actualise le texte à afficher
            string text = story.Continue();
            dialogueText.text = text;

            float timer = 0f;
            while (timer < 2f && !Input.GetKeyDown(KeyCode.F))
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

    private IEnumerator ShowChoices()
    {
        Debug.Log("Time for a Choice");
        testButtonYes.gameObject.SetActive(true);
        testButtonNo.gameObject.SetActive(true);
        List<Choice> _choices = story.currentChoices;

        for (int i = 0; i < _choices.Count; i++)
        {
            
            // GameObject button = Instantiate (customButton, choicesPanel.transform);
            // button.transform.GetChild(0).GetComponent(Tetx)
        }
        testButtonYes.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _choices[0].text;
        testButtonNo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _choices[1].text;

        yield return new WaitUntil (() => { return choiceSelected != 99 ; });

        ContinueFromDecision();
        

        //testButtonYes.AddComponent<Selectable>();
        //testButtonYes.GetComponent<Selectable>().element = _choices[0];
        //testButtonYes.GetComponent<Button>().onClick.AddListener(() => { testButton.GetComponent<Selectable>().Decide(); });
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else
            {
                ContinueDialogue();
                
                if (story.currentChoices.Count != 0)
                {
                    Debug.Log("Show Choices");
                    StartCoroutine(ShowChoices());
                    // ShowChoices();
                }

            }
        }

    }

    public void Choice(int index)
    {
        choiceSelected = index;
        story.ChooseChoiceIndex(choiceSelected);
        testButtonYes.gameObject.SetActive(false);
        testButtonNo.gameObject.SetActive(false);
        choiceSelected = 99;
        ContinueDialogue();
    }

    private void ContinueFromDecision ()
    {
        ContinueDialogue();
    }
}
