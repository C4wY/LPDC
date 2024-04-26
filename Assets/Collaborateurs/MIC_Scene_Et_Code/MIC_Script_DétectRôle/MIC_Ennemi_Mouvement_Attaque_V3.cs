using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Importer l'espace de noms Avatar
using Avatar;

public class MIC_Ennemi_Mouvement_Attaque_V3 : MonoBehaviour
{
    // Référence à l'Avatar qui perd de la vie
    public Avatar.Avatar target;
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
    bool isAttacking = false;
    public GameObject attackENM;
    public SpriteRenderer ENMSprite; // Référence au SpriteRenderer de l'ennemi
    public AudioClip ENMAttack; //Son de l'attaque de l'ennemi
    public AudioSource AudioAtk; // Composant AudioSource attaché à l'ennemi

    void Start()
    {
        targetPoint = pointB; // Commence par se diriger vers le point B
        attackText = attackTextObject.GetComponent<Text>(); // Récupérer le composant Text du GameObject
        attackText.enabled = false; // Désactiver le texte d'attaque au démarrage
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = backgroundMusic;
        //Récupérer le composant AudioSource de cet objet
        AudioAtk = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Vérifier si l'Avatar est à portée de détection
        if (Vector3.Distance(transform.position, target.transform.position) <= detectionRange)
        {
            // L'Avatar est détecté, réinitialiser le compteur de temps
            timeSinceLastSeenPlayer = 0.0f;
            // Avatar détecté, jouer la musique

            // Initialiser le dernier temps d'attaque uniquement si l'Avatar vient d'être détecté
            if (Time.time - lastAttackTime <= attackInterval)
            {
                lastAttackTime = Time.time;
            }

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // Vérifier si l'Avatar est à portée d'attaque
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                // Attaquer l'Avatar
                Attack();
            }
            else
            {
                // Se déplacer vers l'Avatar
                MoveTowardsPlayer();
            }
        }
        else
        {
            // L'Avatar n'est pas détecté, incrémenter le compteur de temps
            timeSinceLastSeenPlayer += Time.deltaTime;
            // Avatar hors de portée, arrêter la musique
            StopMusic();
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // Si l'Avatar n'a pas été détecté depuis un certain temps, retourner aux allers-retours
            if (timeSinceLastSeenPlayer >= returnDelay)
            {
                ReturnToPatrol();
                attackTextObject.SetActive(false);
            }
            else
            {
                // Continuer les allers-retours
                MoveBetweenPoints();
                attackTextObject.SetActive(false);
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // Se déplacer vers l'Avatar
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = 0; // Ne pas bouger sur l'axe Y
        transform.Translate(direction * speed * Time.deltaTime);
        PlayMusic();
        // Activer l'objet contenant le texte d'attaque
        attackTextObject.SetActive(true);
    }

    void MoveBetweenPoints()
    {
        // Se déplacer vers le point cible
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        direction.y = 0; // Ne pas bouger sur l'axe Y
        transform.Translate(direction * speed * Time.deltaTime);

        // Changer de direction si l'ennemi est arrivé à destination
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Changer de point de destination
            if (targetPoint == pointA)
                targetPoint = pointB;
            else
                targetPoint = pointA;
            attackTextObject.SetActive(false);
        }
    }

    void ReturnToPatrol()
    {
        MoveBetweenPoints();
    }

    void Attack()
    {
        // Vérifier si l'ennemi n'est pas déjà en train d'attaquer
        if (!isAttacking)
        {
            // Vérifier si l'Avatar a un composant de santé
            Avatar.Santé targetHealth = target.GetComponent<Avatar.Santé>();
            if (targetHealth != null)
            {
                // Vérifier si l'Avatar a suffisamment de points de vie pour subir des dégâts
                if (targetHealth.PV > 0)
                {
                    // L'ennemi est en train d'attaquer
                    isAttacking = true;

                    ENMSprite.enabled = false;

                    // Activer l'anim d'attaque
                    attackENM.SetActive(true);

                    // Jouer le son de l'attaque
                    if (ENMAttack != null && AudioAtk != null)
                    {
                        AudioAtk.PlayOneShot(ENMAttack);
                    }

                    // Activer l'objet contenant le texte d'attaque
                    attackTextObject.SetActive(true);

                    // Infliger des dégâts à l'Avatar
                    targetHealth.FaireDégâts(dégâts);

                    // Réinitialiser l'attaque de l'ennemi après un certain délai
                    StartCoroutine(ResetAttack());
                }
            }
        }
    }

    IEnumerator ResetAttack()
    {
        // Attendre un certain délai avant de réinitialiser l'attaque de l'ennemi
        yield return new WaitForSeconds(attackInterval);
        attackTextObject.SetActive(false);
        attackENM.SetActive(false);
        // Réactiver le SpriteRenderer de l'ennemi
        ENMSprite.enabled = true;
        // Mettre à jour le dernier temps d'attaque
        lastAttackTime = Time.time;

        // Réinitialiser l'attaque de l'ennemi
        isAttacking = false;
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
