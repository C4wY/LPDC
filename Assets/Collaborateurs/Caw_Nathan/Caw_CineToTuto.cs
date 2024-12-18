using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Caw_CineToTuto : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    void Start()
    {
        var videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer.clip == null)
        {
            OnVideoEnd();
            return;
        }

        videoPlayer.loopPointReached += _ => OnVideoEnd();
    }

    void OnVideoEnd()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
