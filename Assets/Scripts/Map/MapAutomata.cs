using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using System.Linq;

public class MapRegion {
    public MapRegion(List<Vector2Int> tiles) {
        this.tiles = tiles;

        centerOfRegion = tiles[Random.Range(0, tiles.Count)];
        
    }

    public MapRegion(List<Vector2Int> tiles, bool spawnMonster, bool spawnPlayer, bool spawnNest) {
        this.tiles = tiles;
        canSpawnMonster = spawnMonster;
        canSpawnPlayer = spawnPlayer;
        canSpawnNest = spawnNest;


        centerOfRegion = tiles[Random.Range(0, tiles.Count)];
    }

    public List<Vector2Int> tiles;
    public Vector2Int centerOfRegion = new Vector2Int(0, 0);

    //Rules
    bool canSpawnMonster;
    bool canSpawnPlayer;
    bool canSpawnNest;
}

public class MapAutomata:MonoBehaviour {

    [Range(0, 100)]
    public int initialChance;

    [Range(1, 8)]
    public int birthLimit;

    [Range(1, 8)]
    public int deathLimit;

    [Range(1, 10)]
    public int numberOfIteration;

    private int[,] terrainMap;

    public Vector2Int tilemapSize;

    public Tile topTile;
    public Tile botTile;
    public List<Tile> debugTiles;

    int width;
    int height;

    public Tilemap debugTilemap;

    public List<MapRegion> ClosedRegion = new List<MapRegion>();

