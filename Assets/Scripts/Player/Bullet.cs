using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float energy = 10;

    private void Update() {
        if(energy <= 0) {
            Destroy(gameObject);
        }
    }
}
