using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{



    //For Enemy Stas
    public float Speed = 5f;
    public float Health = 100f;
    public float Armour = 1f; //Minimum 1. It can be as high as you much but don't forget (real dmg = dmg/Armour)
    public float HealthGenerator = 1f; //I guess it can be negative if you kill objects that you shouldn't destroy (?). Like hostages (?)
    public float rotationSpeed = 10f;
    public float ViewDistance = 13f;
    public float ViewAngle = 150f;
    public float StoppingDistance = 0f;
    public Boolean isStationary = false;    //doesn't roam around BEFORE the player is found OR doesn't move at all (like automatic robatic stationary guns)
    public bool isDead = false; //M: I need this to get rid of multiple collisions with the same enemy.
    //public float Scale = 2f; //05.05 NOT USING IT CURRENTLY. However with this we could make a big boss. Not really interessting for our current map

    //For Death Icon
    public Sprite deathSprite; // assign a cross/grave sprite in the inspector
    private SpriteRenderer spriteRenderer;


    //For the agent and waypoints/spawnpoints ...
    [SerializeField]
    NavMeshAgent agent;
    public Transform[] waypoints;         //just added it for fun. So enemies in first level/round can go from point A to B (technically to more Waypoints but I am keeping it simple)
    public float StoppingDuration = 1f;
    private int currentWaypointIndex = 0;  // Start at the first waypoint
    private float MovingWaitTime = 0f;
    private float ShootingWaitTime;
    private EnemyAttack enemyAttack;
    

    //movement, aim direction, checkingforplayers and etc.
    public Transform target;
    private Rigidbody2D rb;
    private Vector2 facingDirection;                //which direction he is facing
    private Vector2 aimDirection;                   //which direction he is aiming at
    private Vector2 movingDirection;
    private Boolean isPlayerFound = false;          //If he sees player once, he will not stop following him
    private Boolean isFirstTime = true;
    private float lastCheckedForPlayer;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        TryGetComponent<EnemyAttack>(out enemyAttack);

        agent.stoppingDistance = StoppingDistance;
        agent.speed = 3.5f;

        rb = GetComponent<Rigidbody2D>();


        facingDirection = Quaternion.Euler(0, 0, -90f) * transform.right.normalized;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }


    void Update()
    {
        if (gameObject == null) return;
        facingDirection = Quaternion.Euler(0, 0, -90f) * transform.right.normalized;

        Rotate();

        //Check if the player has spawned every 3s
        if (Time.time > lastCheckedForPlayer + 3f && target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            lastCheckedForPlayer = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (gameObject == null) return;
        if (isPlayerFound)
        {
            ChasePlayer();
            ShootPlayer();
        }

        if (!isPlayerFound && target != null)
        {
            //if there are two waypoints or more. Go through all of them and patrol
            if (!isStationary && waypoints.Length >= 2)
            {
                Patroling();
            }
            if (waypoints.Length == 1)  //added this for spawned enemies so they go to one particular point on a map
            {

            }
            PlayerExistence();
        }

    }

    private void Rotate()
    {
        if (!isPlayerFound) movingDirection = agent.velocity.normalized;

        if (isPlayerFound)
        {
            aimDirection = target.transform.position - transform.position;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), 10 * rotationSpeed * Time.deltaTime);

        }
        else if (!isPlayerFound && movingDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(movingDirection.y, movingDirection.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), 500 * Time.deltaTime);
        }
    }

    private void Patroling()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (MovingWaitTime <= 0f)
            {
                MovingWaitTime = StoppingDuration;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                if (currentWaypointIndex == waypoints.Length - 1)
                {
                    currentWaypointIndex = 0;
                }
                else
                {
                    currentWaypointIndex++;
                }
            }
            else
            {

                MovingWaitTime -= Time.deltaTime;
            }
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

    }

    //This class tries to find out if the player is in view distance and not behind a wall (collisions)
    private void PlayerExistence()
    {
        Vector2 PlayerPos = target.GetComponent<Rigidbody2D>().position;

        //get direction and distance to player
        Vector2 EnemyToPlayerDirection = (PlayerPos - rb.position);
        float EnemyToPlayerDistance = EnemyToPlayerDirection.magnitude;
        EnemyToPlayerDirection = EnemyToPlayerDirection.normalized;

        //check if the player is under ViewDistance
        if (EnemyToPlayerDistance <= ViewDistance && !isPlayerFound)
        {
            //check if player is ViewAngle
            if (PlayerInViewAngle(EnemyToPlayerDirection, ViewAngle) && NoObstacleToPlayer(EnemyToPlayerDirection))
            {
                FoundPLAYER();
            }
        }
    }


    private Boolean PlayerInViewAngle(Vector2 EnemyToPlayerDirection, float ViewAngle)
    {
        float angle = Vector2.Angle(facingDirection, EnemyToPlayerDirection);    //angle between view vector and enemy to player vector
        return angle <= ViewAngle / 2;
    }

    private Boolean NoObstacleToPlayer(Vector2 EnemyToPlayerDirection)
    {
        LayerMask layerMask = LayerMask.GetMask("Collisions", "Player");

        RaycastHit2D hit = Physics2D.Raycast(rb.position, EnemyToPlayerDirection, ViewDistance, layerMask);

        if (hit.collider != null && hit.collider.transform == target)
        {
            return true;
        }
        return false;
    }

    public void FoundPLAYER()
    {
        if (isFirstTime)
        {
            //EnemyFirstPosition = rb.position;
            //PlayerLastPosition = PlayerPos;

            isFirstTime = false;
            agent.speed = Speed;     //probably need to change this
            agent.isStopped = false;
            agent.stoppingDistance = 5;
            MovingWaitTime = Time.time + 1f;
            ShootingWaitTime = Time.time + 0.5f;
            isPlayerFound = true;
        }
    }


    private void ChasePlayer()
    {
        if (Time.time > MovingWaitTime)
        {
            agent.SetDestination(target.position);
        }

    }
    private void ShootPlayer()
    {
        if (Time.time > ShootingWaitTime && PlayerInViewAngle(aimDirection, 20f) && NoObstacleToPlayer( aimDirection))
        {
            enemyAttack.isShootingAllowed = true;
        }
        else if ( !PlayerInViewAngle(aimDirection, 20f) || !NoObstacleToPlayer( aimDirection))
        {
            enemyAttack.isShootingAllowed = false;
        }
    }


    private static readonly HashSet<string> checkTags = new() { "Player", "Bullet" };

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFirstTime && checkTags.Contains(collision.gameObject.tag))
        {
            FoundPLAYER();
        }
    }

    //in case it dies. Show a gravestone and deactivate eveything
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (spriteRenderer != null && deathSprite != null)
        {
            spriteRenderer.sprite = deathSprite;
            spriteRenderer.sortingLayerName = "Default"; // Ensure it's in the default layer
            spriteRenderer.sortingOrder = -1; // render the gravestone behind the player
        }

        //rotate the death picture and make it bigger
        transform.right = Vector3.right;
        transform.localScale = new Vector3(2, 2, 2);

        // Stop the enemy
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Lock Rigidbody
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeAll;
        // disable collider
        if (TryGetComponent<Collider2D>(out Collider2D col)) col.enabled = false;
        // disable enemyAttack script
        if (enemyAttack != null) enemyAttack.enabled = false;
        // disable this script
        enabled = false;

        // destroy the enemy in X seconds
        Destroy(gameObject, 2f);
    }

    //for later

    /* 

    private void GoBackToYourLastPosition()
    {

    }

    private void CheckSurrounding()
    {

    }            
    private void OnDestroy()
    {

    }

    */
}
