using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class DisablePhysics : MonoBehaviour
{
    public GameObject player; // Assignez l'objet Player via l'inspecteur

    private Rigidbody playerRigidbody;
    private Collider playerCollider;
    private bool isPhysicsDisabled = false;
    private bool isCooldownActive = false;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        // Obtenez les composants Rigidbody et Collider du Player
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            playerCollider = player.GetComponent<Collider>();
        }
    }

    void Update()
    {
        // Déterminer la direction du déplacement
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
        {
            moveDirection = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveDirection = Vector3.right;
        }
        else
        {
            moveDirection = Vector3.zero; // Pas de mouvement si aucune touche n'est pressée
        }

        // Vérifiez si la touche A est pressée et que le joueur bouge sur l'axe X
        if (Input.GetKeyDown(KeyCode.A) && player != null)
        {
            if (isPhysicsDisabled)
            {
                // Si la physique est désactivée, réactivez-la immédiatement
                CancelInvoke("EnablePhysicsComponents");
                EnablePhysicsComponents();
            }
            else if (!isCooldownActive && moveDirection != Vector3.zero)
            {
                // Désactiver la gravité
                DisablePhysicsComponents();
                // Réactiver la gravité après 3 secondes
                Invoke("EnablePhysicsComponents", 2f);
            }
        }

        if (player != null && playerRigidbody != null && !playerRigidbody.useGravity && moveDirection != Vector3.zero)
        {
            // Déplacement du Player dans la direction spécifiée
            player.transform.position += moveDirection * Time.deltaTime * 20;
        }
    }

    void DisablePhysicsComponents()
    {
        // Désactiver la gravité
        if (playerRigidbody != null)
        {
            playerRigidbody.useGravity = false; // Désactive la gravité

            // Désactiver les déplacements sur les axes Y et Z
            playerRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        isPhysicsDisabled = true;
        isCooldownActive = true; // Démarre le cooldown
    }

    void EnablePhysicsComponents()
    {
        // Réactiver la gravité
        if (playerRigidbody != null)
        {
            playerRigidbody.useGravity = true; // Réactive la gravité

            // Réactiver les déplacements sur les axes Y et Z
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        isPhysicsDisabled = false;
        moveDirection = Vector3.zero; // Réinitialise la direction de déplacement

        // Démarrer le cooldown de 5 secondes avant de pouvoir désactiver la gravité à nouveau
        Invoke("ResetCooldown", 3f);
    }

    void ResetCooldown()
    {
        isCooldownActive = false; // Réinitialise le cooldown
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isPhysicsDisabled)
        {
            // Réactiver la gravité si le player entre en collision avec un autre objet
            CancelInvoke("EnablePhysicsComponents");
            EnablePhysicsComponents();
        }
    }
} 