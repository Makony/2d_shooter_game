using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public event Action OnDeath;

    //For Enemy Stas
    public float Speed = 5f;
    public float Health = 100f;
    public float Armour = 1f; //Minimum 1. It can be as high as you much but don't forget (real dmg = dmg/Armour)
    public float HealthGenerator = 1f; //I guess it can be negative if you kill objects that you shouldn't destroy (?). Like hostages (?)
    public float rotationSpeed = 10f;
    public float ViewDistance = 13f;
    public float ViewAngle = 150f;
    public float StoppingDistance = 1f;
    public Boolean isStationary = false;    //doesn't roam around BEFORE the player is found OR doesn't move at all (like automatic robatic stationary guns)
    public bool isDead = false; //M: I need this to get rid of multiple collisions with the same enemy.
    //public float Scale = 2f; //05.05 NOT USING IT CURRENTLY. However with this we could make a big boss. Not really interessting for our current map

    //For Death Icon
    public Sprite deathSprite; // assign a cross/grave sprite in the inspector
    private SpriteRenderer spriteRenderer;


    //For Patrolling
    [SerializeField]
    public Transform[] waypoints;         //just added it for fun. So enemies in first level/round can go from point A to B (technically to more Waypoints but I am keeping it simple)
    public float StoppingDuration = 1f;
    private int currentWaypointIndex = 0;  // Start at the first waypoint
    private float MovingWaitTime = 0f;
    public float MovingWaitTimeMutliplicator = 1;
    private float ShootingWaitTime;
    public float ShootingWaitTimeMultiplicator = 1;
    private EnemyAttack enemyAttack;

    public Boolean IsBuffNeeded = false;
    private float buffTimer = 0f;
    private float buffInterval = 5f; 
    private float speedBuffAmount = 1f;
    private float rotationBuffAmount = 5f;


    //For JPS Pathfinding (Replaces NavMeshAgent)
    private GridManager gridManager;
    private JPS pathfinder;
    private List<Node> currentPath;
    private int pathIndex;
    private float repathTimer;
    public float repathInterval = 0.75f;

    //movement, aim direction, checkingforplayers and etc.
    public Transform target;
    private Rigidbody2D rb;
    private Vector2 facingDirection;                //which direction he is facing
    private Vector2 aimDirection;                   //which direction he is aiming at
    private Vector2 movingDirection;
    public Boolean isPlayerFound = false;          //If he sees player once, he will not stop following him
    private Boolean isFirstTime = true;
    private float lastCheckedForPlayer;
     private float randomStoppingDistance;

    //for animation and stuff
    private Animator animator;


    void Start()
    {
        randomStoppingDistance = UnityEngine.Random.Range(5f, 10f);

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        

        rb = GetComponent<Rigidbody2D>();
        TryGetComponent<EnemyAttack>(out enemyAttack);
  
        gridManager = GridManager.Instance;
        if (gridManager == null || gridManager.GetGrid() == null)
        {
            enabled = false;
            return;
        }
        pathfinder = new JPS(gridManager.GetGrid());




        facingDirection = Quaternion.Euler(0, 0, -90f) * transform.right.normalized;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        
        if (!isStationary && waypoints.Length > 0)
        {
            FindNewPath(waypoints[currentWaypointIndex].position);
        }

    }


    void Update()
    {
        if (isDead) return;

        if (gameObject == null) return;
        facingDirection = Quaternion.Euler(0, 0, -90f) * transform.right.normalized;

        Rotate();

        //Check if the player has spawned every 3s
        if (target == null && Time.time > lastCheckedForPlayer + 3f)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            lastCheckedForPlayer = Time.time;
        }

        if (IsBuffNeeded && isPlayerFound)
        {
            buffTimer += Time.deltaTime;
            if (buffTimer >= buffInterval)
            {
                buffTimer = 0f; // Reset timer
                repathInterval *= 0.99f;
                Speed += speedBuffAmount;
                rotationSpeed += rotationBuffAmount;
            }
        }

        animator.SetBool("isRunning", rb.linearVelocity.sqrMagnitude > 0.01f);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
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

    private bool FindNewPath(Vector3 targetPosition)
    {
        if (pathfinder == null) return false;
        
        Node startNode = gridManager.WorldToNode(transform.position);
        Node targetNode = gridManager.WorldToNode(targetPosition);

        if (startNode == null || !startNode.IsWalkable()) return false;

        if (targetNode == null || !targetNode.IsWalkable())
        {
            targetNode = gridManager.FindNearestWalkableNode(targetNode);
        }

        if (targetNode == null) return false;

        currentPath = pathfinder.FindPath(startNode, targetNode);
        pathIndex = 0;
        
        if (currentPath != null && currentPath.Count > 0)
        {
            return true;
        }
        
        currentPath = null;
        return false;
    }

     private void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            movingDirection = Vector2.zero; // No path or path is finished
            return;
        }

        Vector2 targetNodePosition = gridManager.NodeToWorld(currentPath[pathIndex]);
        
        // Check stopping distance for the final target
        bool isFinalDestination = (pathIndex == currentPath.Count - 1);
        if (isPlayerFound && isFinalDestination && Vector2.Distance(transform.position, targetNodePosition) <= StoppingDistance)
        {
            movingDirection = Vector2.zero;
            return;
        }

        // Move towards the next node
        transform.position = Vector2.MoveTowards(transform.position, targetNodePosition, Speed * Time.deltaTime);
        movingDirection = (targetNodePosition - (Vector2)transform.position).normalized;

        // If we've reached the current node, target the next one
        if (Vector2.Distance(transform.position, targetNodePosition) < 0.1f)
        {
            pathIndex++;
        }
    }
    
    private void Rotate()
    {
        //if (!isPlayerFound) movingDirection = agent.velocity.normalized;

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
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            MovingWaitTime += Time.deltaTime;
            if (MovingWaitTime >= StoppingDuration)
            {
                MovingWaitTime = 0f;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                FindNewPath(waypoints[currentWaypointIndex].position);
            }
        }
        else
        {
            MoveAlongPath();
        }
    }

    private void PlayerExistence()
    {
        if (target == null) return;         
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
            MovingWaitTime = Time.time + (0.75f / MovingWaitTimeMutliplicator);
            ShootingWaitTime = Time.time + (0.35f / ShootingWaitTimeMultiplicator);
            isPlayerFound = true;
        }
    }


    private void ChasePlayer()
    {
        if (Time.time > MovingWaitTime)
        {
            repathTimer += Time.deltaTime;
            if (repathTimer >= repathInterval)
            {
                repathTimer = 0f;
                FindNewPath(target.position);
            }

            Vector2 EnemyToPlayerDirection = (target.position - transform.position);
            float EnemyToPlayerDistance = EnemyToPlayerDirection.magnitude;
            EnemyToPlayerDirection.Normalize();

            if (EnemyToPlayerDistance <= randomStoppingDistance && NoObstacleToPlayer(EnemyToPlayerDirection))
            {
                // Stop moving if close and has line of sight
                movingDirection = Vector2.zero; 
            }
            else
            {
                // Continue moving along the path
                MoveAlongPath();
            }
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
        SoundManager.Instance.EnemyDeathSound();
        OnDeath?.Invoke();
        animator.enabled = false;

        if (spriteRenderer != null && deathSprite != null)
        {
            spriteRenderer.sprite = deathSprite;
            spriteRenderer.sortingLayerName = "Default"; // Ensure it's in the default layer
            spriteRenderer.sortingOrder = -1; // render the gravestone behind the player
        }

        //rotate the death picture and make it bigger
        transform.right = Vector3.right;
        transform.localScale = new Vector3(2, 2, 2);

        movingDirection = Vector2.zero;

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

    //for later probably never :D

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
