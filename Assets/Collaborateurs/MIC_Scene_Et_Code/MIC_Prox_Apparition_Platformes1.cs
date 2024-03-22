using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Prox_Apparition_Platformes : MonoBehaviour


{
    public KeyCode activationKey = KeyCode.F;
    public float interactionDistance = 3f;
    public GameObject leverText;
    public GameObject[] platformsToActivate;

    private bool isPlayerInRange = false;
    private bool leverActivated = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(activationKey))
        {
            ActivateLever();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            leverText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            leverText.SetActive(false);
        }
    }

    private void ActivateLever()
    {
        if (!leverActivated)
        {
            foreach (GameObject platform in platformsToActivate)
            {
                platform.SetActive(true);
            }
            leverActivated = true;
            Debug.Log("Le levier a été activé !");
        }
    }
}