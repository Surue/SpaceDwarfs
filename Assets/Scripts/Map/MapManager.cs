using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour {

    [SerializeField]
    MapAutomata mapGenerator;

    [SerializeField]
    MapController mapController;

    public Tilemap solidTilemap;
    public Tilemap groundTilemap;

    List<Vector2Int> emptyTiles;

    CompositeCollider2D colliderSolideTilemap;

    public enum Step {
        IDLE,
        GENERATING_MAP,
        DIGGING_TUNNEL,
        REGION_GENERATION,
        REGION_ASSOCIATION,
        ADD_ORE,
        ADD_TREASURE,
        ASSOCIATE_TILES,
        FINISH
    }

    public Step step = Step.IDLE;

    bool stepStarted = false;

    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(step);

        switch(step) {
            case Step.IDLE:
                break;

            case Step.GENERATING_MAP:
                if(!stepStarted) {
                    stepStarted = true;

                    if(!mapGenerator.isGenerating) {
                        if(solidTilemap.cellBounds.x == 0) {
                            StartCoroutine(mapGenerator.GenerateMap(mapController.tiles));
                        }
                    }
                }

                if(!mapGenerator.isGenerating) {
                    stepStarted = false;
                    step = Step.DIGGING_TUNNEL;
                }
                break;

            case Step.DIGGING_TUNNEL:
                if(!stepStarted) {
                    stepStarted = true;
                    StartCoroutine(mapGenerator.DigBetweenRegions(mapController.tiles));
                }

                if(!mapGenerator.isGenerating) {
                    stepStarted = false;
                    step = Step.REGION_GENERATION;
                }
                break;

            case Step.REGION_GENERATION:
                if(!stepStarted) {
                    stepStarted = true;
                    StartCoroutine(mapGenerator.GetRegions(mapController.tiles));
                }

                if(!mapGenerator.isGenerating) {
                    stepStarted = false;
                    step = Step.REGION_ASSOCIATION;
                }
                break;

            case Step.REGION_ASSOCIATION:
                if(!stepStarted) {
                    stepStarted = true;
                    StartCoroutine(mapGenerator.AssociateRegions(mapController.tiles, mapController.regions));
                }

                if(!mapGenerator.isGenerating) {
                    stepStarted = false;
                    step = Step.ASSOCIATE_TILES;
                }
                break;

            case Step.ASSOCIATE_TILES:
                if(!stepStarted) {
                    stepStarted = true;
                    StartCoroutine(mapGenerator.AssociateTiles(mapController.tiles));
                }

                if(!mapGenerator.isGenerating) {
                    mapController.DrawAll();
                    stepStarted = false;
                    step = Step.FINISH;
                }
                break;

            case Step.FINISH:
                break;
        }
	}

    public void StartGeneratingMap() {
        step = Step.GENERATING_MAP;
    }

    public void ClearMap() {
        solidTilemap.ClearAllTiles();
        groundTilemap.ClearAllTiles();

        solidTilemap.size = new Vector3Int(0, 0, 0);
        groundTilemap.size = new Vector3Int(0, 0, 0);
    }
}
