using System;
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform Gun;
    public Transform bulletManager; //to make it clean if we are spawning 100+ bullets at a time. Basically all bullets are under That now



    public Boolean isContinuesFire = false;   //false means something like a shotgun/pistol. true means rifle, smg and etc.
    public float bulletCooldown = 0.2f;      //Cooldown between shots
    public float BulletSize = 1f;            //makes the bullet bigger or smaller.
    public float BulletSpeed = 15f;          //for rewards 
    public float BulletDamage = 10f;         //for rewards 
    public float BulletLifetime = 2.5f;      //for rewards   
    public float Ammo = 10f;                //for rewards
    public int BulletPerShot = 1;            //for shotguns maybe? or rifles that got isContinuesFire = false and can fire 3 bullets at a time
    public float MaxAmmo = 10f;                //How much bullets are left before reloading
    public float RemainingAmmo;
    public float AccuracyErrorAngle = 10f;   //x Degree to left and x Degree to right of where you are aiming at. Example: 10 means 20 Degree Deviation
    public float ReloadTime = 2f;            //05.05. A: How long it takes to Reload
    public Boolean isReloading = false;
    //public Boolean hasShootingModes = true;    //05.05. A: example burst fire (3 bullets per shot) for pistols for example
    public Boolean isShootingAllowed = false;
    private float lastBulletTime;

    //for animation and stuff
    private Animator animator;

    void Start()
    {
        Gun = transform.Find("Gun");
        bulletManager = BulletManager.Instance.transform;
        animator = GetComponent<Animator>();
        RemainingAmmo = MaxAmmo;
    }

    void Update()
    {
        if (isShootingAllowed)
        {
            animator.SetBool("isShooting", true);
            if (RemainingAmmo > 0)
            {
                Shoot();
            }
            else
            {
                Reload();
            }
        } else
        {
            animator.SetBool("isShooting", false);
        }
    }

    void Reload()
    {
        if (!isReloading)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        animator.SetBool("isShooting", false);
        isReloading = true;
        animator.SetBool("isReloading", true);
        yield return new WaitForSeconds(2f); // Wait for 1 second
        animator.SetBool("isReloading", false);
        RemainingAmmo = MaxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        if (Time.time > lastBulletTime + bulletCooldown)
        {
            //check if bullets and gun are assigned
            if (bulletPrefab != null && Gun != null)
            {
                //only important for BulletPerShot > 1 => basically it will make a bullet every frame (look under ShootsBulletsOvertime)
                StartCoroutine(ShootsBulletsOvertime(BulletPerShot));
            }
        }
    }
    //added this because spawning 1000 bullets at one frame was freezing Unity. This makes bullets look a bit better.
    IEnumerator ShootsBulletsOvertime(int BulletPerShot)
    {
        //shoot n number of bullets depending how much BulletPershot is then wait one frame
        for (int i = 1; i <= BulletPerShot; i++)
        {
            if (RemainingAmmo <= 0) yield break;
            //Calculate a shooting error angle, change the rotation of the bullet (it comes out of the Gun so change that) THEN make the bullet
            float fireAngleERR = UnityEngine.Random.Range(-AccuracyErrorAngle, AccuracyErrorAngle);
            Quaternion newFireDirection = Gun.rotation * Quaternion.Euler(0f, 0f, fireAngleERR);
            GameObject bullet = Instantiate(bulletPrefab, Gun.position, newFireDirection.normalized, bulletManager);   //BulletManager becomes the parent here. To make everything lcean in left side of Unity
            if (bullet.TryGetComponent<Bullet>(out Bullet bulletStats))
            {
                bulletStats.bulletSpeed = BulletSpeed;
                bulletStats.bulletDamage = BulletDamage;
                bulletStats.lifetime = BulletLifetime;
            }
            bullet.transform.localScale = new Vector3(BulletSize * 0.1f, BulletSize * 0.1f, 1f);    // make the bullet as big you want
            RemainingAmmo--;
            lastBulletTime = Time.time;    //brought this here so it knows when last bullet got fired from the gun (not when you pressed your mouse button)
            yield return null;  //wait 1 frame
        }
    }
}
