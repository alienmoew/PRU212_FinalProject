using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    private PhotonView view;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastSentVelocity;

    private HealthSystem healthSystem;

    public int maxHealth = 100;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        healthSystem = new HealthSystem(maxHealth);

        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Setup(healthSystem);
        }
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetMouseButtonDown(1))
            {
                healthSystem.Damage(10);
            }
            if (Input.GetMouseButtonDown(2))
            {
                healthSystem.Heal(10);
            }
        }
    }

    void FixedUpdate()
    {
        if (view.IsMine)
        {
            float speedX = Input.GetAxisRaw("Horizontal");
            float speedY = Input.GetAxisRaw("Vertical");

            Vector2 velocity = new Vector2(speedX, speedY).normalized * moveSpeed;
            rb.velocity = velocity;

            if (velocity != lastSentVelocity)
            {
                photonView.RPC("SyncMovement", RpcTarget.Others, velocity);
                lastSentVelocity = velocity;
            }

            // Xử lý animation
            animator.SetFloat("MoveX", velocity.x);
            animator.SetFloat("MoveY", velocity.y);
            animator.SetBool("IsMoving", velocity.magnitude > 0);

            // Flip character
            if (velocity.x != 0)
            {
                photonView.RPC("FlipCharacter", RpcTarget.All, velocity.x > 0);
            }
        }
    }

    [PunRPC]
    void SyncMovement(Vector2 velocity)
    {
        rb.velocity = velocity;
        animator.SetFloat("MoveX", velocity.x);
        animator.SetFloat("MoveY", velocity.y);
        animator.SetBool("IsMoving", velocity.magnitude > 0);
    }

    [PunRPC]
    void FlipCharacter(bool flipX)
    {
        spriteRenderer.flipX = flipX;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
    }
}