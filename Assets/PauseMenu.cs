using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    // Appelé lorsque le bouton Pause est pressé
    public void TogglePauseMenu()
    {
        if (pauseMenuUI.activeSelf)
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
        Time.timeScale = 0f; // Met le temps à zéro, arrêtant le déroulement du jeu
        pauseMenuUI.SetActive(true); // Active le menu pause
    }

    // Fonction pour reprendre le jeu
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Remet le temps à la normale, reprenant le déroulement du jeu
        pauseMenuUI.SetActive(false); // Désactive le menu pause
    }
}
