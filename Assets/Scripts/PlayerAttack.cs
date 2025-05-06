using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform Gun;
    public Transform BulletManager; //to make it clean if we are spawning 100+ bullets at a time. Basically all bullets are under That now



    public Boolean isContinuesFire = true;   //false means something like a shotgun/pistol. true means rifle, smg and etc.
    public float bulletCooldown = 0.2f;      //Cooldown between shots
    public float BulletSize = 1f;            //makes the bullet bigger or smaller.
    public float BulletSpeed = 15f;          //for rewards 
    public float BulletDamage = 10f;         //for rewards 
    public float BulletLifetime = 2.5f;      //for rewards
    public float Ammo = 100f;                //for rewards
    public int BulletPerShot = 5;            //for shotguns maybe? or rifles that got isContinuesFire = false and can fire 3 bullets at a time
    public float MagazineCount = 7;          //Magazine Count
    public float BulletsPerMag = 30;         //number of bullets in one magazine
    public float AccuracyErrorAngle = 25f;   //x Degree to left and x Degree to right of where you are aiming at. Example: 10 means 20 Degree Deviation
    public float ReloadTime = 1f;            //05.05. A: How long it takes to Reload
    //public Boolean hasShootingModes = true;    //05.05. A: example burst fire (3 bullets per shot) for pistols for example

    private float lastBulletTime;

    private void Start()
    {
        Gun = transform.Find("Gun");
    }

    void Update()
    {
        if (isContinuesFire)
        {
            if (Input.GetMouseButton(0) && Time.time > lastBulletTime + bulletCooldown)
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && Time.time > lastBulletTime + bulletCooldown)
            {
                Shoot();
            }

        }
    }

    void Shoot()
    {
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
            yield return null;  //wait 1 frame
        }
    }
}
