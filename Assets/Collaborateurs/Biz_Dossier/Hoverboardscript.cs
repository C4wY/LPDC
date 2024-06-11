using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePhysics : MonoBehaviour
{
    public GameObject player; // Assignez l'objet player via l'inspecteur

    private Rigidbody playerRigidbody;
    private Collider playerCollider;

    void Start()
    {
        // Obtenez les composants Rigidbody et Collider de l'player
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            playerCollider = player.GetComponent<Collider>();
        }
    }

    void Update()
    {
        // Vérifiez si une touche est pressée (par exemple, la touche Espace)
        if (Input.GetKeyDown(KeyCode.A) && player != null)
        {
            // Désactivez la physique
            DisablePhysicsComponents();
            // Réactivez la physique après 5 secondes
            Invoke("EnablePhysicsComponents", 3f);
        }
        
         if (player != null && playerRigidbody != null && !playerRigidbody.useGravity)
         {
        // Déplacement de l'Avatar vers l'avant
        playerRigidbody.MovePosition(player.transform.position + Vector3.right * Time.deltaTime * 40);
         }
    }
    

 void DisablePhysicsComponents()
    {
        // Désactiver la gravité
        if (playerRigidbody != null)
        {
            playerRigidbody.useGravity = false; // Désactive la gravité
        }
    }

    void EnablePhysicsComponents()
    {
        // Réactiver la gravité
        if (playerRigidbody != null)
        {
            playerRigidbody.useGravity = true; // Réactive la gravité
        }
    }
}