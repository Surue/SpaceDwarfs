using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

    public int levelFinished = 0;

    int monsterKilledTotal = 0;
    int monsterKilledCurrentLevel = 0;

    int scoreTotal = 0;
    int scoreCurrentLevel = 0;

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

    public void NewLevel() {
        monsterKilledCurrentLevel = 0;
    }

    public void AddScore(int s) {
        scoreCurrentLevel += s;
        scoreTotal += s;
    }

    public void MonsterKilled() {
        monsterKilledTotal++;
        monsterKilledCurrentLevel++;
    }

    public void LevelFinised() {
        levelFinished++;
    }
}
