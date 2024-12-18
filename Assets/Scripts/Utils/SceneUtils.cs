using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    public static IEnumerable<GameObject> AllGameObjectsInScene()
    {
        // Get root objects in the active scene
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            // Use a helper method to enumerate through the hierarchy
            foreach (GameObject child in AllChildren(rootObject))
            {
                yield return child;
            }
        }
    }

    public static IEnumerable<GameObject> AllChildren(GameObject parent)
    {
        // Yield the parent first
        yield return parent;

        // Then recursively yield each child
        foreach (Transform child in parent.transform)
        {
            foreach (GameObject descendant in AllChildren(child.gameObject))
            {
                yield return descendant;
            }
        }
    }
}
