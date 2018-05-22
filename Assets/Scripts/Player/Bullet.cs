﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float damage = 2;
    public bool bounce;

    private void Update() {
        if(damage <= 0.5f) {
            Destroy(gameObject);
        }
    }
}
