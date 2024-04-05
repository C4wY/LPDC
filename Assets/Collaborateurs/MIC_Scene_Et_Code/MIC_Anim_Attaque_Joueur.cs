using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Anim_Attaque_Joueur : MonoBehaviour
{
    public int dégâts = 1; // Points de dégâts
    public float distanceMaximale = 3f; // Distance maximale à partir de laquelle le joueur peut infliger des dégâts
    private SpriteRenderer spriteRenderer; // Référence au Sprite Renderer du joueur

    public Sprite attackSprite; // Image spécifique à afficher pendant l'attaque
    public float attackDuration = 0.5f; // Durée de l'attaque en secondes

    private SpriteRenderer playerSpriteRenderer; // Référence au SpriteRenderer du joueur
    private Sprite originalSprite; // Sprite d'origine du joueur

    void Start()
    {
       
        // Récupérer le SpriteRenderer de l'enfant spécifique (remplacez "Avatar (Leader)" par le nom de l'enfant contenant le SpriteRenderer)
        playerSpriteRenderer = transform.Find("Avatar (Leader)/Sprite").GetComponent<SpriteRenderer>();

        // Sauvegarder le sprite d'origine du joueur
        originalSprite = playerSpriteRenderer.sprite;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

            // Lancer la coroutine pour gérer l'attaque
            StartCoroutine(PerformAttack());
        // Rechercher tous les ennemis dans la scène avec le tag "Enemy"
        GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Enemy");

        // Vérifier si chaque ennemi est à portée et appuie sur la touche "A" pour infliger des dégâts
        foreach (GameObject ennemi in ennemis)
        {
            if (Vector3.Distance(transform.position, ennemi.transform.position) < distanceMaximale)
            {
                // Vérifier si le joueur est du côté droit de l'ennemi et si le Sprite Renderer n'est pas inversé horizontalement
                if (IsPlayerOnRightSide(ennemi) && spriteRenderer.flipX)
                {
                    // Infliger des dégâts à l'ennemi
                    Attack(ennemi);
                }
                // Vérifier si le joueur est du côté gauche de l'ennemi et si le Sprite Renderer est inversé horizontalement
                else if (!IsPlayerOnRightSide(ennemi) && !spriteRenderer.flipX)
                {
                    // Infliger des dégâts à l'ennemi
                    Attack(ennemi);
                }
                else
                {
                    Debug.Log("L'autre côté tête de noeud");
                }
            }
        }
            
        }
    }

    // Méthode pour infliger des dégâts à un ennemi
    private void Attack(GameObject ennemi)
    {
        ennemi.GetComponent<MIC_EnemyHealth>().FaireDégât(dégâts); // Appeler une méthode de l'ennemi pour lui infliger des dégâts
    }

    private bool IsSpriteFlipped()
    {
        return spriteRenderer.flipX;
    }

    // Méthode pour vérifier si le joueur est du côté droit d'un ennemi
    private bool IsPlayerOnRightSide(GameObject ennemi)
    {
        return transform.position.x > ennemi.transform.position.x;
    }

    IEnumerator PerformAttack()
    {
        // Changer le sprite du joueur pendant la durée de l'attaque
        playerSpriteRenderer.sprite = attackSprite;

        // Attendre pendant la durée de l'attaque
        yield return new WaitForSeconds(attackDuration);

        // Revenir au sprite d'origine du joueur
        playerSpriteRenderer.sprite = originalSprite;
    }
}
