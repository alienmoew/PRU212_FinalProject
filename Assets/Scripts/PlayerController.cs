using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float moveSpeed;
    public float speedX;
    public float speedY;
    public Rigidbody2D rb;
    public Animator animator;

    private PhotonView view;
    private SpriteRenderer spriteRenderer;
    public bool IsIdle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (view.IsMine)
        {
            speedX = Input.GetAxisRaw("Horizontal") * moveSpeed;
            speedY = Input.GetAxisRaw("Vertical") * moveSpeed;
            rb.velocity = new Vector2(speedX, speedY);

            if (speedX == 0 && speedY == 0)
            {
                // Idle
                IsIdle = true;
                animator.SetBool("IsMoving", false);
            }
            else
            {
                // Moving
                IsIdle = false;
                animator.SetFloat("MoveX", speedX);
                animator.SetFloat("MoveY", speedY);
                animator.SetBool("IsMoving", true);

                // Flip the character
                if (speedX < 0)
                {
                    photonView.RPC("FlipCharacter", RpcTarget.All, false);
                }
                else if (speedX > 0)
                {
                    photonView.RPC("FlipCharacter", RpcTarget.All, true);
                }
            }
        }
    }

    [PunRPC]
    void FlipCharacter(bool flipX)
    {
        spriteRenderer.flipX = flipX;
    }
}
