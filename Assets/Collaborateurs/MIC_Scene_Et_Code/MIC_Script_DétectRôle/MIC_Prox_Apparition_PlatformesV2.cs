using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Importer l'espace de noms Avatar
using Avatar;

public class MIC_Prox_Apparition_PlatformesV2 : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.F;
    public float interactionDistance = 3f;
    public GameObject leverText;
    public GameObject[] platformsToActivate;
    public AudioClip leverSound;
    public AudioSource audioSource;

    private bool isLeaderInRange = false; // Utilisation du rôle Leader
    private bool leverActivated = false;

    //Utiliser Avatar.Avatar comme type de cible
    public Avatar.Avatar target;
    // Référence directe à l'avatar actif

    void Update()
    {
        if (isLeaderInRange && Input.GetKeyDown(activationKey)) // Utilisation de isLeaderInRange
        {
            
            
            ActivateLever();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si le collider appartient à un objet avec le script Avatar et est un leader
        Avatar.Avatar  avatar = other.GetComponent<Avatar.Avatar>();
        if (avatar != null && avatar.IsLeader)
        {
            isLeaderInRange = true; // Utilisation de isLeaderInRange
            leverText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Vérifier si le collider appartient à un objet avec le script Avatar et est un leader
        Avatar.Avatar avatar = other.GetComponent<Avatar.Avatar>();
        if (avatar != null)
        {
            isLeaderInRange = false; // Utilisation de isLeaderInRange
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
            
            if (audioSource && leverSound)
            {
                audioSource.PlayOneShot(leverSound);
            }   
        }
    }
}
