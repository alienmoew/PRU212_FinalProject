using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerAimWeapon : MonoBehaviourPunCallbacks
{
    private Transform aimTransform;
    private Animator aimChildAnimator;

    private PhotonView view;
    private float aimAngle;
    private Vector3 aimlocalScale;
    private Vector3 aimDirection;

    // Bullet
    public GameObject bullet;
    public Transform[][] firePos;

    public float TimeBtwFire;
    public float bulletForce;

    private float timeBtwFire;

    private GameObject gunObject;
    private GameObject rifleObject;

    public float TimeBtwFireGun;
    public float TimeBtwFireRifle;

    private float timeBtwFireGun;
    private float timeBtwFireRifle;

    private HealthSystem healthSystem;
    public int maxHealth = 100;

    // Score System
    private int score = 0;

    // Buff thresholds
    private bool hasReached50Points = false;
    private bool hasReached100Points = false;
    private bool hasReached150Points = false;
    private bool hasReached300Points = false;


    private void Awake()
    {
        aimTransform = transform.Find("Aim");

        gunObject = aimTransform.Find("Visual/Gun").gameObject;
        rifleObject = aimTransform.Find("Visual/Rifle").gameObject;

        firePos = new Transform[2][];

        firePos[0] = new Transform[1];
        firePos[0][0] = transform.Find("Aim/Visual/Gun/FirePos");

        firePos[1] = new Transform[1];
        firePos[1][0] = transform.Find("Aim/Visual/Rifle/FirePos");

        aimChildAnimator = aimTransform.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        view = GetComponent<PhotonView>();

        healthSystem = new HealthSystem(maxHealth);

        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Setup(healthSystem);
        }

        UpdateScoreText();
    }

 private void Update()
    {
        if (view.IsMine)
        {
            HandleAiming();
            HandleShooting();
            HandleWeaponSwitching();

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                UIManager.Instance.ShowWinPanel();
            }
        }
        else
        {
            UpdateAiming();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.ShowMenuPanel();
        }
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetGunActive(true);
            SetRifleActive(false);
            view.RPC("SyncWeaponChange", RpcTarget.Others, true, false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetGunActive(false);
            SetRifleActive(true);
            view.RPC("SyncWeaponChange", RpcTarget.Others, false, true);
        }
    }


    private void SetGunActive(bool isActive)
    {
        gunObject.SetActive(isActive);
        if (isActive) UpdateAnimator();
    }

    private void SetRifleActive(bool isActive)
    {
        rifleObject.SetActive(isActive);
        if (isActive) UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (gunObject.activeSelf)
        {
            aimChildAnimator = gunObject.GetComponentInChildren<Animator>();
        }
        else if (rifleObject.activeSelf)
        {
            aimChildAnimator = rifleObject.GetComponentInChildren<Animator>();
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
        if (Input.GetMouseButton(0))
        {
           

            if (gunObject.activeSelf)
            {
                timeBtwFireGun -= Time.deltaTime;
                if (timeBtwFireGun < 0)
                {
                    aimChildAnimator.SetTrigger("Shoot");
                    view.RPC("PlayShootAnimation", RpcTarget.Others);
                    FireBullet(0);
                    timeBtwFireGun = TimeBtwFireGun;
                }
            }
            else if (rifleObject.activeSelf)
            {
                timeBtwFireRifle -= Time.deltaTime;
                if (timeBtwFireRifle < 0)
                {
                    aimChildAnimator.SetTrigger("Shoot");
                    view.RPC("PlayShootAnimation", RpcTarget.Others);
                    FireBullet(1);
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

            if (bulletScript != null)
            {
                bulletScript.owner = photonView;

                if (gunType == 0) // Gun
                {
                    bulletScript.damage = 5;
                }
                else if (gunType == 1) // Rifle
                {
                    bulletScript.damage = 1;
                }
            }

            bulletScript.bulletForce = bulletForce;
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
            UIManager.Instance.ShowDeathPanel();
            PhotonNetwork.Destroy(gameObject);
            //SceneManager.LoadScene("Lobby"); // This can be handled from the UI panel button
        }
    }

    public void TakeHealth(int health)
    {
        photonView.RPC("RPC_TakeHealth", RpcTarget.All, health);
    }

    [PunRPC]
    public void RPC_TakeHealth(int health)
    {
        healthSystem.Heal(health);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            TakeDamage(5);
            Debug.Log("enemy dame");
        }

        else if (other.CompareTag("Heart"))
        {
            TakeHealth(50);
            Debug.Log("Healing");
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(5);
            Debug.Log("enemy dame");
        }
    }



    public void AddScore(int points)
    {
        score += points;
        CheckScoreMilestones();
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
        }
    }

    private void CheckScoreMilestones()
    {
        if (score >= 50 && !hasReached50Points)
        {
            hasReached50Points = true;
            IncreaseHealth(50);
        }
        if (score >= 100 && !hasReached100Points)
        {
            hasReached100Points = true;
            IncreaseFireRate(0.5f); // Reduces the fire rate by half (increase speed)
        }
        if (score >= 150 && !hasReached150Points)
        {
            hasReached150Points = true;
            IncreaseBulletDamage(1.5f); // Increase bullet damage by 50%
        }
        if (score >= 300 && !hasReached300Points)
        {
            hasReached300Points = true;
            IncreaseAllStats(1.1f); // Increase all stats by 10%
        }
    }

    private void IncreaseHealth(int amount)
    {
        maxHealth += amount;
        healthSystem.Heal(amount); // Heal the player by the increased amount

        // Sync health across the network
        photonView.RPC("SyncHealth", RpcTarget.All, maxHealth);
    }


    [PunRPC]
    private void SyncHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        healthSystem.SetHealth(maxHealth);
    }


    private void IncreaseFireRate(float multiplier)
    {
        TimeBtwFireGun *= multiplier;
        TimeBtwFireRifle *= multiplier;
    }

    private void IncreaseBulletDamage(float multiplier)
    {
        bulletForce *= multiplier;
    }

    private void IncreaseAllStats(float multiplier)
    {
        maxHealth = (int)(maxHealth * multiplier);
        healthSystem.SetHealth(maxHealth);
        TimeBtwFireGun *= multiplier;
        TimeBtwFireRifle *= multiplier;
        bulletForce *= multiplier;
    }
}
