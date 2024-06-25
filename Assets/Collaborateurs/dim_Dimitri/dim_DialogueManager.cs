using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class dim_DialogueManager : MonoBehaviour
{
    public TextAsset globalsInkJSON;
    public KeyCode activationKey = KeyCode.F;
    private Story story;


    // Start is called before the first frame update
    void Start()
    {
        var globalsStory = new Story(globalsInkJSON.text);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStory(TextAsset inkJSONAsset)
    {
        story = new Story(inkJSONAsset.text);
        var globalsVariables = new Story(globalsInkJSON.text).variablesState;
        foreach (string variableName in globalsVariables)
        {
            story.variablesState[variableName] = globalsVariables[variableName];
        }
    }

    public void StartDialogue (Key)
    {

    }
}
