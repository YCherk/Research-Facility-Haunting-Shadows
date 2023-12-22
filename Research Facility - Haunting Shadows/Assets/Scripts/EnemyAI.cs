using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float patrolRadius = 10.0f;
    public float patrolTimer = 5.0f;
    public float moveSpeed = 2.0f;
    public Transform player; // Reference to the player
    public float attackDistance = 2.0f; // Distance at which the enemy starts attacking
    public float sightDistance = 15.0f; // Distance at which the enemy can see the player in front
    public float backSightDistance = 5.0f; // Distance at which the enemy can see the player from behind
    public float fieldOfView = 60.0f; // Field of view for line of sight in front
    public float backFieldOfView = 90.0f; // Field of view for line of sight from behind

    private float timer;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasingPlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = patrolTimer;

        agent.speed = moveSpeed;
        agent.updateRotation = false; // Disable automatic rotation
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer(distanceToPlayer);
        bool canSeePlayerFromBehind = CanSeePlayerFromBehind(distanceToPlayer);

        if (canSeePlayer || canSeePlayerFromBehind)
        {
            // Chase the player
            isChasingPlayer = true;
            agent.SetDestination(player.position);

            // Trigger attack animation if within attack distance
            if (distanceToPlayer <= attackDistance)
            {
                animator.SetTrigger("IsAttack");
            }
            else
            {
                animator.SetBool("IsAttack", false);
            }

            // Turn towards the player if detected from behind
            if (canSeePlayerFromBehind)
            {
                TurnTowardsPlayer();
            }
        }
        else if (isChasingPlayer)
        {
            // Stop chasing if player is no longer visible
            isChasingPlayer = false;
            animator.SetBool("IsAttack", false);
        }
        else
        {
            // Random Patrol
            Patrol();
        }

        // Set isIdle based on whether the enemy is moving or not
        animator.SetBool("isIdle", agent.velocity.magnitude < 0.01f);

        // Update the enemy's rotation to face the movement direction
        if (agent.velocity.magnitude > 0.01f && !canSeePlayerFromBehind)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
    }

    public void AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(30);
        }
    }

    bool CanSeePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= sightDistance)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle <= fieldOfView / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightDistance))
                {
                    return hit.transform == player;
                }
            }
        }
        return false;
    }
    bool CanSeePlayerFromBehind(float distanceToPlayer)
    {
        if (distanceToPlayer <= backSightDistance)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(-transform.forward, directionToPlayer); // Check angle with the back direction
            if (angle <= backFieldOfView / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, backSightDistance))
                {
                    return hit.transform == player;
                }
            }
        }
        return false;
    }
    void Patrol()
    {
        timer += Time.deltaTime;

        if (timer >= patrolTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, patrolRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        // Update the walking animation based on velocity
        animator.SetBool("WalkForward", agent.velocity.magnitude > 0.01f);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    private void TurnTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }
}
