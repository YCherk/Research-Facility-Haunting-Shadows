using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoint positions
    public int currentWaypointIndex = 0; // Current waypoint index
    public float moveSpeed = 2.0f; // Speed of the enemy movement
    public float gravity = -9.81f; // Gravity applied to the enemy

    private Animator animator; // Reference to the Animator component
    private CharacterController characterController; // Character controller for movement

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        characterController = GetComponent<CharacterController>(); // Get the CharacterController component


    }

    void Update()
    {
        MoveEnemy();
        UpdateAnimation();
    }

    void MoveEnemy()
    {
        if (waypoints.Length == 0) return;

        // Get the current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 targetDirection = (targetWaypoint.position - transform.position).normalized;

        // Adjust for slope and gravity
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            Vector3 groundNormal = hitInfo.normal;
            Vector3 projectedMoveDirection = Vector3.ProjectOnPlane(targetDirection, groundNormal);
            projectedMoveDirection *= moveSpeed;

            // Apply gravity
            projectedMoveDirection.y += gravity * Time.deltaTime;

            // Move the character controller
            characterController.Move(projectedMoveDirection * Time.deltaTime);
        }
        else
        {
            // Fallback if raycast does not hit the ground
            characterController.Move(targetDirection * moveSpeed * Time.deltaTime);
        }

        // Check if the enemy has reached the waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Move to the next waypoint
        }

        // Update the enemy's rotation to face the movement direction
        if (targetDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
        }
    }

    void UpdateAnimation()
    {
        // Calculate the local direction of movement
        Vector3 localDirection = transform.InverseTransformDirection(characterController.velocity).normalized;

        // Set the appropriate animation based on the movement direction
        animator.SetBool("WalkForward", localDirection.z > 0);
        animator.SetBool("WalkBackward", localDirection.z < 0);
        animator.SetBool("WalkRight", localDirection.x > 0);
        animator.SetBool("WalkLeft", localDirection.x < 0);
    }
}
