using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Pics3 : MonoBehaviour
{
    public GameObject spriteToShow; // Sprite à faire apparaître
    public GameObject teleportTarget; // Objet de référence pour la téléportation
    public float moveSpeed = 1f; // Vitesse de déplacement du sprite
    public AudioClip activationSound; // Son de l'activation de la plaque

    private bool isPlayerOnPlate = false;
    private bool plateActivated = false;
    private bool isTeleporting = false;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlayerOnPlate && !plateActivated)
        {
            ActivatePlate();
        }

        if (plateActivated && spriteToShow != null && !isTeleporting)
        {
            // Déplacer progressivement le sprite vers la position cible
            Vector3 targetPosition = teleportTarget.transform.position;
            spriteToShow.transform.position = Vector3.MoveTowards(spriteToShow.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Vérifier si le sprite a atteint la position cible pour terminer la téléportation
            if (spriteToShow.transform.position == targetPosition)
            {
                isTeleporting = true;
                plateActivated = false; // On désactive la plaque après la téléportation
                // Désactiver le mouvement du sprite une fois qu'il a atteint sa destination
                isTeleporting = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlate = false;
        }
    }

    private void ActivatePlate()
    {
        Debug.Log("La plaque de pression est activée !");
        plateActivated = true;

        // Jouer le son d'activation si spécifié
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
    }
}