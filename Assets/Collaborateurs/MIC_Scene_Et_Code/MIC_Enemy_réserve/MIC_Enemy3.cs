using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Enemy3 : MonoBehaviour
{
    public int PV = 3; // Points de vie initiaux
    public int dégâts = 1; // Points de dégâts
    public float distanceMaximale = 5f; // Distance maximale à partir de laquelle le joueur peut infliger des dégâts
    private GameObject joueur; // Référence au joueur
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        joueur = GameObject.Find("Avatar (Leader)");
        if (joueur != null)
        {
        spriteRenderer = joueur.transform.Find("Sprite").GetComponent<SpriteRenderer>();   
        }

 //Ne marche pas pour la détection
    }

    void Update()
    {
               GameObject[] joueurs = GameObject.FindGameObjectsWithTag("Player");
               foreach (GameObject joueur in joueurs)
               {
        // Vérifier si le joueur est à portée et appuie sur la touche "A" pour infliger des dégâts
        if (Vector3.Distance(transform.position, joueur.transform.position) < distanceMaximale && Input.GetKeyDown(KeyCode.A)) 
        {
            if (!IsSpriteFlipped())
            {
               FaireDégâts();
            break; 
            }
            else
            {
            // Infliger des dégâts au GameObject
            
            Debug.Log("L'autre côté tête de noeud");
            }

        }
               }

    }

    // Méthode pour recevoir des dégâts du joueur
    public void FaireDégâts()
    {
        if (PV > 0)
        {
            PV -= dégâts; // Retirer les points de dégâts des points de vie
            Debug.Log("Les points de vie ont diminué. PV restant : " + PV);

            if (PV <= 0)
            {
                Debug.Log("L'objet est cassé !");
                // Mettre ici tout code lié à la destruction de l'objet
                Destroy(gameObject); // Détruire l'objet lorsque les PV tombent à zéro ou moins
            }
        }
    }
    private bool IsSpriteFlipped()
    {
        return spriteRenderer.flipX;
    }
}

