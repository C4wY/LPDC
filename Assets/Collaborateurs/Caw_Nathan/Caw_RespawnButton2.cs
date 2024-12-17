using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Caw_RespawnButton2 : MonoBehaviour
{
    public Animator fadeAnimator;
    public string fadeOutTrigger = "Start";
    public float animationDuration = 1f;

    public void OnRespawnButtonClicked()
    {
        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger(fadeOutTrigger);
        }

        yield return new WaitForSeconds(animationDuration);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}