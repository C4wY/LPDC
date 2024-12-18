using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Caw_RespawnButton2 : MonoBehaviour
{
    public string levelLoaderName = "Level Loader";
    public string fadeOutTrigger = "Start";
    public float animationDuration = 1f;

    public void OnRespawnButtonClicked()
    {
        StartCoroutine(ReloadScene());

        GameObject.Find(levelLoaderName)
            .GetComponentInChildren<Animator>()
            .SetTrigger(fadeOutTrigger);
    }

    private IEnumerator ReloadScene()
    {
        yield return TimeUtils.WaitUnscaledTime(animationDuration);

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}