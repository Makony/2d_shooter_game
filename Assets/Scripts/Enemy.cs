using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{

    //Note: not complete. Just added this from the NavMesh2D video that I sent you so I could test out how all of this works.
    //Gonna add usable behaviours and etc. from the now obsolet "EnemyMovement" script. (Rotation and etc.)

    /*03.05, A: added auto assign "target" under Start()
     */

    /*05.05, A: added many stuff
     * 
     */









    [SerializeField]
    NavMeshAgent agent;
    
    
    public Transform Gun;
    public Transform[] waypoints;         //just added it for fun. So enemies in first level/round can go from point A to B (technically to more Waypoints but I am keeping it simple)
    public float StoppingDuration = 1f;
    private int currentWaypointIndex = 0;  // Start at the first waypoint
    private float MovingWaitTime= 0f;
    private float ShootingWaitTime;


    //change Transform target from SerializeField to Public so we can change it via other Gameobjects. Imagine a power that makes some enemies hostile to other enemies
    public Transform target;

    


    //ENEMY
    //05.05, some needed stats
    public float Speed = 2f;
    public float rotationSpeed = 10f;
    public float ViewDistance = 13f;
    public float ViewAngle = 150f;
    public float StoppingDistance = 0f;
    public Boolean isStationary = false;    //doesn't roam around BEFORE the player is found OR doesn't move at all (like automatic robatic stationary guns)
    //public float Scale = 2f; //05.05 NOT USING IT CURRENTLY. However with this we could make a big boss. Not really interessting for our current map

    //05.05 by A: added the followings for movement+rotation of rigidbody rb
    private Rigidbody2D rb;
    private Vector2 facingDirection;                //which direction he is facing
    private Vector2 aimDirection;                   //which direction he is aiming at
    private Vector2 movingDirection;
    private Boolean isPlayerFound = false;          //If he sees player once, he will not stop following him
    private Boolean isFirstTime = true;
    private float lastCheckedForPlayer;


    //05.05 by A: not really needed because it is not a stealh game. still got the codes intact if we need it for main proejct.
    //private Vector2 EnemyFirstPosition;
    //private Vector2 PlayerLastPosition;




    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        //05.05, A:
        agent.stoppingDistance = StoppingDistance;
        agent.speed = Speed;

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
        facingDirection = Quaternion.Euler(0, 0, -90f) * transform.right.normalized;

        Rotate();

        //Check if the player has spawned every 3s (?)
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

        if (isPlayerFound)
            ChasePlayer();

        if (!isPlayerFound && target != null)
        {
            if (!isStationary && waypoints.Length >= 2)
            {
                Patroling();
            }
            PlayerExistence();
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
        if (isFirstTime) {
            //EnemyFirstPosition = rb.position;
            //PlayerLastPosition = PlayerPos;

            isFirstTime = false;
            agent.speed = 5f;     //probably need to change this
            agent.isStopped = false;
            agent.stoppingDistance = 4;
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

        if (Time.time > ShootingWaitTime && PlayerInViewAngle(aimDirection , 20f) )
        {

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
