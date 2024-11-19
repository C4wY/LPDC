using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Portal : MonoBehaviour
{
    [SerializeField] private Object targetScene;

    private string sceneName;
    private bool isPlayerInPortal = false;

    private void Start()
    {
#if UNITY_EDITOR
        if (targetScene != null && targetScene is SceneAsset sceneAsset)
        {
            sceneName = sceneAsset.name;
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false;
        }
    }

    private void Update()
    {
        if (isPlayerInPortal && Input.GetKeyDown(KeyCode.F))
        {
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Aucune cible");
        }
    }
}
