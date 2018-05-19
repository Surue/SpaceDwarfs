using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelManager:MonoBehaviour {

    MapManager mapManager;
    MapController mapController;
    NavigationAI navigationGraph;

    public Tilemap solidTilemap;
    public Tilemap groundTilemap;

    public GameObject playerPrefab;
    public GameObject spaceshipPrefab;

    //Variables for landing
    float timeLanding = 5.0f;

    //Variables related to time before evacuation
    float timeBeforeEvacuationInSeconds = 0;
    float minimumTimeBeforeEvacInSeconds = 30;
    float levelStepForTime = 5.0f;
    float timePerStepInSeconds = 30;
    [SerializeField]
    Text timeBeforeEvacText;

    //Variables for evacuation
    Vector3 evacuationPosition;
    bool evacuationAvailable = false;

    //Logic variables & player
    GameObject player;
    SpaceshipController spaceship;

    Vector2 spawnPosition;

    public enum State {
        MAP_GENERATION,
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
        mapController = FindObjectOfType<MapController>();
        mapManager = FindObjectOfType<MapManager>();
        navigationGraph = FindObjectOfType<NavigationAI>();

        //Compute time before evacuation in level
        timeBeforeEvacuationInSeconds = Mathf.Floor(PlayerInfo.Instance.levelFinished / levelStepForTime) * timePerStepInSeconds + minimumTimeBeforeEvacInSeconds;
    }

    // Update is called once per frame
    void Update() {
        switch(state) {
            case State.MAP_GENERATION:

                if(mapManager.step == MapManager.Step.IDLE) {
                    player = Instantiate(playerPrefab, new Vector2(-10, -10), Quaternion.identity);
                    player.GetComponent<PlayerController>().state = PlayerController.State.BLOCKED;

                    spaceship = Instantiate(spaceshipPrefab, new Vector2(-10, -10), Quaternion.identity).GetComponent<SpaceshipController>();
                    mapManager.StartGeneratingMap();
                }

                if(mapManager.step == MapManager.Step.FINISH) {
                    spawnPosition = mapController.GetPlayerSpawnPosition();
                    player.transform.position = spawnPosition + new Vector2(0, 10);
                    spaceship.transform.position = player.transform.position;
                    state = State.LANDING;
                }
                break;

            case State.LANDING:
                if(Vector2.Distance(player.transform.position, spawnPosition) > 0.1f) {
                    Vector3 speed = new Vector3(0, -(Vector2.Distance(player.transform.position, spawnPosition)),0); 

                    player.transform.position += Time.deltaTime * speed;
                    spaceship.transform.position += Time.deltaTime * speed;
                } else {
                    player.GetComponent<PlayerController>().state = PlayerController.State.NOT_BLOCKED;
                    spaceship.state = SpaceshipController.State.OPENING;
                    state = State.WAIT_FOR_EVACUATION;
                    FindObjectOfType<PlayerController>().enabled = true; //TO CHANGE
                    FindObjectOfType<PlayerController>().state = PlayerController.State.NOT_BLOCKED; //TO CHANGE
                    StartCoroutine(ClockAnimation());
                }
                break;

            case State.WAIT_FOR_EVACUATION:
                if(evacuationAvailable) {
                    UpdateUIText("Evacuation");
                    state = State.EVACUATION_READY;
                    spaceship.canTakeOff = true;
                }
                break;

            case State.EVACUATION_READY:
                if(spaceship.state == SpaceshipController.State.TAKING_OFF) {
                    player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    player.GetComponent<PlayerController>().state = PlayerController.State.BLOCKED;
                    state = State.EVACUATE;
                }
                break;

            case State.EVACUATE:
                if(Vector2.Distance(player.transform.position, spawnPosition) < 10f) {
                    Vector3 speed = new Vector3(0, (Vector2.Distance(player.transform.position, spawnPosition)),0);
                    
                    spaceship.transform.position += Time.deltaTime * speed;
                    player.transform.position = spaceship.transform.position;
                } else {
                    GameManager.Instance.LoadNextLevel();
                }
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
