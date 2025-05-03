using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    

    //30.04 by A.: 
    public float BulletDamage = 10f;    //bullets have Dmg 
    public Boolean isEnemyBullet = true;   //added this but I am not really using it yet in the game yet. For more information look at "if (stats.Health <= 0)"

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        // transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //30.04 by A.: Checks if ObjectStats is on the Enemy
            //If yes, we continue then we substract (BulletDamage divided by Armour) from HP
            //Afterwards we check if HP is equal 0 or less
            //If true, Enemy gets destroyed
            //FOR LATER: Player gets back some HP. Like 1HP for every enemy killed.
            //I call it HealthGenerator.

            ObjectStats stats = collision.gameObject.GetComponent<ObjectStats>();
            if (stats != null)
            {
                stats.Health -= BulletDamage/stats.Armour; ;
                Debug.Log("Enemy got " + stats.Health + " HP!!!");

                if (stats.Health <= 0)
                {
                    //if the bullet is fired from player then heal him by the ammount of HealthGenerator
                    if (!isEnemyBullet) {
                        GameObject player = GameObject.FindWithTag("Player"); //to add HP to the Player for example.
                        player.GetComponent<ObjectStats>().Health += stats.HealthGenerator;
                    }
                    Destroy(collision.gameObject);
                }
            }


            //30.04 removed the following because I added an if clause above
            //Destroy(collision.gameObject);

            //Bullet gets destroyed anyway no matter if the other collider has HP or not
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Collisions"))
        {
            Destroy(gameObject);
        }
    }
}
