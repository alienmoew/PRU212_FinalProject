using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;
    public float avoidanceRadius = 1.0f;  // Radius within which enemies will avoid each other
    public float avoidanceStrength = 0.5f;  // How strong the avoidance force should be
    public Transform[] patrolPoints;  // Array of patrol points
    private int currentPatrolIndex = 0;

    private Transform player;

    void Start()
    {
        FindNearestPlayer();
    }

    void Update()
    {
        FindNearestPlayer();

        if (player == null)
        {
            Patrol();
            return;
        }

        Vector2 targetPosition = player.position;
        Vector2 currentPosition = transform.position;

        // Calculate the direction towards the player
        Vector2 direction = targetPosition - currentPosition;
        float distanceToPlayer = direction.magnitude;

        // Apply avoidance from other enemies
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
        Vector2 avoidanceForce = Vector2.zero;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject && hitCollider.CompareTag("Enemy"))
            {
                Vector2 directionToCollider = (Vector2)hitCollider.transform.position - currentPosition;
                avoidanceForce -= directionToCollider.normalized / directionToCollider.magnitude;
            }
        }

        Vector2 moveDirection = direction.normalized;

        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(currentPosition, currentPosition + moveDirection + avoidanceForce * avoidanceStrength, speed * Time.deltaTime);
        }
        else if (distanceToPlayer < stoppingDistance && distanceToPlayer > retreatDistance)
        {
            // Enemy stays in place, apply only avoidance force
            transform.position = Vector2.MoveTowards(currentPosition, currentPosition + avoidanceForce * avoidanceStrength, speed * Time.deltaTime);
        }
        else if (distanceToPlayer < retreatDistance)
        {
            transform.position = Vector2.MoveTowards(currentPosition, currentPosition - moveDirection + avoidanceForce * avoidanceStrength, speed * Time.deltaTime);
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = patrolTarget.position;

        // Move towards the patrol point
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        // Check if the enemy reached the patrol point
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            // Move to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float nearestDistance = Mathf.Infinity;
        Transform nearestPlayer = null;

        foreach (GameObject playerObject in players)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);
            if (distanceToPlayer < nearestDistance)
            {
                nearestDistance = distanceToPlayer;
                nearestPlayer = playerObject.transform;
            }
        }

        player = nearestPlayer;
    }

    void OnDrawGizmosSelected()
    {
        // Draw the avoidance radius in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
    }
}
