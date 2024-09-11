using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Importer l'espace de noms Avatar
using Avatar;

public class MIC_Ennemi_Mouvement_Attaque_V4 : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.0f;
    public float detectionRange = 5.0f;
    public float attackRange = 1.5f;
    public int damage = 1;

    private Avatar.Avatar leaderAvatar;
    private Transform targetPoint;
    private bool isAttacking = false;

    void Start()
    {
        // Trouver l'Avatar avec le rôle de Leader dans la scène
        leaderAvatar = FindLeaderAvatar();
        targetPoint = pointB;
    }

    void Update()
    {
        // Vérifier si l'Avatar Leader est à portée de détection
        if (leaderAvatar != null && Vector3.Distance(transform.position, leaderAvatar.transform.position) <= detectionRange)
        {
            // L'Avatar Leader est détecté, se déplacer vers lui
            MoveTowardsLeader();

            // Vérifier si l'Avatar Leader est à portée d'attaque
            if (Vector3.Distance(transform.position, leaderAvatar.transform.position) <= attackRange)
            {
                // Attaquer l'Avatar Leader
                AttackLeader();
            }
        }
        else
        {
            // L'Avatar Leader n'est pas détecté, reprendre le mouvement normal
            MoveBetweenPoints();
        }
    }

    void MoveTowardsLeader()
    {
        // Se déplacer vers l'Avatar Leader
        Vector3 direction = (leaderAvatar.transform.position - transform.position).normalized;
        direction.y = 0; // Ne pas bouger sur l'axe Y
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void MoveBetweenPoints()
    {
        // Se déplacer entre les points A et B
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
        }
    }

    void AttackLeader()
    {
        // Vérifier si l'ennemi n'est pas déjà en train d'attaquer
        if (!isAttacking)
        {
            // Infliger des dégâts à l'Avatar Leader
            Avatar.Santé leaderHealth = leaderAvatar.GetComponent<Avatar.Santé>();
            if (leaderHealth != null)
            {
                leaderHealth.FaireDégâts(damage);
                isAttacking = true;
                // Réinitialiser l'attaque après un certain délai
                StartCoroutine(ResetAttack());
            }
        }
    }

    IEnumerator ResetAttack()
    {
        // Attendre un certain délai avant de réinitialiser l'attaque de l'ennemi
        yield return new WaitForSeconds(1.0f); // Temps d'attaque
        isAttacking = false;
    }

    private Avatar.Avatar FindLeaderAvatar()
    {
        // Rechercher tous les objets dans la scène ayant le script Avatar et filtrer ceux avec le rôle Leader
        Avatar.Avatar[] avatars = FindObjectsByType<Avatar.Avatar>(FindObjectsSortMode.None);
        foreach (Avatar.Avatar avatar in avatars)
        {
            if (IsLeader(avatar.gameObject))
            {
                return avatar;
            }
        }
        return null;
    }

    private bool IsLeader(GameObject obj)
    {
        // Vérifier si l'objet a "(Leader)" dans son nom
        return obj.name.Contains("(Leader)");
    }
}
