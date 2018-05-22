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
    

    [SerializeField]
    Tilemap solidTilemap;
    [SerializeField]
    Tilemap groundTilemap;
    [SerializeField]
    Tilemap decalTilemap;

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
                tiles[x, y] = mapTiles[x, y];
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
            if(GetComponent<Score>()) {
                Vector3 location = new Vector3(t.position.x, t.position.y, 0);

                FindObjectOfType<ScoreManager>().DisplayScore(t.score, location + new Vector3(0.5f, 0.5f, 0));
            }
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
                Vector3Int pos = new Vector3Int(tile.position.x, tile.position.y, 0);
                solidTilemap.SetTile(pos, tile.tile);
                groundTilemap.SetTile(pos, tile.groundTile);
                decalTilemap.SetTile(pos, tile.decalTile);
            }
        }
    }

    public void DrawAll() {
        foreach(MapTile tile in tiles) {
            Vector3Int pos = new Vector3Int(tile.position.x, tile.position.y, 0);
            solidTilemap.SetTile(pos, tile.tile);
            groundTilemap.SetTile(pos, tile.groundTile);
            decalTilemap.SetTile(pos, tile.decalTile);
        }

        solidTilemap.GetComponent<CompositeCollider2D>().GenerateGeometry();
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
                foreach(MapTile t in region.tiles) {
                    if(t.position == spawnPosition) {
                        t.AddItem();
                    }
                }
            }
        }

        return spawnPosition + new Vector2(0.5f, 0.5f);
    }

    public Vector2 GetNestPosition() {
        foreach(MapRegion region in regions) {
            if(region.type == MapRegion.TypeRegion.NEST) {
                return region.GetNestPosition() + new Vector2(0.5f, 0.5f);
            }
        }

        return new Vector2();
    }

    public Vector2 GetPatrolPoint() {
        while(true) {
            MapTile t = tiles[Random.Range(0, tiles.GetLength(0)), Random.Range(0, tiles.GetLength(1))];

            if(!t.isSolid && !t.isOccuped && !t.isInvulnerable) {
                foreach(MapRegion region in regions) {
                    if(region.tiles.Contains(t)) {
                        if(region.type != MapRegion.TypeRegion.CLOSED) {
                            return t.position;
                        }
                    }
                }
            }
        }
    }

    public List<Vector2> GetPatrolsPointForRegion(Transform pos) {
        Vector2Int tilePos = Transform2TilePos(pos);
        foreach(MapRegion region in regions) {
           foreach(MapTile tile in region.tiles) {
                if(tile.position == tilePos) {
                    return region.GetPatrolsPoint();
                }
            }
        }
        return null;
    }

    Vector2Int Transform2TilePos(Transform pos) {
        int x = 0;
        int y = 0;

        x = Mathf.FloorToInt(pos.position.x);
        y = Mathf.FloorToInt(pos.position.y);

        return new Vector2Int(x, y);
    }

    public static Vector2Int Vector2TilePos(Vector2 pos) {
        int x = 0;
        int y = 0;

        x = Mathf.FloorToInt(pos.x);
        y = Mathf.FloorToInt(pos.y);

        return new Vector2Int(x, y);
    }
}

//Represents a tile of a map, does not take in count the layer of tilemap
public class MapTile {

    [System.Serializable]
    public enum TileType {
        FREE,
        SOLID,
        INVULNERABLE,
        ANY //Used to draw them
    }

    public TileType type;

    public Vector2Int position;

    public float cost = 1;

    public Tile tile;
    public Tile groundTile;
    public Tile decalTile;

    public int score = 5;

    public bool isSolid;
    public bool isInvulnerable;
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
        type = t.type;

        if(isSolid) {
            lifePoint = 1000;
        }
    }

    public void SetType(TileType t) {
        type = t;

        switch(type) {
            case TileType.FREE:
                isInvulnerable = false;
                isSolid = false;
                break;

            case TileType.SOLID:
                isInvulnerable = false;
                isSolid = true;
                break;

            case TileType.INVULNERABLE:
                isInvulnerable = true;
                isSolid = true;
                cost = Mathf.Infinity;
                break;
        }
    }

    public void AddOre(SO_Ore ore) {
        decalTile = ore.oreTile;
        score = ore.oreScore;

        isOccuped = true;
    }

    public void AddItem() {
        isOccuped = true;
        cost = Mathf.Infinity;
    }

    public bool Attack(float d) {
        if(!isSolid || isInvulnerable) {
            return false;
        }

        lifePoint -= d;
        if(lifePoint <= 0) {
            SetType(TileType.FREE);

            decalTile = null;

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

            if(!HasNeighborDown(randomTile) || randomTile.isOccuped) randomTile = null;
        }

        return randomTile.position;
    }

    public void Fusion(MapRegion region) {
        foreach(MapTile t in region.tiles) {
            tiles.Add(new MapTile(t));
        }
    }

    public Vector2Int GetNestPosition() {
        foreach(MapTile t in tiles) {
            if(t.isOccuped) {
                return t.position;
            }
        }

        return new Vector2Int();
    }

    public List<Vector2> GetPatrolsPoint() {
        List<Vector2> patrolsPoint = new List<Vector2>();

        List<MapTile> possibleTiles = tiles;
        while(patrolsPoint.Count < 3) {
            MapTile t = possibleTiles[Random.Range(0, possibleTiles.Count)];
            if(!t.isOccuped) {
                patrolsPoint.Add(t.position + new Vector2(0.5f, 0.5f));
                possibleTiles.Remove(t);
            }
        }

        return patrolsPoint;
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
