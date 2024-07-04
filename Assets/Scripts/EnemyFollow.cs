using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class EnemyFollow : MonoBehaviourPunCallbacks
{
    public float speed = 5f;
    public float stoppingDistance = 1f;
    public float avoidanceRadius = 1.0f;
    public float avoidanceStrength = 0.5f;
    public float patrolSpeed = 2f;
    public float startWaitTime = 3f;
    public List<Transform> moveSpots;
    public float followDistance = 10f;
    public float returnDistance = 15f;

    public int maxHealth = 100;

    private GameObject currentTarget;
    private bool isFollowingPlayer;
    private int currentSpotIndex;
    private float waitTime;
    private HealthSystem healthSystem;

    private void Start()
    {
        waitTime = startWaitTime;
        currentSpotIndex = 0;

        healthSystem = new HealthSystem(maxHealth);

        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Setup(healthSystem);
        }
    }

    private void Update()
    {
        FindClosestPlayer();

        if (currentTarget != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, currentTarget.transform.position);

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

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = followDistance;
        GameObject closestPlayer = null;

        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            currentTarget = closestPlayer;
        }
    }

    private void FollowPlayer()
    {
        if (currentTarget == null)
        {
            isFollowingPlayer = false;
            return;
        }

        Vector2 targetPosition = currentTarget.transform.position;
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

    public void TakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        healthSystem.Damage(damage);

        if (healthSystem.GetHealth() <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10);
            Destroy(other.gameObject);
        }
    }
}
