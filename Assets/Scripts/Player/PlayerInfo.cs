using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

    public int levelFinished = 0;

    int monsterKilled = 0;

    string namePlayer = "Belfrog";

    static PlayerInfo instance;
    public static PlayerInfo Instance {
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
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MonsterKilled() {
        monsterKilled++;
    }

    public void LevelFinised() {
        levelFinished++;
    }
}
