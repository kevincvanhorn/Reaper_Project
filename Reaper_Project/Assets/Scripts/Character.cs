using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour {
    public LayerMask collisionMask;

    //public PolygonCollider2D collider; //Update: will be an array of colliders
        //note: the collider2D is solely used for the rigidBody calculations.
    public Rigidbody2D rigidBody; // Not Kinematic: moves not by transform, but by physics

    public CollisionInfo collisions;

    // Jump Vars
    public float moveSpeed = 10; // Horizontal
    public float sprintSpeed = 20;
    public float activeSpeed;
    public float lateralAccelGrounded = 60;
    public float lateralAccelAirborne = 60;

    public float jumpHeightMax = 5;
    public float jumpHeightMin = .9f;
    public float timeToJumpApex = .4f;

    float directionFacing = 1;
    float gravity;
    float jumpVelocityMax;
    float jumpVelocityMin;
    Vector3 velocity;
    private int rotation = 0;

    void Start() {
        collisions.isTouchingTop = false;
        collisions.isTouchingRight = false;
        collisions.isTouchingLeft = false;

        activeSpeed = moveSpeed;
        collisions.isRightPressed = false;
        collisions.isLeftPressed = false;
        collisions.isGrounded = false;
        collisions.isSprinting = false;

        collisions.isRotated = false;

        //collider = GetComponent<PolygonCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        

        // Calc constants in terms of Jump time and apex height
        gravity = -(2 * jumpHeightMax) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocityMax = Mathf.Abs(gravity * timeToJumpApex);
        jumpVelocityMin = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeightMin);
    }

    // Update is called once per frame
    void Update() {
        calcJump();
    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Platform") {
            //Vector2 dir2 = coll.contacts[0].point; // How to draw debug lines
            //Debug.DrawLine(transform.position, (Vector2)transform.position - dir2, Color.red, 100);
            //Debug.DrawRay(dir2, dir*2, Color.red, 100);
            Vector2 dir = coll.contacts[0].normal;

            if (Mathf.Round(dir.y) == 1) { // Bottom Collision
                velocity.y = 0;
                collisions.isGrounded = true;
            }
            else if (Mathf.Round(dir.y) == -1) { // Top Collisions
                velocity.y = 0;
                collisions.isTouchingTop = true;
            }
            if (Mathf.Round(dir.x) == 1) { // Left Collisions
                collisions.isTouchingLeft = true;
                collisions.onWall = true;
            }
            else if (Mathf.Round(dir.x) == -1) { // Right Collisions       
                collisions.isTouchingRight = true;
                collisions.onWall = true;
            }
            float slopeAngle = Vector2.Angle(dir, Vector2.up);

            /*if (slopeAngle < 60){
                transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                collisions.isRotated = true;
            }*/
                
            //print("Enter: "+dir); //flat wall = (1,0) LSteep = (1,.3)
            //print(Vector2.Angle(transform.position, dir));
        }
    }

    void OnCollisionExit2D(Collision2D coll) {
        if (coll.gameObject.tag == "Platform") {
            //collisions.isGrounded = false;

            Vector2 dir = coll.contacts[0].normal;
            print("Exit: " + dir);

            if (Mathf.Round(dir.y) == 1) { // Bottom Collision
                collisions.isGrounded = false;
            }
            else if (Mathf.Round(dir.y) == -1) { // Top Collisions
                collisions.isTouchingTop = false;
            }
            else if (Mathf.Round(dir.x) == 1) { // Left Collisions
                collisions.isTouchingLeft = false;
                collisions.onWall = false;
            }
            else if (Mathf.Round(dir.x) == -1) { // Right Collisions
                collisions.isTouchingRight = false;
                collisions.onWall = false;
            }

            /*if (collisions.isRotated)
            {
                transform.rotation = Quaternion.FromToRotation(dir, Vector3.up);
                collisions.isRotated = false;
            }*/
        }

        
    }

    void calcJump() {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // Raw is no smoothing

        // Vertical / JUMP Calc ------------------------------------------
        if (Input.GetKeyDown(KeyCode.UpArrow)) {// replace with more general
            if(collisions.isGrounded){ // normal Jumps
                velocity.y = jumpVelocityMax;
                collisions.isGrounded = false;
            }
            else if(collisions.onWall){ // Wall Jumps // Note: to fix the sticking to wall part: check above equations with gravity - may need to use a different equation for gravity.
                if (collisions.isTouchingLeft){// && directionFacing == -1) {
                    velocity.y = jumpVelocityMax;
                    velocity.x = moveSpeed;
                }
                else if(collisions.isTouchingRight){
                    velocity.x = moveSpeed * -1;
                    velocity.y = jumpVelocityMax;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.UpArrow)) {
            if (velocity.y > jumpVelocityMin) { // Keep applying velocity up while key is pressed - variable jump
                velocity.y = jumpVelocityMin;
            }
        }

        if (!collisions.isGrounded) { // Apply Gravity every frame until grounded
            velocity.y += gravity * Time.deltaTime;
        }

        // Lateral Calc --------------------------------------------------
        if (Input.GetKeyDown(KeyCode.LeftShift)) { // Sprint - Start
            collisions.isSprinting = true;
            if (collisions.isGrounded) {
                activeSpeed = sprintSpeed;
                velocity.x = activeSpeed * input.x;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) { // Sprint - Stop
            collisions.isSprinting = false;
            activeSpeed = moveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !collisions.isRightPressed) { // on L/R input - setting conditions
            collisions.isRightPressed = true;
            collisions.isLeftPressed = false;
            directionFacing = 1;
            if (collisions.isGrounded) {
                velocity.x = activeSpeed;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !collisions.isLeftPressed) {
            collisions.isLeftPressed = true;
            collisions.isRightPressed = false;
            directionFacing = -1;
            if (collisions.isGrounded) {
                velocity.x = activeSpeed * -1; // has to be done bc input.x changes
            }

        }
        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            collisions.isRightPressed = false;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            collisions.isLeftPressed = false;
        }
        // X Acceleration
        if (collisions.isGrounded && (collisions.isRightPressed || collisions.isLeftPressed)) {
            velocity.x = activeSpeed * directionFacing; // Ground Sliding
        }
        else if (collisions.isGrounded && !collisions.isRightPressed && !collisions.isLeftPressed) { // On-release of Lateral Movement controls - Deccelerate
            if (directionFacing == 1 && velocity.x < 0 || directionFacing == -1 && velocity.x > 0) { // Stops deccel when hits 0 from the initial negative(left moving) or pos(right moving) val
                velocity.x = 0;
            }
            if (directionFacing == 1 && velocity.x > 0) { // Decceleration Right
                velocity.x -= lateralAccelGrounded * Time.deltaTime;
            }
            else if (directionFacing == -1 && velocity.x < 0) { // Decceleration Left
                velocity.x += lateralAccelGrounded * Time.deltaTime;
            }
        }
        else if (!collisions.isGrounded) { // Air Lateral Movement
            if (!collisions.onWall && (collisions.isTouchingLeft || collisions.isTouchingRight)) { // in-air, hitting a wall laterally
                velocity.x = 0;
                //print("HEY"+collisions.isTouchingRight+" "+collisions.isTouchingLeft);
            }
            else if (collisions.isRightPressed && velocity.x < activeSpeed) { // in-air lateral move right
                velocity.x += lateralAccelAirborne * Time.deltaTime;
            }
            else if (collisions.isLeftPressed && velocity.x > -activeSpeed) { // in-air lateral move left
                velocity.x -= lateralAccelAirborne * Time.deltaTime;
            }
            // Rotation
            if (collisions.isRotated && !collisions.isTouchingLeft && !collisions.isTouchingRight){
                transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.up);
                collisions.isRotated = false;
            }
        }

        //transform.rotation = Quaternion.FromToRotation((float)transform.rotation, (float)velocity.magnitude*Time.deltaTime);
        //if(transform.rotation.eulerAngles.z < 90)
            //transform.Rotate(directionFacing *Vector3.forward * activeSpeed *Time.deltaTime, Space.World);
        rigidBody.velocity = velocity;
        //rigidBody.AddTorque(transform.right);
        //print(collisions.isGrounded +" "+ collisions.isTouchingLeft +" "+ collisions.isTouchingRight);
    }
}

public struct CollisionInfo {
    public bool isGrounded; // Essentially touchingBot
    public bool isSprinting;
    public bool isRightPressed; // is a force right applied
    public bool isLeftPressed; // is a force left applied
    //note: 3 states- left, right, and still require two variables

    public bool isTouchingTop;
    public bool isTouchingRight;
    public bool isTouchingLeft;

    public bool onWall;
    public bool isRotated;
}