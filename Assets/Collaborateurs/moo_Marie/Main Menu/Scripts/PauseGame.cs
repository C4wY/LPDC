using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Continuer();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Continuer()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Options()
    {
        SceneManager.LoadScene("Unj_Controles manette");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Unj_MainMenu");
    }

    public void Quitter()
    {
        Application.Quit();
    }
}
