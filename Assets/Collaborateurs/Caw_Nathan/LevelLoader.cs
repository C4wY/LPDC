using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    private bool canLoadNextLevel = false;

    private void Start()
    {
        StartCoroutine(StartLevelTransition());
    }

    public void OnNextLevelButtonPressed()
    {
        if (canLoadNextLevel)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator StartLevelTransition()
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
        }

        canLoadNextLevel = true;
    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
        }

        SceneManager.LoadScene(levelIndex);
    }
}
