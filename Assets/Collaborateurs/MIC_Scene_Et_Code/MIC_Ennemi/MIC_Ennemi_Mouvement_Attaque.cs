using System.Collections;
using System.Collections.Generic;
using Avatar;
using UnityEngine;
using UnityEngine.UI;

public class MIC_Ennemi_Mouvement_Attaque : MonoBehaviour
{
    public GameObject target; // Référence au GameObject qui perd de la vie
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.0f;
    public float detectionRange = 5.0f;
    public float returnDelay = 10.0f; // Temps avant de retourner aux allers-retours après la perte de la cible
    public int dégâts = 1;
    public float attackInterval = 1.0f; // Intervalle entre chaque attaque en secondes
    public float attackRange = 1.5f;

    private Transform targetPoint;
    private float lastAttackTime = 0.0f;
    private float timeSinceLastSeenPlayer = 0.0f;
    public Text attackText; // Référence au texte d'attaque
    public GameObject attackTextObject; // Référence au GameObject contenant le texte d'attaque
    HashSet<Avatar.Avatar> avatarsEnContact = new();

    void Start()
    {
        targetPoint = pointB; // Commence par se diriger vers le point B
        attackText = attackTextObject.GetComponent<Text>(); // Récupérer le composant Text du GameObject
        attackText.enabled = false; // Désactiver le texte d'attaque au démarrage

    }

    void Update()
    {
        if (targetPoint == pointB && Vector3.Distance(transform.position, pointB.position) < 0.1f)
        {
            // L'ennemi est bloqué au point B, réinitialiser le comportement
            ReturnToPatrol();
        }
        // Vérifier si le joueur est à portée de détection
        if (Vector3.Distance(transform.position, target.transform.position) <= detectionRange)
        {
            // Le joueur est détecté, réinitialiser le compteur de temps
            timeSinceLastSeenPlayer = 0.0f;

            // Vérifier si le joueur est à portée d'attaque
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                // Attaquer le joueur
                Attack();
            }
            else
            {
                // Se déplacer vers le joueur
                MoveTowardsPlayer();
            }
        }
        else
        {
            // Le joueur n'est pas détecté, incrémenter le compteur de temps
            timeSinceLastSeenPlayer += Time.deltaTime;

            // Si le joueur n'a pas été détecté depuis un certain temps, retourner aux allers-retours
            if (timeSinceLastSeenPlayer >= returnDelay)
            {
                ReturnToPatrol();
            }
            else
            {
                // Continuer les allers-retours
                MoveBetweenPoints();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // Se déplacer vers le joueur
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void MoveBetweenPoints()
    {
        // Se déplacer vers le point cible
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        // Changer de direction si l'ennemi est arrivé à destination
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Changer de point de destination
            if (targetPoint == pointA)
                targetPoint = pointB;
            else
                targetPoint = pointA;
        }
    }

    void ReturnToPatrol()
    {
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Changer de point de destination
            if (targetPoint == pointA)
                targetPoint = pointB;
            else
                targetPoint = pointA;
        }
        MoveBetweenPoints();
    }

    void Attack()
    {
        // Vérifier si le joueur a un composant de santé
        Santé targetHealth = target.GetComponent<Santé>();
        if (targetHealth != null)
        {
            // Infliger des dégâts au joueur
            targetHealth.FaireDégâts(dégâts);

            // Afficher le texte d'attaque
            attackText.enabled = true;
            attackText.text = "Ennemi attaque!";
            // Désactiver le texte d'attaque après un court délai
            Invoke("DisableAttackText", 1.0f);
        }

        // Mettre à jour le temps de la dernière attaque
        lastAttackTime = Time.time;
    }
    void DisableAttackText()
    {
        // Désactiver le texte d'attaque
        attackText.enabled = false;
    }

    //Réparation pour une attaque à la collision entre l'ennemi et le joueur
    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<Avatar.Avatar>();
            if (avatarsEnContact.Contains(avatar) == false)
            {
                avatar.Santé.FaireDégâts(dégâts);
                avatarsEnContact.Add(avatar);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            var avatar = other.attachedRigidbody.GetComponent<Avatar.Avatar>();
            if (avatarsEnContact.Contains(avatar) == true)
            {
                avatarsEnContact.Remove(avatar);
            }
        }
    }
}