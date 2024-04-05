using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Attaque_Joueur : MonoBehaviour
{
    public int dégâts = 1; // Points de dégâts
    public float distanceMaximale = 3f; // Distance maximale à partir de laquelle le joueur peut infliger des dégâts
    private SpriteRenderer spriteRenderer; // Référence au Sprite Renderer du joueur

    void Start()
    {
       
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();   
        
    }

    void Update()
    {
        // Rechercher tous les ennemis dans la scène avec le tag "Enemy"
        GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Enemy");

        // Vérifier si chaque ennemi est à portée et appuie sur la touche "A" pour infliger des dégâts
        foreach (GameObject ennemi in ennemis)
        {
            if (Vector3.Distance(transform.position, ennemi.transform.position) < distanceMaximale && Input.GetKeyDown(KeyCode.A))
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

    // Méthode pour infliger des dégâts à un ennemi
    private void Attack(GameObject ennemi)
    {
        ennemi.GetComponent<MIC_EnemyHealth>().TakeDamage(dégâts); // Appeler une méthode de l'ennemi pour lui infliger des dégâts
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
}
