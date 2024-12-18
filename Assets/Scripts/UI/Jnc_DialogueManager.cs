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
            Debug.Log(text);
            yield break;
        }
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
