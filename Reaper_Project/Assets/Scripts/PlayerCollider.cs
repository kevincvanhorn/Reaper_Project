using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour {
    public bool isTouching; // True if collider is touching an object.

    // Use this for initialization
    void Start() {
        isTouching = false;
    }

    // Update is called once per frame
    void Update() {
    }

    /** Called on Player collision with object. **/
    void OnTriggerEnter2D(Collider2D coll) {
        isTouching = true;
    }

    /** Called on Player collision with object. **/
    void OnTriggerExit2D() {
        isTouching = false;
    }
}
