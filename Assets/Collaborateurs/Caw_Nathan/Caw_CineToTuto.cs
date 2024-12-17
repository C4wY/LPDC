using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Caw_CineToTuto : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [SerializeField] private string nextSceneName;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
