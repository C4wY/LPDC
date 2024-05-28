using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    private DialogueManager dialogueManager;

    [System.Obsolete]
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dialogueManager != null && other.gameObject.name.Contains("(Leader)"))
        {
            dialogueManager.SetStory(inkJSONAsset);
        }
    }
}
