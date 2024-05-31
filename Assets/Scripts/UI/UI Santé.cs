using UnityEngine;
using UnityEngine.UI;

public class UpdateChildImages : MonoBehaviour
{
    public int PV; // Public variable to set the threshold
    public Sprite plein; // Sprite to set if i < PV
    public Sprite vide; // Sprite to set if i >= PV

    void UpdateChildSprites()
    {
        int childCount = transform.childCount;

        // Debug log to verify the number of children
        Debug.Log("Number of children: " + childCount);

        // Ensure the loop does not exceed the number of children
        for (int i = 0; i < Mathf.Min(3, childCount); i++)
        {
            Transform child = transform.GetChild(i);

            // Debug log to verify each child's index
            Debug.Log("Processing child index: " + i);

            if (child.TryGetComponent<Image>(out var image))
            {
                // Debug log to verify if the component was found
                Debug.Log("Image component found on child index: " + i);

                if (i < PV)
                {
                    image.sprite = plein;
                }
                else
                {
                    image.sprite = vide;
                }

                // Debug log to verify the sprite assignment
                Debug.Log("Assigned sprite to child index: " + i);
            }
            else
            {
                // Debug log to verify if the component was not found
                Debug.Log("Image component not found on child index: " + i);
            }
        }
    }

    // Optionally, you can call this method from Start or Update
    void Start()
    {
        UpdateChildSprites();
    }
}
