using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Importer l'espace de noms Avatar
using LPDC;

public class MIC_Prox_Apparition_PlatformesV2 : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.F;
    public float interactionDistance = 3f;
    public GameObject leverText;
    public GameObject[] platformsToActivate;
    public AudioClip leverSound;
    public AudioSource audioSource;

    private bool isLeaderInRange = false;
    private bool leverActivated = false;

    void Update()
    {
        // Vérifier si l'Avatar Leader est dans la zone du levier
        if (IsLeaderInRange() && Input.GetKeyDown(activationKey))
        {
            ActivateLever();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si le collider appartient à un Avatar Leader
        if (IsLeader(other.gameObject))
        {
            isLeaderInRange = true;
            leverText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Vérifier si le collider appartient à un Avatar Leader
        if (IsLeader(other.gameObject))
        {
            isLeaderInRange = false;
            leverText.SetActive(false);
        }
    }

    private bool IsLeaderInRange()
    {
        // Vérifier si l'Avatar Leader est dans la zone du levier
        return isLeaderInRange;
    }

    private bool IsLeader(GameObject obj)
    {
        // Vérifier si l'objet est un Avatar avec le rôle Leader
        return obj.name.Contains("(Leader)");
    }

    private void ActivateLever()
    {
        if (!leverActivated)
        {
            // Activer les plateformes
            foreach (GameObject platform in platformsToActivate)
            {
                platform.SetActive(true);
            }
            leverActivated = true;
            Debug.Log("Le levier a été activé !");

            // Jouer le son du levier
            if (audioSource && leverSound)
            {
                audioSource.PlayOneShot(leverSound);
            }
        }
    }
}