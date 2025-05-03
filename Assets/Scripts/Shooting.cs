using System;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform gun;

    public float bulletCooldown = 0.2f;
    public Boolean isContinuesFire = true;

    private float lastBulletTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //extra for PISTOLES to only shoot once per click
        //if (Input.GetMouseButtonDown(0))
        //for machine guns and etc. we use the isContinuesFire
         

        if (isContinuesFire)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time > lastBulletTime + bulletCooldown)
                {
                    Shoot();
                    lastBulletTime = Time.time;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time > lastBulletTime + bulletCooldown)
                {
                    Shoot();
                    lastBulletTime = Time.time;
                }
            }

        }
    }

    //03.05, A: I think we should make another class so we don't use the same code above twice. Idk right now. Like this:
    
    /*
    private void checkCooldown(float bulletCooldown)
    {
        if (Time.time > lastBulletTime + bulletCooldown)
        {
            Shoot();
            lastBulletTime = Time.time;
        }
    }
    */

    void Shoot(){
        GameObject bullet = Instantiate(bulletPrefab, gun.position, gun.rotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = gun.right * bullet.GetComponent<Bullet>().speed;
    }
}
