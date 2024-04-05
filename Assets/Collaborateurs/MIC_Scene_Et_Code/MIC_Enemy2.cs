using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_Enemy2 : MonoBehaviour
{
    public int PV = 3; // Points de vie initiaux
    public int dégâts = 1; // Points de dégâts
    public float distanceMaximale = 5f; // Distance maximale à partir de laquelle le joueur peut infliger des dégâts
    private GameObject joueur; // Référence au joueur

    void Start()
    {
        joueur = GameObject.Find("Avatar (Leader)"); // Trouver le joueur dans la scène par son nom
        //joueur = GameObject.Find("Avatar (Follower)"); si intégré il doit avoir une distinction entre perso Front et Back si une modification de tag = aller chercher le tag pour le perso de front et le mettre à jour lors du Switch  //Le follower peut aussi "attaquer" s'il est porté, intégrer la condition pour incarner le perso Front pour sélectionner qui peut attaqué avec une condition
    }

    void Update()
    {
        // Vérifier si le joueur est à portée et appuie sur la touche "A" pour infliger des dégâts
        if (Vector3.Distance(transform.position, joueur.transform.position) < distanceMaximale && Input.GetKeyDown(KeyCode.A))
        {
            // Infliger des dégâts au GameObject
            FaireDégâts();
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
}

