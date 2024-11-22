using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneDelayLoader : MonoBehaviour
{
    [SerializeField] private Object targetScene;
    [SerializeField] private float delay = 3f;

    private string sceneName;

    private void Start()
    {
#if UNITY_EDITOR
        if (targetScene != null && targetScene is SceneAsset sceneAsset)
        {
            sceneName = sceneAsset.name;
        }

#endif
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadSceneAfterDelay());
        }
        else
        {
            Debug.LogError("definir scene cible");
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}

