using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {

    List<Weapon> weapons;

    static InventoryController instance;
    public static InventoryController Instance {
        get {
            return instance;
        }
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);

        weapons = new List<Weapon>();
	}
}
