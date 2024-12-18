using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Portal : MonoBehaviour
{
    [SerializeField] private Object targetScene;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionDuration = 2f;

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

    if (transitionAnimator == null)
    {
        transitionAnimator = GetComponentInChildren<Animator>();
        if (transitionAnimator == null)
        {
            Debug.LogError("Aucun Animator trouvé");
        }
    }
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
            StartCoroutine(LoadTargetScene());
        }
    }

    private IEnumerator LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("Start");
                yield return new WaitForSeconds(transitionDuration);
            }

            SceneManager.LoadScene("Biz_Lobby");
        }
        else
        {
            Debug.LogWarning("Aucune cible");
        }
    }
    
}

