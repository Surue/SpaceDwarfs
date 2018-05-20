using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour {

    [Header("Monsters prefabs")]
    [SerializeField]
    GameObject crawlerPrefab;

    GameObject player;

    LevelManager levelManager;

    List<GameObject> activeMonstersList;

    MapController mapController;

    int levelFinised;

    int minMonsterForSearching;

    int difficultyLevel = 0;

    [Header("Rules")]
    public int rule_minDifficultyToGoBackToNest;
    public int rule_monDifficultyToGoBerserker;

    enum State {
        WAIT_FOR_LANDING,
        SPAWN_PACIFIC_MONSTER,
        SEARCHING_PLAYER,
        PLAYER_FOUNDED,
        BERSERKER,
        PLAYER_EVACUATING
    }

    State state = State.WAIT_FOR_LANDING;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");

        levelManager = FindObjectOfType<LevelManager>();

        activeMonstersList = new List<GameObject>();

        levelFinised = PlayerInfo.Instance.levelFinished;

        difficultyLevel = Mathf.FloorToInt(levelFinised / 2.0f) + 1;

        minMonsterForSearching = difficultyLevel + 4; minMonsterForSearching = 1;

        mapController = FindObjectOfType<MapController>();
    }
	
	// Update is called once per frame
	void Update () {
        switch(state) {
            case State.WAIT_FOR_LANDING:
                if(levelManager.state == LevelManager.State.WAIT_FOR_EVACUATION) {
                    state = State.SPAWN_PACIFIC_MONSTER;
                }
                break;

            case State.SPAWN_PACIFIC_MONSTER:

                state = State.SEARCHING_PLAYER;
                break;

            case State.SEARCHING_PLAYER:
                for(int i = 0; i < activeMonstersList.Count;i++) {
                    if(activeMonstersList[i] == null) {
                        activeMonstersList.RemoveAt(i);
                        i--;
                    }
                }

                if(activeMonstersList.Count < minMonsterForSearching) {
                    activeMonstersList.Add(Instantiate(crawlerPrefab, mapController.GetMonsterSpawnPosition(), Quaternion.identity)); //TO CHANGE
                }
                break;

            case State.PLAYER_FOUNDED:
                break;

            case State.BERSERKER:
                break;
        }
	}
}
