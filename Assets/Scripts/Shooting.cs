using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform gun;

    public float bulletCooldown = 0.2f;
    private float lastBulletTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

    void Shoot(){
        GameObject bullet = Instantiate(bulletPrefab, gun.position, gun.rotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = gun.right * bullet.GetComponent<Bullet>().speed;
    }
}
