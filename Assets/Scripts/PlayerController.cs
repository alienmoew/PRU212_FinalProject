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
    private Vector2 lastSentVelocity; // Lưu trữ velocity gửi lần cuối

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

            // Kiểm tra nếu velocity đã thay đổi từ lần gửi trước đó
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
}
