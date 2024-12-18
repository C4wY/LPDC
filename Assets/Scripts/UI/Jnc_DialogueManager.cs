using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Jnc_DialogueManager : MonoBehaviour
{
    Story story;

    TMPro.TextMeshProUGUI mainText;

    public void EnterDialogue(TextAsset inkDialogue)
    {
        story = new Story(inkDialogue.text);
        StartCoroutine(DisplayText());
    }

    IEnumerator DisplayText()
    {
        while (story.canContinue)
        {
            var text = story.Continue();

            for (int i = 0; i < text.Length; i++)
            {
                yield return new WaitForSeconds(0.1f);
                mainText.text = text[..i];
            }
            yield break;
        }
    }

    void Start()
    {
        mainText = transform.Find("MainText").GetComponent<TMPro.TextMeshProUGUI>();
        mainText.text = "";
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