    public void GenerateMap(Tilemap solidTilemap, Tilemap groundTilemap) {
        width = tilemapSize.x;
        height = tilemapSize.y;

        //Initial repartition of tile
        if(terrainMap == null) {
            terrainMap = new int[width, height];
            InitPosition();
        }

        //Use automata algo to create cave
        for(int i = 0;i < numberOfIteration;i++) {
            terrainMap = GenTilePos(terrainMap);
        }

        //Assure a passe between all big region
        //Associate visual tile
        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                if(terrainMap[x, y] == 1) {
                    solidTilemap.SetTile(new Vector3Int(x, y, 0), topTile);
                } else {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), botTile);
                }
            }
        }

        LinkAllRgionBigEnough(solidTilemap);

        //Create spawn point region
        //Select spawnPoint
        int rule_SpawnWidthFromEdge = 5;
        int rule_SpawnHeightFromEdge = 5;

        List<Vector2Int> possibleSpawnPoint = new List<Vector2Int>();

        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                if((x >= 0 && x < -1 + rule_SpawnWidthFromEdge) || (x > width - rule_SpawnWidthFromEdge && x < width)) {
                    if(terrainMap[x, y] == 0) {
                        possibleSpawnPoint.Add(new Vector2Int(x, y));
                    }
                } else if((y >= 0 && y < -1 + rule_SpawnHeightFromEdge) || (y > height - rule_SpawnHeightFromEdge && x < height)) {
                    if(terrainMap[x, y] == 0) {
                        possibleSpawnPoint.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        Vector2Int spawnPosition = possibleSpawnPoint[Random.Range(0, possibleSpawnPoint.Count)];

        //MapRegion spawnRegion = GenerateMapRegionForSpawn(spawnPosition);

        //if(debugTilemap != null) {
        //    foreach(Vector2Int tile in spawnRegion.tiles) {
        //        debugTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), debugTiles[0]);
        //    }
        //}

        //Create all other region

        //Fill region with treasure

        //Fill solide tile with treasure

        //Associate visual tile
        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                if(terrainMap[x, y] == 1) {
                    solidTilemap.SetTile(new Vector3Int(x, y, 0), topTile);
                } else {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), botTile);
                }
            }
        }
    }

    void LinkAllRgionBigEnough(Tilemap solidTilemap) {
        int rule_minimumRegionSize = 20;

        List<MapRegion> mapRegion = new List<MapRegion>();

        List<Vector2Int> freeTiles = new List<Vector2Int>();

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                if(terrainMap[x, y] == 0) {
                    freeTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        //On selectionne toute les zones existante
        while(freeTiles.Count > 0) {

            List<Vector2Int> tilesForRegion = new List<Vector2Int>();

            List<Vector2Int> tilesToCheck = new List<Vector2Int>();
            tilesToCheck.Add(freeTiles.First());
            freeTiles.Remove(freeTiles.First());

            while(tilesToCheck.Count > 0) {
                Vector2Int currentTile = tilesToCheck.First();
                tilesToCheck.Remove(currentTile);

                foreach(Vector3Int b in bounds.allPositionsWithin) {
                    if(b.x == 0 && b.y == 0) continue;
                    if(currentTile.x + b.x >= 0 && currentTile.x + b.x < width && currentTile.y + b.y >= 0 && currentTile.y + b.y < height) { //Is in the map
                        if(freeTiles.Contains(new Vector2Int(currentTile.x + b.x, currentTile.y + b.y))) { //Is a free tile
                            if(freeTiles.Contains(new Vector2Int(currentTile.x, currentTile.y + b.y)) || freeTiles.Contains(new Vector2Int(currentTile.x + b.x, currentTile.y))) {
                                freeTiles.Remove(new Vector2Int(currentTile.x + b.x, currentTile.y + b.y));
                                tilesToCheck.Add(new Vector2Int(currentTile.x + b.x, currentTile.y + b.y));
                            }
                        }
                    }
                }

                tilesForRegion.Add(currentTile);
            }

            mapRegion.Add(new MapRegion(tilesForRegion));
        }

        //Keep smalest region

        for(int i = 0;i < mapRegion.Count;i++) {
            if(mapRegion[i].tiles.Count < rule_minimumRegionSize) {
                ClosedRegion.Add(mapRegion[i]);
                mapRegion.Remove(mapRegion[i]);
                i--;
            }
        }

        //Dig from all area to each other
        DigFromAllRegionFromBiggestOne(mapRegion, solidTilemap);
    }

    void DigFromAllRegionFromBiggestOne(List<MapRegion> regionsToLink, Tilemap solidTilemap) {

        //Get the biggest region
        MapRegion biggestRegion = null;

        int maxTile = 0;

        foreach(MapRegion region in regionsToLink) {
            if(region.tiles.Count > maxTile) {
                maxTile = region.tiles.Count;
                biggestRegion = region;
            }
        }
       
        //Order all other by closest from centerOfRegion
        regionsToLink.Remove(biggestRegion);
        regionsToLink.OrderBy(x => Vector2.Distance(x.centerOfRegion, biggestRegion.centerOfRegion));

        NavigationAI navigationGraph = FindObjectOfType<NavigationAI>();

        PathFinding aStart = FindObjectOfType<PathFinding>();

        int count = 0;

        //Dig from center to all region 
        while(regionsToLink.Count > 0) {
            List<List<Vector2Int>> paths = new List<List<Vector2Int>>();

            foreach(MapRegion region in regionsToLink) {
                navigationGraph.GenerateNavigationGraph();
                List<Vector2Int> path = new List<Vector2Int>();

                path = aStart.GetPathFromTo(biggestRegion.centerOfRegion, region.centerOfRegion, true);

                for(int i = 0;i < path.Count;i++) {
                    if(terrainMap[path[i].x, path[i].y] == 0) {
                        path.Remove(new Vector2Int(path[i].x, path[i].y));
                        i--;
                    }
                }

                paths.Add(path);

            }

            float minPath = Mathf.Infinity;
            int index = 0;
            
            for(int i = 0; i < regionsToLink.Count;i++) {
                Debug.Log("count = " + paths[i].Count);
                if(paths[i].Count < minPath) {
                    minPath = paths[i].Count;
                    index = i;
                }
            }

            List<Vector2Int> minimumPath = paths[index];
            regionsToLink.RemoveAt(index);
            Debug.Log("Selected path length  = " + minimumPath.Count);   
            foreach(Vector2Int node in minimumPath) {
                terrainMap[node.x, node.y] = 0;
                debugTilemap.SetTile(new Vector3Int(node.x, node.y, 0), debugTiles[count+1]);
            }

            solidTilemap.ClearAllTiles();

            for(int x = 0;x < width;x++) {
                for(int y = 0;y < height;y++) {
                    if(terrainMap[x, y] == 1) {
                        solidTilemap.SetTile(new Vector3Int(x, y, 0), topTile);
                    }
                }
            }
            count++;
            navigationGraph.solidTilemap = solidTilemap;
        }

        if(debugTilemap != null) {
            foreach(Vector2Int tile in biggestRegion.tiles) {
                debugTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), debugTiles[0]);
            }
        }
    }

    MapRegion GenerateMapRegionForSpawn(Vector2Int spawnTile) {
        List<Vector2Int> tiles = new List<Vector2Int>();

        List<Vector2Int> tilesToCheck = new List<Vector2Int>();

        tilesToCheck.Add(spawnTile);
        
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        while(tilesToCheck.Count > 0 && tiles.Count < 30) {
            tilesToCheck.OrderBy(x => Vector2.Distance(x, spawnTile));
            
            Vector2Int tile = tilesToCheck.First();

            tilesToCheck.Remove(tile);

            if(terrainMap[tile.x, tile.y] == 0) {
                tiles.Add(tile);

                foreach(Vector3Int b in bounds.allPositionsWithin) {
                    if(b.x == 0 && b.y == 0) continue;
                    if(tile.x + b.x >= 0 && spawnTile.x + b.x < width && spawnTile.y + b.y >= 0 && spawnTile.y + b.y < height) {
                        if(!tilesToCheck.Contains(new Vector2Int(tile.x + b.x, tile.y + b.y)) && !tiles.Contains(new Vector2Int(tile.x + b.x, tile.y + b.y)))
                        tilesToCheck.Add(new Vector2Int(tile.x + b.x, tile.y + b.y));
                    } 
                }

            }
        }

        return new MapRegion(tiles, false, true, false);
    }

    public int [,] GenTilePos(int[,] previousMap) {
        int[,] newMap = new int[width, height];

        int nbOfNeighbor;
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height;y++) {
                nbOfNeighbor = 0;

                foreach(Vector3Int b in bounds.allPositionsWithin) {
                    if(b.x == 0 && b.y == 0) continue;
                    if(x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height) {
                        nbOfNeighbor += previousMap[x + b.x, y + b.y];
                    } else {
                        nbOfNeighbor++;
                    }
                }

                if(previousMap[x, y] == 1) {
                    if(nbOfNeighbor < deathLimit) {
                        newMap[x, y] = 0;
                    } else {
                        newMap[x, y] = 1;
                    }
                }

                if(previousMap[x, y] == 0) {
                    if(nbOfNeighbor > birthLimit) {
                        newMap[x, y] = 1;
                    } else {
                        newMap[x, y] = 0;
                    }
                }
            }
        }

        return newMap;
    }

    public void InitPosition() {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                terrainMap[x, y] = Random.Range(1, 101) < initialChance ? 1 : 0;
            }
        }
    }
}
