using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [SerializeField]
    SpriteRenderer hands;

    [SerializeField]
    Weapon primaryGun;
    [SerializeField]
    Weapon miningTool;

    Weapon activeGun;

    float delay = 0.0f;

    SpriteRenderer spriteRenderer;

    PlayerController player;

    bool isFireing = false;

    bool lookingRight = true;

	// Use this for initialization
	void Start () {
        if(primaryGun == null) {
            primaryGun = WeaponGenerator.Instance.GenerateWeapon();
        }

        activeGun = primaryGun;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = activeGun.spriteRight;
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
        if(!lookingRight) {
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

        //Change active weapon
        if(Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.Alpha1)) {
            if(activeGun != primaryGun) {
                activeGun = primaryGun;

                if(lookingRight) {
                    spriteRenderer.sprite = activeGun.spriteRight;
                } else {
                    spriteRenderer.sprite = activeGun.spriteLeft;
                }
            }
        } else if(Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.Alpha2)) {
            if(activeGun != miningTool) {
                activeGun = miningTool;

                if(lookingRight) {
                    spriteRenderer.sprite = activeGun.spriteRight;
                } else {
                    spriteRenderer.sprite = activeGun.spriteLeft;
                }
            }
        }


        if(Input.GetButtonDown("Fire1")) {
            isFireing = true;
        }

        if(Input.GetButton("Fire1")) {
            if(delay <= 0) {
                CameraShake.Instance.ShakeOnce(2, 2, 0.1f, 0.1f);

                activeGun.Attack(transform.position + (lookPos.normalized * 0.55f), lookPos.normalized) ;
                delay = activeGun.delayBetweenShots;
            }
        }

        if(Input.GetButtonUp("Fire1")) {
            isFireing = false;
        }
    }

    public void FlipSprite(bool flip) {
        if(!flip) {
            spriteRenderer.sprite = activeGun.spriteRight;
            lookingRight = true;
        } else {
            spriteRenderer.sprite = activeGun.spriteLeft;
            lookingRight = false;
        }
    }
}
