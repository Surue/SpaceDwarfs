using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    List<MapRegion> regions;
    MapTile[,] tiles;

    PlayerController player;

	// Use this for initialization
	void Start () {
        regions = new List<MapRegion>();

        player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddRegions(MapRegion region) {
        regions.Add(region);
    }

    public Vector2 GetPlayerSpawnPosition() {
        Vector2 spawnPosition = new Vector2();

        foreach(MapRegion region in regions) {
            if(region.type == MapRegion.TypeRegion.PLAYER_SPAWN) {
                spawnPosition = region.GetRandomPoint();
            }
        }

        return spawnPosition;
    }
}

//Represents a tile of a map, does not take in count the layer of tilemap
public class MapTile {
    public MapTile(Vector2Int pos, float c, Tile t, int s = 0) {
        position = pos;
        cost = c;
        tile = t;
        score = s;
    }

    Vector2Int position;

    float cost;

    Tile tile;

    int score;
}

//Represents a region of a map with different rules
public class MapRegion {

    #region Variables

    public List<Vector2Int> listTiles;
    public Vector2Int centerOfRegion = new Vector2Int(0, 0);

    //Type of region
    public enum TypeRegion {
        PLAYER_SPAWN,
        CLOSED,
        NORMAL,
        NEST
    }

    public TypeRegion type;

    //Rules
    public bool canSpawnMonster;
    public bool canSpawnPlayer;
    public bool canSpawnNest;

    #endregion

    #region Constructor

    public MapRegion(List<Vector2Int> tiles, TypeRegion type = TypeRegion.NORMAL) {
        listTiles = tiles;

        centerOfRegion = tiles[Random.Range(0, tiles.Count)];

        switch(type) {
            case TypeRegion.PLAYER_SPAWN:
                canSpawnMonster = false;
                canSpawnPlayer = true;
                canSpawnNest = false;
                break;

            case TypeRegion.CLOSED:
                canSpawnNest = false;
                canSpawnPlayer = false;
                canSpawnMonster = false;
                break;

            case TypeRegion.NORMAL:
                canSpawnMonster = true;
                canSpawnPlayer = false;
                canSpawnNest = false;
                break;

            case TypeRegion.NEST:
                canSpawnMonster = true;
                canSpawnPlayer = false;
                canSpawnNest = true;
                break;
        }
    }

    #endregion

    #region Public methods

    public Vector2Int GetRandomPoint() {
        return listTiles[Random.Range(0, listTiles.Count)];
    }

    #endregion
}
