using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform player;
    public Vector3 offset;
    float width, height, playerHeight;
    Vector3 worldDimensions, center, focalpoint;
    float camX, camY, camZ;
    Character myPlayer;

    /* Colliders */
    public CameraCollisionsInfo collisions;
    public List<GameObject> ChildrenColliders;
    public CameraBounds topCollider;
    public CameraBounds botCollider;
    public CameraBounds leftCollider;
    public CameraBounds rightCollider;

	// Use this for initialization
	void Start () {

        /* CollisionInfo Deafults */
       collisions.isTouchingTop = false;
       collisions.isTouchingBot = false;
       collisions.isTouchingRight = false;
       collisions.isTouchingLeft = false;
       collisions.isMovingRight = false;
       collisions.isMovingUp = false;


        /* Set child colliders. */
        foreach (Transform child in transform) {
            if (child.tag == "CameraBoundingChild") {
                ChildrenColliders.Add(child.gameObject);
            }
        }

        topCollider = ChildrenColliders[0].GetComponent<CameraBounds>();
        botCollider = ChildrenColliders[1].GetComponent<CameraBounds>();
        leftCollider = ChildrenColliders[2].GetComponent<CameraBounds>();
        rightCollider = ChildrenColliders[3].GetComponent<CameraBounds>();

        topCollider.OnEdgeEnter += onTopCollisionEnter;
        botCollider.OnEdgeEnter += onBotCollisionEnter;
        leftCollider.OnEdgeEnter += onLeftCollisionEnter;
        rightCollider.OnEdgeEnter += onRightCollisionEnter;

        topCollider.OnEdgeExit += onTopCollisionExit;
        botCollider.OnEdgeExit += onBotCollisionExit;
        leftCollider.OnEdgeExit += onLeftCollisionExit;
        rightCollider.OnEdgeExit += onRightCollisionExit;


        GameObject myPlayerObj = GameObject.Find("Player");
        if(myPlayerObj != null){
            myPlayer = myPlayerObj.GetComponent<Character>();
        }

        player = myPlayer.transform;
        playerHeight = 2;

        /* Camera Properties*/
        worldDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -10));
        width = worldDimensions.x;
        height = worldDimensions.y;

        offset.x = 0;// width;
        offset.y = 0;// height / 2;//height;
        offset.z = 0;

        camX = 0.1f;
        camY = 11.1f;
        camZ = -10;
	}
	
	// Update is called once per frame
	void Update () {
        print(collisions.isTouchingLeft);

        if (!collisions.isTouchingLeft) {
            camX = player.transform.position.x;
        }
        //if(!collisions.isTouchingBot){
            camY = player.transform.position.y;
        //}

        transform.position = new Vector3(camX, camY, -10);
	}


    /** Called on Camera collision with boundingBox. **/
    void onTopCollisionEnter() {
        collisions.isTouchingTop = true;
    }
    void onBotCollisionEnter() {
        collisions.isTouchingBot = true;
        print("BOT!");
    }
    void onLeftCollisionEnter() {
        collisions.isTouchingLeft = true;
        print("LEFT!");
    }
    void onRightCollisionEnter() {
        collisions.isTouchingRight = true;
        print("RIGHT!");
    }

    /** Called on Player leaving collision with an object. **/
    void onTopCollisionExit() {
        collisions.isTouchingTop = false;
    }
    void onBotCollisionExit() {
        collisions.isTouchingBot = false;
    }
    void onLeftCollisionExit() {
        collisions.isTouchingLeft = false;
    }
    void onRightCollisionExit() {
        collisions.isTouchingRight = false;
    }



}

public struct CameraCollisionsInfo {
    public bool isTouchingTop;
    public bool isTouchingBot;
    public bool isTouchingRight;
    public bool isTouchingLeft;

    public bool isMovingRight;
    public bool isMovingUp;
}
