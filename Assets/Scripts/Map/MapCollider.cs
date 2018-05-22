using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCollider : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<Bullet>()) {
            if(!collision.gameObject.GetComponent<Bullet>().bounce) {
                collision.gameObject.GetComponent<Bullet>().damage = 0;
            }
            else{
                collision.gameObject.GetComponent<Bullet>().damage /= 2f;
            }
        }
    }
}
