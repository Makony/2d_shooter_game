using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 5f; 
    public float bulletDamage = 10f;
    public float lifetime = 2f;
    private Rigidbody2D rb;

    private bool isDestroyed = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
        TryGetComponent<Rigidbody2D>(out rb);
    }

    void FixedUpdate()
    {
        //set speed of the bullet and check if it is alawys the same (important for collision with dead enemies
        if (rb != null && rb.linearVelocity != (Vector2)transform.right.normalized * bulletSpeed)
        {
            rb.linearVelocity = transform.right.normalized * bulletSpeed;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
           /*SUMMARY: Gets stats of enemy
             * substract damage
             * then check if he should die
             * if yes, heal the player 
             * tell LevelManager one enemy is down
             */

            if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (enemy.isDead) return; //M: if enemy is already dead, don't call the function for multiple collisions => it thinks two enemies were killed instead of one
                enemy.Health -= bulletDamage;

                if (enemy.Health <= 0)
                {
                    enemy.Die();
                    Player player = GameObject.FindWithTag("Player").GetComponent<Player>();      //get Player script
                    player.Health = Mathf.Ceil( Math.Clamp(player.Health + enemy.HealthGenerator, 0, player.MaxHP));    //add HP to player but HP can only be between 0 and MAX HP
                    LevelManager.Instance.HPstat();
                    //LevelManager.Instance?.EnemyKilled(); //M: call the function in LevelManager to check if all enemies are dead
                    // I was getting the message "Unity objects should not use null propagation" so I am using if clause now
                    if (LevelManager.Instance != null)
                    {
                        LevelManager.Instance.EnemyKilled();
                    }
                }
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            /*SUMMARY: Gets stats of player
             * substract damage
             * then check if he should die
             */
            
            if (collision.gameObject.TryGetComponent<Player>(out Player player))
            {
                player.Health -= Mathf.Floor( bulletDamage / player.Armour);
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.HPstat();
                }
                if (player.Health <= 0)
                {
                    player.Die();
                }
            }
        }

        Destroy(gameObject);
    }
}
