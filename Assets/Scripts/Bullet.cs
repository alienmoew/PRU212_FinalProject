using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    public float bulletForce;
    public float destroyTime = 1f;
    public int damage; // Add damage property
    public PhotonView owner; // Add owner property

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            rb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
            Invoke(nameof(DestroyBullet), destroyTime);
        }
    }

    private void DestroyBullet()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
        }
        else
        {
            rb.position = (Vector2)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();
        }
    }
}
