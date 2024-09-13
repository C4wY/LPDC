using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public TMPro.TextMeshPro hintText;

    public TextAsset inkJSONAsset;
    DialogueManager dialogueManager;

    [System.Obsolete]
    void Start()
    {
        if (hintText == null)
        {
            Debug.LogError("No hint text found");
            return;
        }

        hintText.enabled = false;
        dialogueManager = FindObjectOfType<DialogueManager>();

        var str = $"Press {dialogueManager.activationKey} to talk";
        hintText.text = str;
    }

    void Update()
    {
        if (Input.GetKey(dialogueManager.activationKey))
        {
            hintText.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var isLeader = LPDC.Avatar.GetLeader() == other.GetComponentInParent<LPDC.Avatar>();
        if (isLeader)
        {
            hintText.enabled = true;
            dialogueManager.SetStory(inkJSONAsset);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var isLeader = LPDC.Avatar.GetLeader() == other.GetComponentInParent<LPDC.Avatar>();
        if (isLeader)
        {
            hintText.enabled = false;
            dialogueManager.ClearStory();
        }
    }
}
