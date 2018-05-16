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
        REGION_ASSOCIATION,
        ADD_ORE,
        ADD_TREASURE,
        ASSOCIATE_TILES,
        FINISH
    }

    public Step step = Step.IDLE;

    bool stepStarted = false;

    void Start () {
		////Check if need to generate map
  //      if(solidTilemap.cellBounds.x == 0){
  //          mapGenerator.GenerateMap(solidTilemap, groundTilemap);
  //      }

  //      //Find empty tile (use for spawn)
  //      emptyTiles = new List<Vector2Int>();

  //      for(int x = 0;x < solidTilemap.cellBounds.size.x; x++) {
  //          for(int y = 0;y < solidTilemap.cellBounds.size.y; y++) {
  //              if(!solidTilemap.HasTile(new Vector3Int(x, y, 0))) {
  //                  emptyTiles.Add(new Vector2Int(x, y));
  //              }
  //          }
  //      }
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
                            mapController.tiles = mapGenerator.GenerateMap(mapController.tiles);
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
                    mapController.tiles = mapGenerator.DigBetweenRegions(mapController.tiles);
                }

                if(!mapGenerator.isGenerating) {
                    stepStarted = false;
                    step = Step.REGION_ASSOCIATION;
                }
                break;

            case Step.REGION_ASSOCIATION:
                if(!stepStarted) {
                    stepStarted = true;
                    mapController.regions = mapGenerator.GetRegions(mapController.tiles);
                }

                if(!mapGenerator.isGenerating) {
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
