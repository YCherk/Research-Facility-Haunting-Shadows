using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float patrolRadius = 10.0f;
    public float patrolTimer = 5.0f;
    public float moveSpeed = 2.0f;
    public Transform player; // Reference to the player
    public float attackDistance = 2.0f; // Distance at which the enemy starts attacking
    public float sightDistance = 15.0f; // Distance at which the enemy can see the player
    public float fieldOfView = 60.0f; // Field of view for line of sight

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
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer(distanceToPlayer);

        if (canSeePlayer)
        {
            // Chase the player
            isChasingPlayer = true;
            agent.SetDestination(player.position);
            animator.SetBool("IsAttack", distanceToPlayer <= attackDistance);
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
}
