using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    [HideInInspector]
    public List<MapRegion> regions;
    [HideInInspector]
    public MapTile[,] tiles;

    PlayerController player;

	// Use this for initialization
	void Start () {
        regions = new List<MapRegion>();

        player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public Vector2 GetPlayerSpawnPosition() {
        Vector2 spawnPosition = new Vector2();

        foreach(MapRegion region in regions) {
            if(region.type == MapRegion.TypeRegion.PLAYER_SPAWN) {
                spawnPosition = region.GetRandomPoint();
            }
        }

        //TO REMOVE
        List<MapTile> freeTile = new List<MapTile>();

        foreach(MapTile t in tiles) {
            if(!t.isSolid) {
                freeTile.Add(t);
            }
        }

        spawnPosition = freeTile[Random.Range(0, freeTile.Count)].position;

        return spawnPosition;
    }
}

//Represents a tile of a map, does not take in count the layer of tilemap
public class MapTile {
    public Vector2Int position;

    float cost;

    Tile tile;

    int score;

    public bool isSolid;

    public MapTile(Vector2Int pos, bool solid) {
        position = pos;
        isSolid = solid;
    }

    public MapTile(Vector2Int pos, float c, Tile t, int s = 0) {
        position = pos;
        cost = c;
        tile = t;
        score = s;
    }
}

//Represents a region of a map with different rules
public class MapRegion {

    #region Variables

    public List<MapTile> tiles;
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

    public MapRegion(List<MapTile> t, TypeRegion type = TypeRegion.CLOSED) {
        tiles = t;

        centerOfRegion = tiles[Random.Range(0, tiles.Count)].position;

        SetType(type);
    }

    #endregion

    #region Public methods

    public void SetType(TypeRegion t) {
        type = t;

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

    public Vector2Int GetRandomPoint() {
        return tiles[Random.Range(0, tiles.Count)].position;
    }

    #endregion
}
