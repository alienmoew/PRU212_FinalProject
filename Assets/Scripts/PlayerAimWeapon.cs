using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAimWeapon : MonoBehaviourPunCallbacks
{
    private Transform aimTransform;
    private Animator aimChildAnimator;

    private PhotonView view;
    private float aimAngle;
    private Vector3 aimlocalScale;
    private Vector3 aimDirection;

    //Bullet
    public GameObject bullet;
    public Transform[][] firePos;

    public float TimeBtwFire = 0.5f;
    public float bulletForce;

    private float timeBtwFire;

    private GameObject gunObject;
    private GameObject rifleObject;

    public float TimeBtwFireGun;
    public float TimeBtwFireRifle;

    private float timeBtwFireGun;
    private float timeBtwFireRifle;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");

        gunObject = aimTransform.Find("Visual/Gun").gameObject;
        rifleObject = aimTransform.Find("Visual/Rifle").gameObject;

        firePos = new Transform[2][];

        firePos[0] = new Transform[3]; // Súng bắn từ 3 vị trí
        firePos[0][0] = transform.Find("Aim/Visual/Gun/FirePos1");
        firePos[0][1] = transform.Find("Aim/Visual/Gun/FirePos2");
        firePos[0][2] = transform.Find("Aim/Visual/Gun/FirePos3");

        // Khởi tạo vị trí bắn cho Rifle
        firePos[1] = new Transform[1]; // Súng trường bắn từ 1 vị trí
        firePos[1][0] = transform.Find("Aim/Visual/Rifle/FirePos");

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

            // Xử lý chuyển đổi súng và súng trường
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetGunActive(true);
                SetRifleActive(false);
                view.RPC("SyncWeaponChange", RpcTarget.Others, true, false); // Gửi RPC khi chuyển sang súng
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetGunActive(false);
                SetRifleActive(true);
                view.RPC("SyncWeaponChange", RpcTarget.Others, false, true); // Gửi RPC khi chuyển sang súng trường
            }
        }
        else
        {
            UpdateAiming();
        }
    }

    private void SetGunActive(bool isActive)
    {
        gunObject.SetActive(isActive);
    }

    private void SetRifleActive(bool isActive)
    {
        rifleObject.SetActive(isActive);
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
        // Kiểm tra nếu người chơi nhấn chuột
        if (Input.GetMouseButton(0))
        {
            aimChildAnimator.SetTrigger("Shoot"); // Chạy animation bắn đạn
            view.RPC("PlayShootAnimation", RpcTarget.Others); // Đồng bộ hóa animation bắn đạn với người chơi khác

            // Xử lý bắn đạn cho từng loại súng
            if (gunObject.activeSelf)
            {
                timeBtwFireGun -= Time.deltaTime;
                if (timeBtwFireGun < 0)
                {
                    FireBullet(0); // Bắn với loại súng Gun (index 0)
                    timeBtwFireGun = TimeBtwFireGun;
                }
            }
            else if (rifleObject.activeSelf)
            {
                timeBtwFireRifle -= Time.deltaTime;
                if (timeBtwFireRifle < 0)
                {
                    FireBullet(1); // Bắn với loại súng Rifle (index 1)
                    timeBtwFireRifle = TimeBtwFireRifle;
                }
            }
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

    [PunRPC]
    private void SyncWeaponChange(bool gunActive, bool rifleActive)
    {
        SetGunActive(gunActive);
        SetRifleActive(rifleActive);
    }

    private void UpdateAiming()
    {
        aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);
        aimTransform.localScale = aimlocalScale;
    }

    private void FireBullet(int gunType)
    {
        timeBtwFire = TimeBtwFire;

        foreach (Transform pos in firePos[gunType])
        {
            GameObject bulletTmp = PhotonNetwork.Instantiate(bullet.name, pos.position, Quaternion.Euler(0, 0, aimAngle));

            Bullet bulletScript = bulletTmp.GetComponent<Bullet>();
            bulletScript.bulletForce = bulletForce;
        }
    }
}
