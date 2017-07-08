using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour {

    public bool isTouching;
    public delegate void TriggerEvent();
    public event TriggerEvent OnEdgeEnter;
    public event TriggerEvent OnEdgeExit;

	// Use this for initialization
	void Start () {
        isTouching = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter2D(Collider2D coll) {
        isTouching = true;
        OnEdgeEnter();
    }

    void OnTriggerStay2D(Collider2D coll) {
    }

    void OnTriggerExit2D() {
        isTouching = false;
        OnEdgeExit();
    }
}
