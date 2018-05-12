using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5;
    public float life = 100;

    Rigidbody2D body;

    Vector2 moveVelocity;

    Camera mainCamera;

    //Animation
    Animator animatorController;
    [SerializeField]
    SpriteRenderer hands;
    bool lookingRight = true;

    //Gun
    GunController gun;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody2D>();
        mainCamera = FindObjectOfType<Camera>();

        animatorController = GetComponentInChildren<Animator>();

        gun = GetComponentInChildren<GunController>();
	}

    void FixedUpdate() {
        body.velocity = moveVelocity;
    }

    // Update is called once per frame
    void Update() {
        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);

        if(lookPos.x > transform.position.x && !lookingRight) {
            animatorController.SetBool("lookingRight", true);
            lookingRight = true;
            hands.flipX = false;
            gun.FlipSprite();
        } else if (lookPos.x < transform.position.x && lookingRight) {
            animatorController.SetBool("lookingRight", false);
            lookingRight = false;
            hands.flipX = true;
            gun.FlipSprite();
        }
    }

    void ManageHandsAnimation() {
        if(lookingRight) {

        }
    }

    public void TakeDamage(float d) {
        life -= d;

        if(life < 0) {
            Debug.Log("You die");
        }
    }
}
