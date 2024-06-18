using UnityEngine;
using System.Collections.Generic;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 5f;
    public float stoppingDistance = 1f;
    public float avoidanceRadius = 1.0f;
    public float avoidanceStrength = 0.5f;
    public float patrolSpeed = 2f;
    public float startWaitTime = 3f;
    public List<Transform> moveSpots;

    private GameObject player;
    private bool isFollowingPlayer;
    private int currentSpotIndex;
    private float waitTime;

    public float followDistance = 10f;
    public float returnDistance = 15f;


    private void Start()
    {
        waitTime = startWaitTime;
        currentSpotIndex = 0;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (player != null && player.CompareTag("Player"))
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= followDistance)
            {
                isFollowingPlayer = true;
            }
            else if (distanceToPlayer > returnDistance)
            {
                isFollowingPlayer = false;
            }
        }
        else
        {
            isFollowingPlayer = false;
        }

        if (isFollowingPlayer)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void FollowPlayer()
    {
        if (player == null)
        {
            isFollowingPlayer = false;
            return;
        }

        Vector2 targetPosition = player.transform.position;
        Vector2 currentPosition = transform.position;
        Vector2 direction = targetPosition - currentPosition;
        float distanceToPlayer = direction.magnitude;

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
        float currentSpeed = speed;

        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(currentPosition, currentPosition + moveDirection + avoidanceForce * avoidanceStrength, currentSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(currentPosition, currentPosition + avoidanceForce * avoidanceStrength, currentSpeed * Time.deltaTime);
        }

        FlipIfNeeded(moveDirection);
    }

    private void Patrol()
    {
        if (moveSpots.Count == 0)
        {
            return;
        }

        Transform targetSpot = moveSpots[currentSpotIndex];

        if (Vector2.Distance(transform.position, targetSpot.position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                currentSpotIndex = (currentSpotIndex + 1) % moveSpots.Count;
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        else
        {
            float currentSpeed = patrolSpeed;
            transform.position = Vector2.MoveTowards(transform.position, targetSpot.position, currentSpeed * Time.deltaTime);
        }

        FlipIfNeeded(targetSpot.position - transform.position);
    }

    private void FlipIfNeeded(Vector2 moveDirection)
    {
        if (moveDirection.x > 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDirection.x < 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
