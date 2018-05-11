using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour {

    MapAutomata mapGenerator;
    NavigationAI navigationGraph;

    public Tilemap solidTilemap;
    public Tilemap groundTilemap;

    public GameObject playerPrefab;
    public GameObject goblingPrefab;

    // Use this for initialization
    void Start () {
        //Get all object needed
        mapGenerator = FindObjectOfType<MapAutomata>();
        navigationGraph = FindObjectOfType<NavigationAI>();

        mapGenerator.solidTilemap = solidTilemap;
        mapGenerator.groundTilemap = groundTilemap;

        navigationGraph.solidTilemap = solidTilemap;

        //First generation
        mapGenerator.GenerateMap();
        navigationGraph.GenerateNavigationGraph();

        //Spawn player
        Instantiate(playerPrefab, mapGenerator.GetPositionForSpawn(), Quaternion.identity);
        Instantiate(goblingPrefab, mapGenerator.GetPositionForSpawn(), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
