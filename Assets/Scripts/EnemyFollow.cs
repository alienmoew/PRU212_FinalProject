using Photon.Pun;
using UnityEngine;

public class EnemyFollow : MonoBehaviourPunCallbacks
{
    public float speed = 5f;
    public float stoppingDistance = 1f;
    public float avoidanceRadius = 1.0f;
    public float avoidanceStrength = 0.5f;
    public float patrolSpeed = 2f;
    public float startWaitTime = 3f;
    public float followDistance = 10f;
    public float returnDistance = 15f;

    public int maxHealth = 100;

    private GameObject currentTarget;
    private bool isFollowingPlayer;
    private Vector2 spawnPosition;
    private float waitTime;
    private HealthSystem healthSystem;

    private PhotonView bulletOwner;

    private Vector2 randomPatrolPosition;
    private float patrolTimer;

    private void Start()
    {
        waitTime = startWaitTime;

        healthSystem = new HealthSystem(maxHealth);

        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Setup(healthSystem);
        }

        randomPatrolPosition = GetRandomPatrolPosition();
        patrolTimer = Random.Range(3f, 8f);
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
        float distanceThreshold = 0.2f; 

        if (Vector2.Distance(transform.position, randomPatrolPosition) < distanceThreshold)
        {
            patrolTimer = Random.Range(3f, 8f); 
            randomPatrolPosition = GetRandomPatrolPosition();
        }

        float currentSpeed = 3f;
        transform.position = Vector2.MoveTowards(transform.position, randomPatrolPosition, currentSpeed * Time.deltaTime);

        Vector2 moveDirection = (randomPatrolPosition - (Vector2)transform.position).normalized;
        FlipIfNeeded(moveDirection);

        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            patrolTimer = Random.Range(3f, 8f);
            randomPatrolPosition = GetRandomPatrolPosition(); 
        }
    }

    private Vector2 GetRandomPatrolPosition()
    {
        float minPatrolDistance = 5f; 
        float maxPatrolDistance = 10f; 

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 position = (Vector2)transform.position + randomDirection * Random.Range(minPatrolDistance, maxPatrolDistance);

        while (Vector2.Distance(position, (Vector2)transform.position) < minPatrolDistance)
        {
            randomDirection = Random.insideUnitCircle.normalized;
            position = (Vector2)transform.position + randomDirection * Random.Range(minPatrolDistance, maxPatrolDistance);
        }

        return position;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                bulletOwner = bullet.owner;
                TakeDamage(10);
                Destroy(other.gameObject);
            }
        }
    }

    private void Die()
    {
        if (bulletOwner != null && bulletOwner.IsMine)
        {
            PlayerAimWeapon playerScore = bulletOwner.GetComponent<PlayerAimWeapon>();
            if (playerScore != null)
            {
                playerScore.AddScore(10);
            }
        }

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
