using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    public float verticalSpeed = 2f;
    public float verticalRange = 2f;
    public float attackSpeed = 5f;
    public int damageAmount = 1; // The amount of damage the enemy deals
    public float knockbackForce = 5f; // The force of the knockback effect
    public Transform player;
    private Vector3 initialPosition;
    private bool isAttacking = false;
    private bool hasStartedAttack = false; // To ensure attack is initiated correctly

    private Avatar.Santé playerHealth; // Reference to the player's health script
    private Rigidbody enemyRigidbody; // Reference to the enemy's Rigidbody
    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody

    void Start()
    {
        initialPosition = transform.position;

        // Find the player health component and rigidbodies
        if (player != null)
        {
            playerHealth = player.GetComponent<Avatar.Santé>();
            playerRigidbody = player.GetComponent<Rigidbody>(); // Assuming the player has a Rigidbody component
        }

        // Get the enemy's Rigidbody component
        enemyRigidbody = GetComponent<Rigidbody>();

        // Freeze the rotation and constrain movement on the Z-axis
        if (enemyRigidbody != null)
        {
            enemyRigidbody.freezeRotation = true;
            enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
    }

    void Update()
    {
        if (isAttacking)
        {
            RushAttack();
        }
        else
        {
            FlyUpDown();
        }
    }

    void FlyUpDown()
    {
        // Perform the up-and-down movement
        float newY = initialPosition.y + Mathf.Sin(Time.time * verticalSpeed) * verticalRange;
        transform.position = new Vector3(transform.position.x, newY, initialPosition.z);

        // Check if the enemy is in front of the player and within a closer range before initiating the attack
        float distanceToPlayer = Vector3.Distance(transform.position, new Vector3(player.position.x, player.position.y, initialPosition.z));

        if (distanceToPlayer < 3f && !hasStartedAttack) // Adjust the distance threshold as needed
        {
            isAttacking = true;
            hasStartedAttack = true;
        }
    }

    void RushAttack()
    {
        // Move towards the player in a straight line (diagonally if above or below)
        Vector3 direction = (new Vector3(player.position.x, player.position.y, initialPosition.z) - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.position.x, player.position.y, initialPosition.z), attackSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with the player
        if (collision.gameObject == player.gameObject)
        {
            if (playerHealth != null && playerHealth.PV > 0) // Ensure the player is still alive
            {
                playerHealth.FaireDégâts(damageAmount);

                // Apply knockback to the player immediately after dealing damage
                if (playerRigidbody != null)
                {
                    Vector3 knockbackDirection = (player.transform.position - transform.position).normalized;
                    playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                }

                // Make the enemy disappear immediately after dealing damage
                gameObject.SetActive(false);
            }
        }
        // Handle collision with the ground
        else if (collision.gameObject.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(player.position.x, player.position.y, initialPosition.z));
    }
}
