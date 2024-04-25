using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_GmO_Sprite_attaque : MonoBehaviour
{
    public GameObject spriteToDisable; // Référence au GameObject du sprite à désactiver pendant l'attaque
    public GameObject attackSpriteObject; // Référence au GameObject du sprite d'attaque
    public float attackDuration = 1f; // Durée de l'attaque en secondes

    private SpriteRenderer spriteRenderer; // Référence au SpriteRenderer à désactiver
    private SpriteRenderer attackSpriteRenderer; // Référence au SpriteRenderer du sprite d'attaque

    void Start()
    {
        // Récupérer le SpriteRenderer du sprite à désactiver
        spriteRenderer = spriteToDisable.GetComponent<SpriteRenderer>();

        // Récupérer le SpriteRenderer du sprite d'attaque
        attackSpriteRenderer = attackSpriteObject.GetComponent<SpriteRenderer>();

        // Désactiver le SpriteRenderer du sprite d'attaque au démarrage
        attackSpriteRenderer.enabled = false;
    }

    void Update()
    {
        // Vérifier si le joueur appuie sur la touche d'attaque (à ajuster selon vos besoins)
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Lancer l'attaque si le sprite n'est pas déjà désactivé
            if (spriteRenderer.enabled)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    IEnumerator PerformAttack()
    {
        // Sauvegarder l'état du flip sur l'axe X du SpriteRenderer à désactiver
        bool isFlipped = spriteRenderer.flipX;

        // Désactiver le SpriteRenderer du sprite pendant l'attaque
        spriteRenderer.enabled = false;

        // Activer le SpriteRenderer du sprite d'attaque pendant l'attaque
        attackSpriteRenderer.enabled = true;

        // Appliquer le flip sur l'axe X au SpriteRenderer de l'attaque si nécessaire
        attackSpriteRenderer.flipX = isFlipped;

        // Attendre pendant la durée de l'attaque
        yield return new WaitForSeconds(attackDuration);

        // Désactiver le SpriteRenderer du sprite d'attaque après l'attaque
        attackSpriteRenderer.enabled = false;

        // Réactiver le SpriteRenderer du sprite après l'attaque
        spriteRenderer.enabled = true;
    }
}