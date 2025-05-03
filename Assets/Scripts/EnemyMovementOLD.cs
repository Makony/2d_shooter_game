using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.Image;

public class EnemyMovementOLD : MonoBehaviour
{
    //03.05,A: COMPLETLY OBSOLETE AFTER using NavMesh. You can read through it if you want but just ignore the debug/avoidance related parts. 
    //I will reuse some of these in the "Enemy" script later.

    public float Speed = 1f;
    public float rotationSpeed = 10f;
    public float ViewDistance = 10f;
    public float ViewAngle = 150f;
    public float ObstacleRange = 0.5f;
    public Boolean moveable = true; //decide if the enemies should be stationary or moving. (I need a better name than moveable. It sounds dumb)
    public Transform Gun;
    public Transform Player;

    //01.05 by A: added the followings for movement+rotation of rigidbody rb
    private Rigidbody2D rb;  
    private Vector2 facingDirection;        //which direction he is facing
    private Vector2 aimDirection;           //which direction he is aiming at
    private Boolean FoundPlayer = false;          //If he sees player once, he will not stop following him
    private float lastCheckedForPlayer;

    private Vector2 EnemyFirstPosition;
    private Vector2 PlayerLastPosition;
    private Boolean AvoidanceNeeded = false;
    private Vector2 FreePosition;

    private Boolean test = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        facingDirection = Quaternion.Euler(0, 0, -90f) * transform.right.normalized;


        ObstacleRange = GetComponent<Collider2D>().bounds.size.y;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.transform;
        }
    }

    
    void Update()
    {
        //Check if the player has spawned every 3s (?)
        if (Time.time > lastCheckedForPlayer + 3f && Player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                Player = playerObj.transform;
            }
            lastCheckedForPlayer = Time.time;
        }

       //if (Player != null) {
       //     facingDirection = (Player.position - transform.position).normalized;        //like in school A to B Vector, A being enemy, B player
       // }
    }

    private void FixedUpdate()
    {
        facingDirection = Quaternion.Euler(0, 0, -90f) * transform.right.normalized;

        if (!FoundPlayer)
        {
            // rb.linearVelocity = facingDirection * Speed;
            if (Player != null)
            {
                PlayerExist();       //like in school A to B Vector, A being enemy, B player
            }
        }

        if (FoundPlayer)
            ChasePlayer();
    }

    private void PlayerExist()
    {
        Vector2 PlayerPos = Player.GetComponent<Rigidbody2D>().position;

        Vector2 EnemyToPlayerDirection = (PlayerPos - rb.position).normalized;
        float EnemyToPlayerDistance = Vector2.Distance(PlayerPos, rb.position);
        
        if (EnemyToPlayerDistance <= ViewDistance && !FoundPlayer)
        {
            Debug.Log("22222");
            if (PlayerInViewAngle(EnemyToPlayerDirection) && NoObstacleToPlayer(EnemyToPlayerDirection))
            {
                
                EnemyFirstPosition = rb.position;
                PlayerLastPosition = PlayerPos;
                FoundPlayer = true;
                Speed = 5f;
            }
        }
    }   


    private Boolean PlayerInViewAngle( Vector2 EtoPDirection)
    {
        float angle = Vector2.Angle(facingDirection, EtoPDirection);    //angle between view vector and enemy to player vector
        Debug.Log("55555   " + angle);
        return angle <= ViewAngle / 2;
    }

    private Boolean NoObstacleToPlayer(Vector2 EtoPDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, EtoPDirection, ViewDistance);
        Debug.Log("66666");
        if (hit.collider != null && hit.collider.transform == Player)
        {
            return true;
        }
        return false;
    }

    private void ChasePlayer()
    {
        Vector2 PlayerPos = Player.GetComponent<Rigidbody2D>().position;
        Vector2 EnemyToPlayerDirection = (PlayerPos - rb.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, EnemyToPlayerDirection, ViewDistance);

        if (!AvoidanceNeeded)
        {
            if ((hit.collider != null && hit.collider.transform == Player))
            {
                rb.linearVelocity = EnemyToPlayerDirection * Speed;
                PlayerLastPosition = PlayerPos;
                rotateEnemy();
                Debug.Log("1");
            }
            else
            {
                EnemyToPlayerDirection = (PlayerLastPosition - rb.position).normalized;
                rb.linearVelocity = EnemyToPlayerDirection * Speed;
                rotateEnemy();
                Debug.Log("2");
            }

            if (Vector2.Distance(rb.position, PlayerLastPosition) < ObstacleRange/4)
            {
                FoundPlayer = false;
                rb.linearVelocity = Vector2.zero;
                CheckSurrounding();
                GoBackToYourLastPosition();
                Debug.Log("3");
            }

            Vector2 AvoidDirection = AvoidObstacles(EnemyToPlayerDirection);
            if (AvoidDirection != EnemyToPlayerDirection)
            {
                FreePosition = rb.position + AvoidDirection ;
                rb.linearVelocity = AvoidDirection * Speed;
                rotateEnemy();
                //AvoidanceNeeded = true;
                Debug.Log("4");
            }
        } else {
            if (Vector2.Distance(rb.position,FreePosition)<0.5f) {
                AvoidanceNeeded = false;
            }
        }
    }

    private Vector2 AvoidObstacles(Vector2 EtoPDirection)
    {
        Vector2 Plus45 = Quaternion.Euler(0, 0, 45f) * EtoPDirection.normalized;
        Vector2 Minus45 = Quaternion.Euler(0, 0, -45f) * EtoPDirection.normalized;


        RaycastHit2D hitPlus45 = Physics2D.Raycast(rb.position, Plus45, ObstacleRange);
        RaycastHit2D hitMinus45 = Physics2D.Raycast(rb.position, Minus45, ObstacleRange);

        if (hitPlus45.collider != null && hitMinus45.collider == null)
        {
            //Vector2 FreePosition = (Vector2)transform.position + Minus45 * (ObstacleRange/2);
            return Minus45;
        }

        if (hitPlus45.collider == null && hitMinus45.collider != null)
        {
            //Vector2 FreePosition = (Vector2)transform.position + Plus45 * (ObstacleRange/2);
            return Plus45;
        }

        return EtoPDirection;

    }

    private void rotateEnemy()
    {
        float distance0 = Vector2.Distance(Gun.position, rb.position); //distance between the gun and enemy
        float distance1 = Vector2.Distance(PlayerLastPosition, rb.position); //distance between player and enemy
        Vector2 PlayerPos = PlayerLastPosition;
        Vector2 GunPos = (Vector2)Gun.position;

        aimDirection = (PlayerPos - rb.position).normalized;    //player's center to enemy's center

        //GUN is pointing at the player if (distance between the gun and player) is less than (distance between mouse and player) plus a small error (0.1f)
        //otherwise it is gonna point center of enemy towards the player (taking the aimDirection from above)
        if (distance0 < distance1 - 0.1f)
        {
            aimDirection = (PlayerPos - GunPos).normalized;
        }
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg+90f;      //to calculate the angle between mouse and gun/player
        angle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime); //added smoothspeed so it turns slowly/fast
        rb.MoveRotation(angle);
    }

    private void GoBackToYourLastPosition()
    {

    }

    private void CheckSurrounding()
    {

    }
}
