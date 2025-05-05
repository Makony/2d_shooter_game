using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 5f;
    public float lifetime = 2f;

    

    //30.04 by A.: 
    public float BulletDamage = 10f;    //bullets have Dmg now
    public Boolean isEnemyBullet = true;   //is it a unfriendly bullt => if yes it can be important for Health Generation and etc.

    void Start()
    {
        Destroy(gameObject, lifetime);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Hit #{collision.contactCount} on {collision.collider.name}");
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //SUMMARY: Checks if ObjectStats is on the Enemy
            //If yes, we continue then we substract (BulletDamage divided by Armour) from HP
            //Afterwards we check if HP is equal 0 or less
            //If true, Enemy gets destroyed and player gets healed by amounth of HealthGenerator


            //UPDATE! 04.05, A: I am using TryGetComponent now because it doesn't allocate memory(?) to stats if it is null
            //changed it after reading the message in console
            //=> Checks if ObjectStats is on the object that we hit. If yes =>
            //basically it outputs stats if there is a component "ObjectStats" on the collision
            collision.gameObject.TryGetComponent<ObjectStats>(out ObjectStats stats);

            if (stats != null)
            {
                stats.Health -= BulletDamage/stats.Armour; 

                if (stats.Health <= 0)
                {
                    //if the bullet is fired from player then heal him by the ammount of HealthGenerator
                    if (!isEnemyBullet) {
                        GameObject player = GameObject.FindWithTag("Player"); //find Player.
                        ObjectStats PlayerStats = player.GetComponent<ObjectStats>();   //Get his stats
                        PlayerStats.Health = Math.Clamp(PlayerStats.Health + stats.HealthGenerator, 0, 100);    //add HP to player but HP can only be between 0 and 100
                    }
                    LevelManager.Instance?.EnemyKilled();
                    Debug.Log("Enemy killed!");
                    Destroy(collision.gameObject);
                }
            }
        }
        
        //Bullet gets destroyed anyway no matter if stats is null or not. This is why I deleted the "else" part
        Destroy(gameObject);
    }

    //04.05 by M: function so we can increase the speed of the bullet as a reward
    public void IncreaseShooting(float value)
    {
        bulletSpeed += value;
    }
}
