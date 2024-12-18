using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Jnc_DialogueManager : MonoBehaviour
{
    public float characterDelay = 0.033f;

    TMPro.TextMeshProUGUI mainText;

    Story story;
    Coroutine displayTextCoroutine;

    public void EnterDialogue(TextAsset inkDialogue)
    {
        gameObject.SetActive(true);
        story = new Story(inkDialogue.text);
        displayTextCoroutine = StartCoroutine(DisplayText());
    }

    public void EndCurrentDialogue()
    {
        if (displayTextCoroutine != null)
            StopCoroutine(displayTextCoroutine);

        story = null;
        displayTextCoroutine = null;
        gameObject.SetActive(false);
    }

    // Does the user require to cut the current dialogue short?
    bool requireCutShort = false;

    bool UserRequireCutShort()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
    }

    bool ConsumeCutShort()
    {
        var result = requireCutShort;
        requireCutShort = false;
        return result;
    }

    void CheckTags()
    {
        foreach (var tag in story.currentTags)
        {
            var parts = tag.Split(' ').Select(x => x.Trim().ToLower()).ToArray();

            if (parts.Length < 2)
                continue; // Invalid tag

            var key = parts[0];
            var value = parts[1];

            switch (key)
            {
                case "speaker":
                    GameManager.GetInstanceOrThrow().dualPortraitEvent.Invoke(new()
                    {
                        mainAvatar = value == "sora" ? LPDC.Avatar.Name.Sora : LPDC.Avatar.Name.Dooms,
                    });
                    break;
            }
        }
    }

    IEnumerator DisplayText()
    {
        mainText.text = "";

        yield return new WaitForSeconds(characterDelay * 4);

        while (story.canContinue)
        {
            var text = story.Continue();

            CheckTags();

            for (int i = 0; i < text.Length; i++)
            {
                if (ConsumeCutShort())
                    break;

                var isWhiteSpace = char.IsWhiteSpace(text[i]);

                if (!isWhiteSpace)
                    yield return new WaitForSeconds(characterDelay);

                mainText.text = text[..i];
            }

            mainText.text = text;

            yield return new WaitForSeconds(characterDelay * 4);

            yield return new WaitUntil(() => ConsumeCutShort());
        }

        EndCurrentDialogue();
    }

    void Awake()
    {
        mainText = transform.Find("MainText").GetComponent<TMPro.TextMeshProUGUI>();

        EndCurrentDialogue();
    }

    void Update()
    {
        requireCutShort = requireCutShort || UserRequireCutShort();

        if (Input.GetKeyDown(KeyCode.Escape))
            EndCurrentDialogue();
    }

    public TextAsset debugInkDialogue;
#if UNITY_EDITOR
    [CustomEditor(typeof(Jnc_DialogueManager))]
    public class Jnc_DialogueManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var dm = (Jnc_DialogueManager)target;

            GUI.enabled = dm.debugInkDialogue != null && Application.isPlaying;
            if (GUILayout.Button("Enter Dialogue"))
                if (dm.debugInkDialogue != null)
                    dm.EnterDialogue(dm.debugInkDialogue);
        }
    }
#endif
}
