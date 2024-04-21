using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MIC_Ennemi_Mouvement_Attaque_V2 : MonoBehaviour
{
    public GameObject target; // Référence au GameObject qui perd de la vie
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.0f;
    public float detectionRange = 5.0f;
    public float returnDelay = 10.0f; // Temps avant de retourner aux allers-retours après la perte de la cible
    public int dégâts = 1;
    public float attackInterval = 5.0f; // Intervalle entre chaque attaque en secondes
    public float attackRange = 1.5f;

    private Transform targetPoint;
    private float lastAttackTime = 0.0f;
    private float timeSinceLastSeenPlayer = 0.0f;
    public Text attackText; // Référence au texte d'attaque
    public GameObject attackTextObject; // Référence au GameObject contenant le texte d'attaque
    public AudioClip backgroundMusic; // Musique à jouer lorsque le joueur est détecté
    public AudioSource audioSource;

    void Start()
    {
        targetPoint = pointB; // Commence par se diriger vers le point B
        attackText = attackTextObject.GetComponent<Text>(); // Récupérer le composant Text du GameObject
        attackText.enabled = false; // Désactiver le texte d'attaque au démarrage
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = backgroundMusic;
    }

    void Update()
    {
        // Vérifier si le joueur est à portée de détection
        if (Vector3.Distance(transform.position, target.transform.position) <= detectionRange)
        {
            PlayMusic();
            // Le joueur est détecté, réinitialiser le compteur de temps
            timeSinceLastSeenPlayer = 0.0f;
            // Joueur détecté, jouer la musique
            

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
            // Joueur hors de portée, arrêter la musique
            StopMusic();

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
        MoveBetweenPoints();
    }

    void Attack()
{
    // Vérifier si le temps écoulé depuis la dernière attaque est supérieur à l'intervalle d'attaque
    if (Time.time - lastAttackTime >= attackInterval)
    {
        // Réinitialiser le compteur d'intervalle d'attaque à zéro
        lastAttackTime = Time.time;

        // Activer l'objet contenant le texte d'attaque
        attackTextObject.SetActive(true);

        // Vérifier si le joueur a un composant de santé
        Avatar.Santé targetHealth = target.GetComponent<Avatar.Santé>();
        if (targetHealth != null)
        {
            // Vérifier si le joueur a suffisamment de points de vie pour subir des dégâts
            if (targetHealth.PV > 0)
            {
                // Infliger des dégâts au joueur
                targetHealth.FaireDégâts(dégâts);
                attackTextObject.SetActive(false);
            }
        }
    }
}

void DisableAttackText()
{
    // Désactiver l'objet contenant le texte d'attaque
    attackTextObject.SetActive(false);
}
void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
