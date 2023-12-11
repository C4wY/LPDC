using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    bool pause = false;

    // Appelé lorsque le bouton Pause est pressé
    public void TogglePause()
    {
        if (pause)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Fonction pour mettre en pause le jeu
    void PauseGame()
    {
        pause = true;
        Time.timeScale = 0f; // Met le temps à zéro, arrêtant le déroulement du jeu
        pauseMenuUI.SetActive(true); // Active le menu pause
    }

    // Fonction pour reprendre le jeu
    public void ResumeGame()
    {
        pause = false;
        Time.timeScale = 1f; // Remet le temps à la normale, reprenant le déroulement du jeu
        pauseMenuUI.SetActive(false); // Désactive le menu pause
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}
