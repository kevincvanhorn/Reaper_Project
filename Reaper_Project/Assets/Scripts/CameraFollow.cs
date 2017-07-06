using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform player;
    public Vector3 offset;
    float width, height, playerHeight;
    Vector3 worldDimensions, center, focalpoint;

	// Use this for initialization
	void Start () {
        GameObject myPlayer = GameObject.Find("Player");
        player = myPlayer.transform;
        playerHeight = 2;

        /* Camera Properties*/
        //Vector2 topRightCorner = new Vector2(1, 1);
        //Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner); // Make 2D -> 3D vector.
        worldDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -10));
        width = worldDimensions.x;
        height = worldDimensions.y;

        offset.x = 0;// width;
        offset.y = 0;// height / 2;//height;
        offset.z = 0;
	}
	
	// Update is called once per frame
	void Update () {
        //print(player.transform.position + " " + transform.position);
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y+offset.y, -10);
        //transform.position = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z + offset.z);

	}
}
