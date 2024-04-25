using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_JGmO_Sprite_Dégât : MonoBehaviour
{
    public GameObject spriteToDisable; // Référence au GameObject du sprite à désactiver pendant l'attaque
    public GameObject DégâtsGmOJoueur; // Référence au GameObject du sprite d'attaque
    public float DuréeAnimdégâts = 1f; // Durée de l'attaque en secondes

    private SpriteRenderer spriteRenderer; // Référence au SpriteRenderer à désactiver
    private SpriteRenderer DégâtsSpriteJoueur; // Référence au SpriteRenderer du sprite d'attaque

    // Référence au script de santé du joueur
    private Avatar.Santé santéDuJoueur;

    void Start()
    {
        // Récupérer le SpriteRenderer du sprite à désactiver
        spriteRenderer = spriteToDisable.GetComponent<SpriteRenderer>();

        // Récupérer le SpriteRenderer du sprite d'attaque
        DégâtsSpriteJoueur = DégâtsGmOJoueur.GetComponent<SpriteRenderer>();

        // Désactiver le SpriteRenderer du sprite d'attaque au démarrage
        DégâtsSpriteJoueur.enabled = false;

        // Récupérer le script de santé du joueur
        santéDuJoueur = transform.parent.GetComponent<Avatar.Santé>();
    }

    // Méthode appelée lorsqu'un joueur prend des dégâts
    public void JoueurPrisDégâts(int dégâts)
    {
        // Lancer l'attaque si le sprite n'est pas déjà désactivé
        if (spriteRenderer.enabled)
        {
            StartCoroutine(AnimPriseDégâts());
        }
    }

    IEnumerator AnimPriseDégâts()
    {
        // Sauvegarder l'état du flip sur l'axe X du SpriteRenderer à désactiver
        bool isFlipped = spriteRenderer.flipX;

        // Désactiver le SpriteRenderer du sprite pendant l'attaque
        spriteRenderer.enabled = false;

        // Activer le SpriteRenderer du sprite d'attaque pendant l'attaque
        DégâtsSpriteJoueur.enabled = true;

        // Appliquer le flip sur l'axe X au SpriteRenderer de l'attaque si nécessaire
        DégâtsSpriteJoueur.flipX = isFlipped;

        // Attendre pendant la durée de l'attaque
        yield return new WaitForSeconds(DuréeAnimdégâts);

        // Désactiver le SpriteRenderer du sprite d'attaque après l'attaque
        DégâtsSpriteJoueur.enabled = false;

        // Réactiver le SpriteRenderer du sprite après l'attaque
        spriteRenderer.enabled = true;
    }
}