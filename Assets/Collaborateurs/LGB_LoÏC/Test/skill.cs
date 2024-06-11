using UnityEngine;

public class skill : MonoBehaviour
{
    [System.Serializable]
    public class SkillKeyBinding
    {
        public KeyCode key;
        public Skill skill;
    }

    [System.Serializable]
    public class SkillSet
    {
        public string setName;
        public SkillKeyBinding[] skillBindings;
    }

    public SkillSet[] skillSets;
    private int currentSkillSetIndex = 0;

    void Update()
    {
        // Switch skill set
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentSkillSetIndex = (currentSkillSetIndex + 1) % skillSets.Length;
            Debug.Log("Switched to skill set: " + skillSets[currentSkillSetIndex].setName);
        }

        // Activate skills in the current skill set
        foreach (var binding in skillSets[currentSkillSetIndex].skillBindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                binding.skill?.Activate(gameObject);
            }
        }
    }
}
