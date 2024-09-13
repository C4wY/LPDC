using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPDC
{
    public class Attack : MonoBehaviour
    {
        public int damageAmount = 1;
        public float cooldown = 0.2f;
        public float hitDuration = 0.2f;
        public bool debug = false;

        Collider hitbox;
        float timeAttack = -1;
        Avatar avatar;
        Avatar Avatar =>
            avatar != null ? avatar : avatar = GetComponentInParent<Avatar>();

        void Start()
        {
            hitbox = GetComponent<Collider>();
        }

        bool WantsToAttack()
        {
            return Avatar.IsLeader && InputManager.Instance.LeaderAttack();
        }

        void DoAttack()
        {
            timeAttack = Time.time;
        }

        bool CanAttack()
        {
            return Time.time > timeAttack + cooldown;
        }

        bool IsAttacking()
        {
            return Time.time >= timeAttack && Time.time <= timeAttack + hitDuration;
        }

        void Update()
        {
            var direction = Avatar.Move.FacingDirection;
            transform.parent.localScale = new Vector3(direction, 1, 1);

            if (WantsToAttack() && CanAttack())
                DoAttack();

            hitbox.enabled = IsAttacking();

            // Debug visualization helper:
            foreach (var meshRenderer in GetComponents<MeshRenderer>())
                meshRenderer.enabled = debug && IsAttacking();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                var enemyHealth = other.GetComponent<EnemyHealth>();

                if (enemyHealth == null)
                    enemyHealth = other.GetComponentInParent<EnemyHealth>();

                if (enemyHealth != null)
                    enemyHealth.ApplyDamage(damageAmount);
            }
        }
    }

}