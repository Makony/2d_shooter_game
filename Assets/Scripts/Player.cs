using System.Diagnostics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public Transform gun;

    //Player Stats like HP, Speed and etc.
    public float speed = 5f;
    public float RotationSpeed = 10f;   //higher values means faster rotation
    public float rotationMinDistance = 0.1f;  //a value means no rotation if the distance from mouse and middle of the player is lower.
    public float Health = 100f;
    public float Lifes = 1f;
    public float MaxHP = 100f;
    public float Armour = 1f; //Minimum 1. It can be as high as you much but don't forget (real dmg = dmg/Armour)
    public bool isDead = false; //M: I need this to get rid of multiple collisions with the same enemy.

    //For movement+rotation of rigidbody rb
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 aimDirection;
    private Vector2 mousePosition;

    //Icon for Death
    public Sprite deathSprite; // assign a cross/grave sprite in the inspector
    private SpriteRenderer spriteRenderer;

    //Stuff for Traps layer
    private readonly float trapDmg =0.10f; //10%
    private bool isInTrap = false;
    private readonly float trapDmgTimer = 1f;
    private float lastDamageTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();   //using rigidbody to get a non buggy collision 
    }
   
    void Update()
    {
        if(isInTrap && Time.time >= lastDamageTime + trapDmgTimer) //if inside the trap do dmg every second
        {
            GetTrapDamage();
            lastDamageTime = Time.time; // Reset the timer
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveHorizontal, moveVertical).normalized;

        //getting mousePosition
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        //movement:
        rb.linearVelocity = new Vector2(movement.x*speed, movement.y*speed);

        //rotation
        float distance2 = Vector2.Distance(gun.position, rb.position); //distance between the gun and player
        float distance3 = Vector2.Distance(mousePosition, rb.position); //distance between player and mouse cursor
        aimDirection = (mousePosition - rb.position).normalized;    //player's center to mouse

        //GUN is pointing at the mouse cursor if (distance between the gun and player) is less than (distance between mouse and player) plus a small error (0.1f)
        //otherwise it is gonna point center of player towards the gun (taking the aimDirection from above)
        if (distance2 < distance3-0.1f)
        {
            aimDirection = (mousePosition - new Vector2(gun.position.x,gun.position.y)).normalized;
        }
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;      //to calculate the angle between mouse and gun/player
        angle = Mathf.LerpAngle(rb.rotation, angle, RotationSpeed * Time.fixedDeltaTime); //added RotationSpeed so it turns slowly/fast


        //threshhold so under 0.1f (default) distance between mouse and player's center you don't turn
        if (distance3 > rotationMinDistance)    
            rb.MoveRotation(angle);     //rotates the rb of player to the angle we calculated
    }

   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            isInTrap = true;
            speed /= 2;
            GetTrapDamage();
            lastDamageTime = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Trap")) 
        {
            isInTrap = false;
            speed *= 2;
        }
    }

    private void GetTrapDamage()
    {
        Health -= MaxHP * trapDmg;
        if (LevelManager.Instance) LevelManager.Instance.HPstat();
        if (Health < 0) Die();
    }





    //in case it dies. Show a gravestone and deactivate eveything
    public void Die()
    {
        if (isDead) return;
        
        if(Lifes > 0){ //don't die if the player has some lifes left
            Lifes--;
            Health = MaxHP;
            if(LevelManager.Instance){
                LevelManager.Instance.HPstat();
                LevelManager.Instance.LifeStat();
            }
        }
        else //no lifes left => die
        {
            isDead = true;

            if (spriteRenderer != null && deathSprite != null)
            {
                spriteRenderer.sprite = deathSprite;
                spriteRenderer.sortingLayerName = "Default"; // Ensure it's in the default layer
                spriteRenderer.sortingOrder = -1;
            }

            //rotate the death picture and make it bigger
            transform.right = Vector3.right;
            transform.localScale = new Vector3(2, 2, 2);

            // Lock Rigidbody
            if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // disable collider
            if (TryGetComponent<Collider2D>(out Collider2D col)) col.enabled = false;

            // disable playerAttack script
            if (TryGetComponent<PlayerAttack>(out PlayerAttack playerAttack)) playerAttack.enabled = false;

            if (LevelManager.Instance) LevelManager.Instance.PlayerKilled();
            // disable this script
            enabled = false;
        
        }
    }
}
