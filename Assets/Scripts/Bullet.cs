using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 5f; //NOT USING IT FROM HERE
    public float lifetime = 2f;



    //30.04 by A.: 
    public float BulletDamage = 10f;    //bullets have Dmg now

    void Start()
    {
        Destroy(gameObject, lifetime);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"Hit #{collision.contactCount} on {collision.collider.name}");
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

            if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                enemy.FoundPLAYER();

            if (collision.gameObject.TryGetComponent<ObjectStats>(out ObjectStats stats))
            {
                if (stats.isDead) return; //M: if enemy is already dead, don't call the function for multiple collisions
                stats.Health -= BulletDamage / stats.Armour;
                Debug.Log(stats.Health);

                if (stats.Health <= 0)
                {
                    stats.isDead = true;
                    //if the bullet is fired from player then heal him by the ammount of HealthGenerator
                    GameObject player = GameObject.FindWithTag("Player"); //find Player.
                    ObjectStats PlayerStats = player.GetComponent<ObjectStats>();   //Get his stats
                    PlayerStats.Health = Math.Clamp(PlayerStats.Health + stats.HealthGenerator, 0, 100);    //add HP to player but HP can only be between 0 and 100

                    LevelManager.Instance?.EnemyKilled(); //M: call the function in LevelManager to check if all enemies are dead
                    Debug.Log("Enemy killed!");

                    Destroy(collision.gameObject);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<ObjectStats>(out ObjectStats stats))
            {
                stats.Health -= BulletDamage / stats.Armour;
                Debug.Log(stats.Health);
                if (stats.Health <= 0)
                {
                    stats.isDead = true; //M: means player is dead now
                    Destroy(collision.gameObject);
                }
            }
        }
        //Bullet gets destroyed anyway no matter if stats is null or not. This is why I deleted the "else" part
        Destroy(gameObject);
    }

    //04.05 by M: function so we can increase the speed of the bullet as a reward
    //05.05 by A: I haven't complety finished "Shooting" script yet but if we want to control Bulletspeed from here, then we have to use this bullet speed here instead of in "Shooting".
    //This is not really the case right now. I was actually thinking to move everything bullet related to Shooting to lessen the "GetComponent" calls.
    public void IncreaseShooting(float value)
    {
        bulletSpeed += value;
    }
}
