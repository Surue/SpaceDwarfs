using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed = 5;
    [Header("Health")]
    public float life = 100;
    float initialLife;
    Image lifeBar;

    Rigidbody2D body;

    Vector2 moveVelocity;

    Collider2D collider;

    //Animation
    Animator animatorController;
    [SerializeField]
    SpriteRenderer hands;
    bool lookingRight = true;

    //Gun
    GunController gun;

    public enum State {
        BLOCKED,
        NOT_BLOCKED
    }

    public State state = State.BLOCKED;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody2D>();

        animatorController = GetComponentInChildren<Animator>();

        gun = GetComponentInChildren<GunController>();

        lifeBar = GameObject.Find("lifeBar").GetComponent<Image>();

        initialLife = life;

        collider = GetComponent<Collider2D>();
    }

    void FixedUpdate() {
        body.velocity = moveVelocity;
    }

    // Update is called once per frame
    void Update() {
        if(state == State.BLOCKED) {
            Debug.Log("ICI");
            collider.enabled = false;
            return;
        } else {
            collider.enabled = true;
        }

        moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);

        if(lookPos.x > transform.position.x && !lookingRight) {
            animatorController.SetBool("lookingRight", true);
            lookingRight = true;
            hands.flipX = false;
            gun.FlipSprite(false);
        } else if (lookPos.x < transform.position.x && lookingRight) {
            animatorController.SetBool("lookingRight", false);
            lookingRight = false;
            hands.flipX = true;
            gun.FlipSprite(true);
        }
    }

    void ManageHandsAnimation() {
        if(lookingRight) {

        }
    }

    public void TakeDamage(float d) {
        life -= d;
        UpdateLifeBar();
        if(life < 0) {
            GameManager.Instance.PlayerDeath();
        }
    }

    void UpdateLifeBar() {
        lifeBar.fillAmount = 1 / (initialLife / life);
    }
}
