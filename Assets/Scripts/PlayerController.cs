using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float moveSpeed;
    public Rigidbody2D rb;
    public Animator animator;

    private PhotonView view;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastSentVelocity;

    private Transform aimTransform;
    private Animator aimChildAnimator;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        aimChildAnimator = aimTransform.GetComponentInChildren<Animator>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    private void Update()
    {
        if(view.IsMine)
        {
            HandleAiming();
            HandleShooting();
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

    private void HandleAiming()
    {
        Vector3 mousePosition = MouseUtils.GetMouseWorldPosition2D();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 aimlocalScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            aimlocalScale.y = -1f;
        }
        else
        {
            aimlocalScale.y = +1f;
        }
        aimTransform.localScale = aimlocalScale;
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = MouseUtils.GetMouseWorldPosition2D();
            aimChildAnimator.SetTrigger("Shoot");
        }
    }
}