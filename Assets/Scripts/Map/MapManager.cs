using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour {

    [SerializeField]
    MapAutomata mapGenerator;

    public Tilemap solidTilemap;
    public Tilemap groundTilemap;

    List<Vector2Int> emptyTiles;

    CompositeCollider2D colliderSolideTilemap;

    // Use this for initialization
    void Start () {
		//Check if need to generate map
        if(solidTilemap.cellBounds.x == 0){
            mapGenerator.GenerateMap(solidTilemap, groundTilemap);
        }

        //Find empty tile (use for spawn)
        emptyTiles = new List<Vector2Int>();

        for(int x = 0;x < solidTilemap.cellBounds.size.x; x++) {
            for(int y = 0;y < solidTilemap.cellBounds.size.y; y++) {
                if(!solidTilemap.HasTile(new Vector3Int(x, y, 0))) {
                    emptyTiles.Add(new Vector2Int(x, y));
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClearMap() {
        solidTilemap.ClearAllTiles();
        groundTilemap.ClearAllTiles();

        solidTilemap.size = new Vector3Int(0, 0, 0);
        groundTilemap.size = new Vector3Int(0, 0, 0);
    }

    public Vector3 GetPositionForSpawn() {
        Vector2 pos = emptyTiles[Random.Range(0, emptyTiles.Count)];

        return new Vector3(pos.x + solidTilemap.cellSize.x / 2.0f + solidTilemap.cellBounds.x, pos.y + solidTilemap.cellSize.y / 2.0f + solidTilemap.cellBounds.y);
    }
}
