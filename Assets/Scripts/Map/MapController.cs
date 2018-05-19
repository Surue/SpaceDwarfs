﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    [HideInInspector]
    public List<MapRegion> regions;
    [HideInInspector]
    public MapTile[,] tiles;

    PlayerController player;
    

    [SerializeField]
    Tilemap solidTilemap;

	// Use this for initialization
	void Start () {
        regions = new List<MapRegion>();

        player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void SetTiles(MapTile[,] mapTiles) {
        int width = mapTiles.GetLength(0);
        int height = mapTiles.GetLength(1);

        tiles = new MapTile[width, height];

        for(int x = 0; x < width;x++) {
            for(int y = 0; y < height; y++) {
                tiles[x, y] = new MapTile(mapTiles[x, y]);
            }
        }
    }

    public MapTile GetTile(Vector2Int pos) {
        if(pos.x >= 0 && pos.x < tiles.GetLength(0) && pos.y >= 0 && pos.y < tiles.GetLength(1)) {
            return tiles[pos.x, pos.y];
        }

        return null;
    }

    public void AttackTile(MapTile t, float damage) {
        if(t.Attack(damage)) {
            UpdateTile(t);
        }
    }

    void UpdateTile(MapTile t) {
        GetComponent<MapAutomata>().UpdateTile(tiles, t);
        FindObjectOfType<NavigationAI>().GenerateNavigationGraph(tiles);

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        foreach(Vector3Int b in bounds.allPositionsWithin) {
            if(t.position.x + b.x >= 0 && t.position.x + b.x < tiles.GetLength(0) && t.position.y + b.y >= 0 && t.position.y + b.y < tiles.GetLength(1)) {
                MapTile tile = tiles[t.position.x + b.x, t.position.y + b.y];
                solidTilemap.SetTile(new Vector3Int(tile.position.x, tile.position.y, 0), tile.tile);
            }
        }
    }

    public void DrawAll() {
        foreach(MapTile tile in tiles) {
            solidTilemap.SetTile(new Vector3Int(tile.position.x, tile.position.y, 0), tile.tile);
        }
    }

    public void ClearRegions() {
        regions.Clear();
    }

    public void AddRegion(MapRegion region) {
        regions.Add(new MapRegion(region));
    }

    public Vector2 GetMonsterSpawnPosition() {
        Vector2 spawnPosition = new Vector2();

        List<MapRegion> possibleRegion = new List<MapRegion>();

        foreach(MapRegion region in regions) {
            if(region.canSpawnMonster) {
                possibleRegion.Add(region);
            }
        }

        spawnPosition = possibleRegion[Random.Range(0, possibleRegion.Count)].GetRandomPoint();

        return spawnPosition + new Vector2(0.5f, 0.5f);
    }

    public Vector2 GetPlayerSpawnPosition() {
        Vector2 spawnPosition = new Vector2();

        foreach(MapRegion region in regions) {
            if(region.canSpawnPlayer) {
                spawnPosition = region.GetRandomPoint();
            }
        }

        return spawnPosition + new Vector2(0.5f, 0.5f);
    }
}

//Represents a tile of a map, does not take in count the layer of tilemap
public class MapTile {
    public Vector2Int position;

    public float cost = 1;

    public Tile tile;

    int score;

    public bool isSolid;
    public bool isOccuped = false;

    float lifePoint;

    public MapTile(Vector2Int pos, bool solid) {
        position = pos;
        isSolid = solid;

        if(isSolid){
            lifePoint = 1000;
        }
    }

    public MapTile(MapTile t) {
        position = t.position;
        isSolid = t.isSolid;
        cost = t.cost;
        tile = t.tile;
        score = t.score;

        if(isSolid) {
            lifePoint = 1000;
        }
    }

    public void AddItem() {
        isOccuped = true;
        cost = Mathf.Infinity;
    }

    public bool Attack(float d) {
        if(!isSolid) {
            return false;
        }

        lifePoint -= d;
        if(lifePoint <= 0) {
            isSolid = false;
            tile = null;

            return true;
        } else{
            return false;
        }
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

    public MapRegion(MapRegion r) {
        tiles = r.tiles;

        centerOfRegion = tiles[Random.Range(0, tiles.Count)].position;

        SetType(r.type);
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
        MapTile randomTile = null;

        while(randomTile == null) {
            randomTile = tiles[Random.Range(0, tiles.Count)];

            if(!HasNeighborDown(randomTile)) randomTile = null;
        }

        return randomTile.position;
    }

    public void Fusion(MapRegion region) {
        foreach(MapTile t in region.tiles) {
            tiles.Add(new MapTile(t));
        }
    }

    #endregion

    private bool HasNeighborDown(MapTile tile) {
        foreach(MapTile t in tiles) {
            if(t.position.x == tile.position.x && t.position.y == tile.position.y - 1) {
                return true;
            }
        }

        return false;
    }
}
