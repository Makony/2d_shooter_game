using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 5f; //NOT USING IT FROM HERE
    public float lifetime = 2f;
    private Rigidbody2D rb;


    //30.04 by A.: 
    public float bulletDamage = 10f;    //bullets have Dmg now

    void Start()
    {
        Destroy(gameObject, lifetime);

        TryGetComponent<Rigidbody2D>(out rb);
    }

    void FixedUpdate()
    {
        if (rb != null && rb.linearVelocity != (Vector2)transform.right.normalized * bulletSpeed)
        {
            rb.linearVelocity = transform.right.normalized * bulletSpeed;
        }
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
            {
                enemy.FoundPLAYER();
                if (enemy.isDead) return; //M: if enemy is already dead, don't call the function for multiple collisions => it thinks two enemies were killed instead of one
                enemy.Health -= bulletSpeed / enemy.Armour;
                Debug.Log(enemy.Health);

                if (enemy.Health <= 0)
                {
                    enemy.isDead = true;
                    //if the bullet is fired from player then heal him by the ammount of HealthGenerator
                    Player player = GameObject.FindWithTag("Player").GetComponent<Player>(); //find Player.

                    player.Health = Math.Clamp(player.Health + enemy.HealthGenerator, 0, 100);    //add HP to player but HP can only be between 0 and 100

                    LevelManager.Instance?.EnemyKilled(); //M: call the function in LevelManager to check if all enemies are dead
                    Debug.Log("Enemy killed!");

                    Destroy(collision.gameObject);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<Player>(out Player player))
            {
                player.Health -= bulletSpeed / player.Armour;
                Debug.Log(player.Health);
                if (player.Health <= 0)
                {
                    player.isDead = true; //M: means player is dead now
                    LevelManager.Instance?.PlayerKilled(); //M: call the function in LevelManager to check if player is dead
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
