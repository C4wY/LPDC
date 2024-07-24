using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerdeSora : MonoBehaviour
{
    public float propulsionForce = 10f; // La force de propulsion
    public float propulsionAngle = 60f; // L'angle de propulsion en degrés
    public float cooldown = 3f; // Le délai de repropulsion en secondes

    private float lastPropulsionTime = -3f; // Le temps de la dernière propulsion

    void Update()
    {
        // Vérifie si la touche 'R' est appuyée et si le délai de repropulsion est écoulé
        if (Input.GetKeyDown(KeyCode.R) && Time.time >= lastPropulsionTime + cooldown)
        {
            PropulsePlayer();
            lastPropulsionTime = Time.time; // Met à jour le temps de la dernière propulsion
        }
    }

    void PropulsePlayer()
    {
        // Calcule la direction de propulsion à 60° sur l'axe X
        float angleInRadians = Mathf.Deg2Rad * propulsionAngle;
        Vector3 forward = transform.right; // Utiliser la direction de l'axe X

        Vector3 propulsionDirection = new Vector3(
            forward.x * Mathf.Cos(angleInRadians),
            forward.x * Mathf.Sin(angleInRadians),
            0 // Pas de propulsion sur l'axe Z
        ).normalized;

        // Applique la force de propulsion
        GetComponent<Rigidbody>().AddForce(propulsionDirection * propulsionForce, ForceMode.Impulse);

        GetComponent<Avatar.Avatar>().Move.enabled = false; // Désactive le mouvement du joueur

        StartCoroutine(EnablePlayerMovement()); // Réactive le mouvement du joueur après 0.5 secondes
    }

    IEnumerator EnablePlayerMovement()
    {
        yield return new WaitForSeconds(0.5f); // Attend 0.5 secondes

        GetComponent<Avatar.Avatar>().Move.enabled = true; // Réactive le mouvement du joueur
    }
}