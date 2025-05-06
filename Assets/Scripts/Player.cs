using System.Diagnostics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public Transform gun;

    //30.04 by A: added the following public floats to have control on rotation speed and min distance that player starts rotating
    public float speed = 5f;
    public float RotationSpeed = 10f;   //higher values means faster rotation
    public float rotationMinDistance = 0.1f;  //a value means no rotation if the distance from mouse and middle of the player is lower.
    public float Health = 100f;
    //Minimum 0 
    public float Armour = 1f; //Minimum 1. It can be as high as you much but don't forget (real dmg = dmg/Armour)
    public bool isDead = false; //M: I need this to get rid of multiple collisions with the same enemy.

    //30.04 by A: added the followings for movement+rotation of rigidbody rb
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 aimDirection;
    private Vector2 mousePosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   //using rigidbody to get a non buggy collision 
    }

    // Update is called once per frame
   
    void Update()
    {

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        //30.04 by Arshia: changed it so it is normalized. removed "Vector2" from beginning because it is already inizialized above
        movement = new Vector2(moveHorizontal, moveVertical).normalized;

        //30.04 by Arshia: removed transform because it was causing teleportations with walls and etc. now I am using Rigidbody (rb). Look under FixedUpdate()
        // transform.Translate(movement * speed * Time.deltaTime, Space.World);


        //getting mousePosition
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //30.04 by Arshia: not needed because I am using rigidbody for moving now. so this got removed
        //if (movement != Vector2.zero)
        //{
        //    float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        //    transform.rotation = Quaternion.Euler(0, 0, angle);
        //}

    }


    //30.04 By Arshia: added FixedUpdate to fix some collision/teleporting problems. rb upadtes at a fixed speed (50hz) so it needs to be under "fixedupdate"
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
}
