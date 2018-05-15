using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [SerializeField]
    Sprite spriteRight;
    [SerializeField]
    Sprite spriteLeft;
    [SerializeField]
    SpriteRenderer hands;
    [SerializeField]
    GameObject ammunition;

    float delayBetweenShots = 0.15f;
    float delay = 0.0f;

    SpriteRenderer spriteRenderer;

    PlayerController player;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteRight;
        player = transform.parent.parent.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if(player.state == PlayerController.State.BLOCKED) return;
        //Gets mouse position, you can define Z to be in the position you want the weapon to be in
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        if(spriteRenderer.sprite == spriteLeft) {
            angle -= 180;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            hands.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        } else {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            hands.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if(delay > 0) {
            delay -= Time.deltaTime;
        }

        if(Input.GetButton("Fire1")) {
            if(delay <= 0) {
                CameraShake.Instance.ShakeOnce(2, 2, 0.1f, 0.1f);
                var bullet = (GameObject)Instantiate(ammunition, transform.position + (lookPos.normalized * 0.55f) + new Vector3(0, -0.1f ,0), Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = lookPos.normalized * 5.75f;
                delay = delayBetweenShots;
            } 
        }
    }

    public void FlipSprite(bool flip) {

        if(!flip) {
            spriteRenderer.sprite = spriteRight;
        } else {
            spriteRenderer.sprite = spriteLeft;
        }
    }
}
