using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour {

    MapManager mapManager;
    NavigationAI navigationGraph;

    public Tilemap solidTilemap;
    public Tilemap groundTilemap;

    public GameObject playerPrefab;
    public GameObject goblingPrefab;

    // Use this for initialization
    void Start () {
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
            if(!FindObjectOfType<Movement>())
                Instantiate(goblingPrefab, mapManager.GetPositionForSpawn(), Quaternion.identity);
        }
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
	void Update () {
    }
}
