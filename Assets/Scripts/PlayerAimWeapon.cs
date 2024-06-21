using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAimWeapon : MonoBehaviour
{
    private Transform aimTransform;
    private Animator aimChildAnimator;

    private PhotonView view;
    private float aimAngle;
    private Vector3 aimlocalScale;
    private Vector3 aimDirection;

    //Bullet
    public GameObject bullet;
    public Transform firePos;
    public float TimeBtwFire = 0.2f;
    public float bulletForce;

    private float timeBtwFire;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        aimChildAnimator = aimTransform.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (view.IsMine)
        {
            HandleAiming();
            HandleShooting();
        }
        else
        {
            UpdateAiming();
        }
    }

    private void HandleAiming()
    {
        Vector3 mousePosition = MouseUtils.GetMouseWorldPosition2D();

        Vector3 direction = (mousePosition - transform.position);
        aimDirection = direction.normalized; 
        aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);

        aimlocalScale = Vector3.one;
        if (aimAngle > 90 || aimAngle < -90)
        {
            aimlocalScale.y = -1f;
        }
        else
        {
            aimlocalScale.y = +1f;
        }
        aimTransform.localScale = aimlocalScale;

        view.RPC("SyncAiming", RpcTarget.Others, aimAngle, aimlocalScale);
    }

    private void HandleShooting()
    {
        timeBtwFire -= Time.deltaTime;
        if (Input.GetMouseButton(0) && timeBtwFire < 0)
        {
            aimChildAnimator.SetTrigger("Shoot");
            view.RPC("PlayShootAnimation", RpcTarget.Others);
            FireBullet();
        }
    }

    [PunRPC]
    private void PlayShootAnimation()
    {
        aimChildAnimator.SetTrigger("Shoot");
    }

    [PunRPC]
    private void SyncAiming(float angle, Vector3 localScale)
    {
        aimAngle = angle;
        aimlocalScale = localScale;
        UpdateAiming();
    }

    private void UpdateAiming()
    {
        aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);
        aimTransform.localScale = aimlocalScale;
    }

    private void FireBullet()
    {
        timeBtwFire = TimeBtwFire;

        GameObject bulletTmp = PhotonNetwork.Instantiate(bullet.name, firePos.position, Quaternion.Euler(0, 0, aimAngle));

        Bullet bulletScript = bulletTmp.GetComponent<Bullet>();
        bulletScript.bulletForce = bulletForce;
    }
}
