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
    public GameObject goblingPrefab;

    //Variables related to time before evacuation
    float timeBeforeEvacuationInSeconds = 0;
    float minimumTimeBeforeEvacInSeconds = 60;
    float levelStepForTime = 5.0f;
    float timePerStepInSeconds = 30;
    [SerializeField]
    Text timeBeforeEvacText;

    // Use this for initialization
    void Start() {
        //Get all object needed
        mapManager = FindObjectOfType<MapManager>();
        navigationGraph = FindObjectOfType<NavigationAI>();

        navigationGraph.solidTilemap = mapManager.solidTilemap;

        if(mapManager.solidTilemap.cellBounds.x == 0) {
            StartCoroutine(WaitEndGeneration());
        } else {
            navigationGraph.GenerateNavigationGraph();

            //Spawn player
            if(!FindObjectOfType<PlayerController>()) //Used to not spawn player if already in the map
                Instantiate(playerPrefab, mapManager.GetPositionForSpawn(), Quaternion.identity);
        }

        //Compute time before evacuation in level
        timeBeforeEvacuationInSeconds = Mathf.Floor(PlayerInfo.Instance.levelFinished / levelStepForTime) * timePerStepInSeconds + minimumTimeBeforeEvacInSeconds;

        StartCoroutine(ClockAnimation());
    }

    //Not Happy with that
    IEnumerator WaitEndGeneration() {

        while(mapManager.solidTilemap.size.x == 0) {
            yield return new WaitForFixedUpdate();
        }

        navigationGraph.GenerateNavigationGraph();

        //Spawn player
        if(!FindObjectOfType<PlayerController>()) //Used to not spawn player if already in the map
            Instantiate(playerPrefab, mapManager.GetPositionForSpawn(), Quaternion.identity);
        if(!FindObjectOfType<Movement>())
            Instantiate(goblingPrefab, mapManager.GetPositionForSpawn(), Quaternion.identity);
    }

    // Update is called once per frame
    void Update() {
        UpdateUIText();
    }

    IEnumerator ClockAnimation() {
        while(timeBeforeEvacuationInSeconds > 0) {
            timeBeforeEvacuationInSeconds -= 1;
            UpdateUIText();

            yield return new WaitForSeconds(1);
        }
    }

    void UpdateUIText() {
        string s = "";
        s = ((int)timeBeforeEvacuationInSeconds / 60).ToString();

        s += " : ";
        if((timeBeforeEvacuationInSeconds % 60) < 10) {
            s += "0" + (timeBeforeEvacuationInSeconds % 60).ToString();
        } else {
            s += (timeBeforeEvacuationInSeconds % 60).ToString();
        }

        timeBeforeEvacText.text = s;
    }
}
