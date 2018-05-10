using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour {

    MapAutomata mapGenerator;
    NavigationAI navigationGraph;

    public Tilemap solidTilemap;
    public Tilemap groundTilemap;

    // Use this for initialization
    void Start () {
        mapGenerator = FindObjectOfType<MapAutomata>();
        navigationGraph = FindObjectOfType<NavigationAI>();

        mapGenerator.solidTilemap = solidTilemap;
        mapGenerator.groundTilemap = groundTilemap;

        navigationGraph.solidTilemap = solidTilemap;

        mapGenerator.GenerateMap();
        navigationGraph.GenerateNavigationGraph();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
