using System.Collections;
using System.Linq;
using UnityEngine;
using Ink.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Jnc_DialogueManager : MonoBehaviour
{
    public float characterDelay = 0.033f;

    TMPro.TextMeshProUGUI mainText;
    GameObject[] choices;
    TMPro.TextMeshProUGUI[] choiceTexts;

    Story story;
    Coroutine displayTextCoroutine;

    public void EnterNewDialogue(TextAsset inkDialogue)
    {
        gameObject.SetActive(true);
        story = new Story(inkDialogue.text);
        displayTextCoroutine = StartCoroutine(DisplayText());

        foreach (var choice in choices)
            choice.SetActive(false);
    }

    public void EndCurrentDialogue()
    {
        if (displayTextCoroutine != null)
            StopCoroutine(displayTextCoroutine);

        story = null;
        displayTextCoroutine = null;
        gameObject.SetActive(false);
    }

    // Does the user want to advance the text?
    bool userAdvance = false;

    bool UserRequireAdvance()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
    }

    bool ConsumeAdvance()
    {
        var result = userAdvance;
        userAdvance = false;
        return result;
    }

    void CheckTags()
    {
        foreach (var tag in story.currentTags)
        {
            // Tags are in the form of "key value", e.g. "speaker Sora"
            var parts = tag.Split(' ').Select(x => x.Trim().ToLower()).ToArray();

            if (parts.Length < 2)
                continue; // Invalid tag

            var key = parts[0];
            var value = parts[1];

            switch (key)
            {
                case "speaker":
                    switch (value)
                    {
                        case "sora":
                            GameManager.DispatchDualPortraitEvent(new()
                            {
                                mainAvatar = LPDC.Avatar.Name.Sora,
                            });
                            break;
                        case "dooms":
                            GameManager.DispatchDualPortraitEvent(new()
                            {
                                mainAvatar = LPDC.Avatar.Name.Dooms,
                            });
                            break;
                        default:
                            Debug.Log($"Unhandled speaker tag: {value}");
                            break;
                    }
                    break;
            }
        }
    }

    void CheckChoices()
    {
        for (int i = 0; i < choices.Length; i++)
        {
            var choice = choices[i];
            var choiceText = choiceTexts[i];

            if (story.currentChoices.Count > i)
            {
                choice.SetActive(true);
                choiceText.text = story.currentChoices[i].text;
            }
            else
            {
                choice.SetActive(false);
            }
        }
    }

    IEnumerator DisplayText()
    {
        mainText.text = "";

        // Initial delay
        yield return new WaitForSeconds(characterDelay * 4);

        while (story.canContinue)
        {
            var text = story.Continue();

            CheckTags();
            CheckChoices();

            for (int i = 0; i < text.Length; i++)
            {
                // Check if user wants to advance
                if (ConsumeAdvance())
                    break;

                var isWhiteSpace = char.IsWhiteSpace(text[i]);

                if (!isWhiteSpace)
                    yield return new WaitForSeconds(characterDelay);

                mainText.text = text[..i];
            }

            // Ensure full text is displayed (in case user cut it off)
            mainText.text = text;

            // Delay after finishing a line
            yield return new WaitForSeconds(characterDelay * 4);

            // Wait for user to advance
            yield return new WaitUntil(ConsumeAdvance);
        }

        EndCurrentDialogue();
    }

    void Awake()
    {
        mainText = transform.Find("MainText").GetComponent<TMPro.TextMeshProUGUI>();
        choices = transform.Find("Choices").Cast<Transform>().Select(t => t.gameObject).ToArray();
        choiceTexts = choices.Select(c => c.GetComponentInChildren<TMPro.TextMeshProUGUI>()).ToArray();

        EndCurrentDialogue();
    }

    void Update()
    {
        userAdvance = userAdvance || UserRequireAdvance();

        if (Input.GetKeyDown(KeyCode.Escape))
            EndCurrentDialogue();
    }

#if UNITY_EDITOR
    public TextAsset debugInkDialogue;
    [CustomEditor(typeof(Jnc_DialogueManager))]
    public class Jnc_DialogueManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var dm = (Jnc_DialogueManager)target;

            GUI.enabled = dm.debugInkDialogue != null && Application.isPlaying;
            if (GUILayout.Button("Enter New Dialogue"))
                if (dm.debugInkDialogue != null)
                    dm.EnterNewDialogue(dm.debugInkDialogue);
        }
    }
#endif
}
