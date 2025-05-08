using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform Gun;
    public Transform BulletManager; //to make it clean if we are spawning 100+ bullets at a time. Basically all bullets are under That now
    public GameObject LevelManager;
    private LevelManager levelManager;

    public Boolean isContinuesFire = false;   //false means something like a shotgun/pistol. true means rifle, smg and etc.
    public float bulletCooldown = 0.2f;      //Cooldown between shots
    public float BulletSize = 1f;            
    public float BulletSpeed = 15f;           
    public float BulletDamage = 10f;         
    public float BulletLifetime = 2.5f;      
    public float MaxAmmo = 10f;                //How much bullets are left before reloading
    public float RemainingAmmo;
    public int BulletPerShot = 1;            //for shotguns maybe? or rifles that got isContinuesFire = false and can fire 3 bullets at a time
    public float AccuracyErrorAngle = 20f;   //x Degree to left and x Degree to right of where you are aiming at. Example: 10 means 20 Degree Deviation
    public float ReloadTime = 0.5f;
    public Boolean isReloading = false;
    //public Boolean hasShootingModes = true;    //A: example burst fire (3 bullets per shot) for pistols for example
    private float lastBulletTime;

    //for animation and stuff
    private Animator animator;

    private void Start()
    {
        Gun = transform.Find("Gun");
        RemainingAmmo = MaxAmmo;
        LevelManager.TryGetComponent<LevelManager>(out levelManager);
        AccuracyErrorAngle /= isContinuesFire ? 1 : 2;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isContinuesFire = !isContinuesFire;
            AccuracyErrorAngle /= isContinuesFire ? (float)0.5 : 2;
            Debug.Log(AccuracyErrorAngle);
            levelManager.AmmoIcon(isContinuesFire);
        }
        if (Input.GetKeyDown(KeyCode.R)) { Reload(); }

        if (isContinuesFire && RemainingAmmo > 0 && !isReloading)
        {
            if (Input.GetMouseButton(0) && Time.time > lastBulletTime + bulletCooldown)
            {
                animator.SetBool("isShooting", true);
                Shoot();
            }
        }
        else if (!isContinuesFire && RemainingAmmo > 0 && !isReloading)
        {
            if (Input.GetMouseButtonDown(0) && Time.time > lastBulletTime + 0.25f)
            {
                
                Shoot();
                StartCoroutine(Hipfires());
            }
        }
        else if (RemainingAmmo <= 0 && !isReloading)
        {
            Reload();
        }

        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(0)) {
            animator.SetBool("isShooting", false);
        }
    }

    IEnumerator Hipfires()
    {
        animator.SetBool("isShooting", true);
        yield return new WaitForSeconds(0.035f);
        animator.SetBool("isShooting", false);
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
        yield return new WaitForSeconds(ReloadTime); // Wait for "ReoloadTime" seconds
        animator.SetBool("isReloading", false);
        RemainingAmmo = MaxAmmo;
        levelManager.AmmoStat();
        isReloading = false;
    }

    void Shoot()
    {
        Debug.Log("shooting HAS STARTED");
        //check if bullets and gun are assigned
        if (bulletPrefab != null && Gun != null)
        {
            //only important for BulletPerShot > 1 => basically it will make a bullet every frame (look under ShootsBulletsOvertime)
            StartCoroutine(ShootsBulletsOvertime(BulletPerShot));
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
            GameObject bullet = Instantiate(bulletPrefab, Gun.position, newFireDirection.normalized, BulletManager);   //BulletManager becomes the parent here. To make everything lcean in left side of Unity
            
            if (bullet.TryGetComponent<Bullet>(out Bullet bulletStats))
            {
                bulletStats.bulletSpeed = BulletSpeed;
                bulletStats.bulletDamage = BulletDamage;
                bulletStats.lifetime = BulletLifetime;
            }
            bullet.transform.localScale = new Vector3(BulletSize * 0.1f, BulletSize * 0.1f, 1f);    // make the bullet as big you want

            lastBulletTime = Time.time;    //brought this here so it knows when last bullet got fired from the gun (not when you pressed your mouse button)
            RemainingAmmo--;
            levelManager.AmmoStat();
            yield return null;  //wait 1 frame
        }
    }
}
