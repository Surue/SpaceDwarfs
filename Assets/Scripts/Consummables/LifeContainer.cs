using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeContainer : MonoBehaviour {

    PlayerController player;
    Animator animator;

    public GameObject lifeUpPrefabs;

    float life = 1;
    public float lifePoint;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<Bullet>()) {

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            
            life -= bullet.energy;

            Destroy(bullet.gameObject);

            if(life <= 0) {
                player.AddLife(lifePoint);
                Score s = GetComponent<Score>();

                Instantiate(lifeUpPrefabs, player.transform.position, Quaternion.identity);

                if(s != null) {
                    s.DisplayScore();
                }

                animator.SetTrigger("Breaking");

                GetComponent<Collider2D>().enabled = false;

                Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0).Length);
            }
        }
    }
}
