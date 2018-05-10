using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5;

    Rigidbody2D body;

    Vector2 moveVelocity;

    Camera mainCamera; 

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        mainCamera = FindObjectOfType<Camera>();
	}

    void FixedUpdate() {
        body.velocity = moveVelocity;
    }

    // Update is called once per frame
    void Update() {
        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
    }
}
