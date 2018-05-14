using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelManager:MonoBehaviour {

    MapManager mapManager;
    NavigationAI navigationGraph;

    public Tilemap solidTilemap;
    public Tilemap groundTilemap;

    public GameObject playerPrefab;

    //Variables for landing
    float timeLanding = 5.0f;

    //Variables related to time before evacuation
    float timeBeforeEvacuationInSeconds = 0;
    float minimumTimeBeforeEvacInSeconds = 60;
    float levelStepForTime = 5.0f;
    float timePerStepInSeconds = 30;
    [SerializeField]
    Text timeBeforeEvacText;

    //Variables for evacuation
    Vector3 evacuationPosition;
    bool evacuationAvailable = false;

    //Logic variables & player
    GameObject player;

    public enum State {
        LANDING,
        WAIT_FOR_EVACUATION,
        EVACUATION_READY,
        EVACUATE
    }

    [SerializeField]
    public State state = State.LANDING;

    // Use this for initialization
    void Start() {
        //Get all object needed
        mapManager = FindObjectOfType<MapManager>();
        navigationGraph = FindObjectOfType<NavigationAI>();

        navigationGraph.solidTilemap = mapManager.solidTilemap;

        //Compute time before evacuation in level
        timeBeforeEvacuationInSeconds = Mathf.Floor(PlayerInfo.Instance.levelFinished / levelStepForTime) * timePerStepInSeconds + minimumTimeBeforeEvacInSeconds;
    }

    // Update is called once per frame
    void Update() {
        switch(state) {
            case State.LANDING:
                if(!FindObjectOfType<PlayerController>()) {
                    player = Instantiate(playerPrefab, mapManager.GetPositionForSpawn(), Quaternion.identity);
                    player.GetComponent<PlayerController>().enabled = false;
                } else {
                    player = GameObject.Find("Player");
                }

                if(timeLanding > 0) {
                    timeLanding -= Time.deltaTime;
                } else {
                    state = State.WAIT_FOR_EVACUATION;
                    FindObjectOfType<PlayerController>().enabled = true; //TO CHANGE
                    navigationGraph.GenerateNavigationGraph();
                    StartCoroutine(ClockAnimation());
                }
                break;

            case State.WAIT_FOR_EVACUATION:
                if(evacuationAvailable) {
                    UpdateUIText("Evacuation");
                    state = State.EVACUATION_READY;
                }
                break;

            case State.EVACUATION_READY:
                if(Vector2.Distance(player.transform.position, evacuationPosition) < 1.0f) {
                    state = State.EVACUATE;
                    player.GetComponent<PlayerController>().enabled = false;
                }
                break;

            case State.EVACUATE:
                break;
        }
    }

    IEnumerator ClockAnimation() {
        while(timeBeforeEvacuationInSeconds > 0) {
            timeBeforeEvacuationInSeconds -= 1;

            string s = "";
            s = ((int)timeBeforeEvacuationInSeconds / 60).ToString();

            s += " : ";
            if((timeBeforeEvacuationInSeconds % 60) < 10) {
                s += "0" + (timeBeforeEvacuationInSeconds % 60).ToString();
            } else {
                s += (timeBeforeEvacuationInSeconds % 60).ToString();
            }

            UpdateUIText(s);

            yield return new WaitForSeconds(1);
        }

        evacuationAvailable = true;
    }

    void UpdateUIText(string s) {
        timeBeforeEvacText.text = s;
    }
}
