using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Sprite_Attaque : MonoBehaviour
{
     public Sprite attackSprite; // Image spécifique à afficher pendant l'attaque
    public float attackDuration = 1f; // Durée totale de l'attaque en secondes
    public int numFrames = 10; // Nombre de frames pour l'attaque

    private SpriteRenderer playerSpriteRenderer; // Référence au SpriteRenderer du joueur
    private Sprite originalSprite; // Sprite d'origine du joueur

    void Start()
    {
        // Récupérer le SpriteRenderer du GameObject "Sprite"
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        // Sauvegarder le sprite d'origine du joueur
        originalSprite = playerSpriteRenderer.sprite;
    }

    void Update()
    {
        // Vérifier si le joueur appuie sur la touche d'attaque (à ajuster selon vos besoins)
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Lancer la coroutine pour gérer l'attaque
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        // Sauvegarder le sprite d'origine du joueur
        Sprite originalSprite = playerSpriteRenderer.sprite;

        // Changer le sprite du joueur pendant l'attaque
        playerSpriteRenderer.sprite = attackSprite;

        // Calculer la durée d'une frame
        float frameDuration = attackDuration / numFrames;

        // Attendre pendant chaque frame de l'attaque
        for (int i = 0; i < numFrames; i++)
        {
            // Attendre une frame
            yield return new WaitForSeconds(frameDuration);

            // Réinitialiser le SpriteRenderer avec le sprite d'attaque
            playerSpriteRenderer.sprite = attackSprite;
        }

        // Réinitialiser le SpriteRenderer avec le sprite d'origine après l'attaque
        playerSpriteRenderer.sprite = originalSprite;
    }
}