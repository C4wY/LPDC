using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Pics3 : MonoBehaviour
{
   public GameObject spriteToShow; // Sprite à faire apparaître
    public float moveDistance = 1f; // Distance à laquelle le sprite doit se déplacer vers le haut
    public float moveSpeed = 1f; // Vitesse de déplacement du sprite
    public AudioClip activationSound; // Son de l'activation de la plaque
        public GameObject Pics;

    private bool isPlayerOnPlate = false;
    private bool plateActivated = false;

    private AudioSource audioSource;

    private Vector3 initialSpritePosition;
    private Vector3 targetSpritePosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Stocker la position initiale du sprite
        initialSpritePosition = spriteToShow.transform.position;
        // Calculer la position cible du sprite (déplacement vers le haut)
        targetSpritePosition = initialSpritePosition + Vector3.up * moveDistance;
    }

    void Update()
    {
        if (isPlayerOnPlate && !plateActivated)
        {
            ActivatePlate();
        }

        // Déplacer progressivement le sprite vers le haut lorsque la plaque est activée
        if (plateActivated && spriteToShow != null && spriteToShow.transform.position != targetSpritePosition)
        {
            spriteToShow.transform.position = Vector3.MoveTowards(spriteToShow.transform.position, targetSpritePosition, moveSpeed * Time.deltaTime);
        }



        // Si le joueur est parti et que la plaque est activée, réinitialiser la position du sprite
        if (!isPlayerOnPlate && plateActivated && spriteToShow != null && spriteToShow.transform.position != initialSpritePosition)
        {
            spriteToShow.transform.position = Vector3.MoveTowards(spriteToShow.transform.position, initialSpritePosition, moveSpeed * Time.deltaTime);
            if (spriteToShow.transform.position == initialSpritePosition)
            {
                plateActivated = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pics.SetActive(true);
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
