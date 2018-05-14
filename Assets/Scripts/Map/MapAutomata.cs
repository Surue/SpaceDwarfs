using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using System.Linq;

public class MapRegion {
    public MapRegion(List<Vector2Int> tiles) {
        this.tiles = tiles;

        float count = 0;

        foreach(Vector2Int tile in tiles) {
            count++;
            centerOfRegion += (Vector2)tile;
        }

        centerOfRegion /= count;
    }

    public MapRegion(List<Vector2Int> tiles, bool spawnMonster, bool spawnPlayer, bool spawnNest) {
        this.tiles = tiles;
        canSpawnMonster = spawnMonster;
        canSpawnPlayer = spawnPlayer;
        canSpawnNest = spawnNest;

        float count = 0;

        foreach(Vector2Int tile in tiles) {
            count++;
            centerOfRegion += (Vector2)tile;
        }

        centerOfRegion /= count;
    }

    public List<Vector2Int> tiles;
    public Vector2 centerOfRegion = new Vector2(0, 0);

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
        //LinkAllRgionBigEnough();

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

        MapRegion spawnRegion = GenerateMapRegionForSpawn(spawnPosition);

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

    void LinkAllRgionBigEnough() {
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

        Debug.Log(mapRegion.Count);

        //Keep smalest region

        for(int i = 0;i < mapRegion.Count;i++) {
            if(mapRegion[i].tiles.Count < rule_minimumRegionSize) {
                ClosedRegion.Add(mapRegion[i]);
                mapRegion.Remove(mapRegion[i]);
                i--;
            }
        }

        //Dig from all area to each other
        DigFromAllRegionFromBiggestOne(mapRegion);
    }

    void DigFromAllRegionFromBiggestOne(List<MapRegion> regionsToLink) {

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

        int rule_CostFullTile = 1000;
        int rule_CostEmptyTile = 1;

        //Dig from center to all region 
        foreach(MapRegion region in regionsToLink) {
            float[,] costGraph = new float[width, height];
            bool[,] visitedGraph = new bool[width, height];
            Vector2Int[,] parentGraph = new Vector2Int[width, height];

            Vector2 endTile = region.centerOfRegion;

            for(int x = 0;x < width;x++) {
                for(int y = 0;y < height;y++) {
                    if(terrainMap[x, y] == 1) {
                        costGraph[x, y] = rule_CostFullTile + Vector2.Distance(new Vector2(x, y), endTile);
                    } else {
                        costGraph[x, y] = rule_CostEmptyTile + Vector2.Distance(new Vector2(x, y), endTile);
                    }

                    visitedGraph[x, y] = false;
                }
            }
            Vector2Int startTile = new Vector2Int(Mathf.RoundToInt(biggestRegion.centerOfRegion.x), Mathf.RoundToInt(biggestRegion.centerOfRegion.y));
            
            costGraph[startTile.x, startTile.y] = 0;

            endTile = new Vector2Int(Mathf.RoundToInt(endTile.x), Mathf.RoundToInt(endTile.y));

            List<Vector2Int> openGraph = new List<Vector2Int>();
            openGraph.Add(startTile);

            do {
                Vector2Int node = new Vector2Int(-1, -1);
                float minCost = Mathf.Infinity;
                foreach(Vector2Int tile in openGraph) {
                    if(minCost > costGraph[tile.x, tile.y]) {
                        minCost = costGraph[tile.x, tile.y];
                        node = tile;
                    }
                }
                
                openGraph.Remove(node);

                BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

                foreach(Vector3Int b in bounds.allPositionsWithin) {
                    if(b.x == 0 && b.y == 0) continue;
                    if(node.x + b.x >= 0 && node.x + b.x < width && node.y + b.y >= 0 && node.y + b.y < height) { //Is in the map
                        Debug.Log("x = " + node.x + ", y = " + node.y);
                        float newCost = costGraph[node.x, node.y] + Vector2.Distance(node, new Vector2(node.x + b.x, node.y + b.y));
                        if(visitedGraph[node.x, node.y]) continue;
                        if(costGraph[node.x + b.x, node.y + b.y] == 0 || newCost < costGraph[node.x + b.x, node.y + b.y]) {
                            parentGraph[node.x + b.x, node.y + b.y] = new Vector2Int(node.x, node.y);
                            if(!openGraph.Contains(new Vector2Int(node.x + b.x, node.y + b.y))) {
                                openGraph.Add(new Vector2Int(node.x + b.x, node.y + b.y));
                            }
                        }
                    }
                }

                if(node == endTile) openGraph.Clear();
                visitedGraph[node.x, node.y] = true;

            } while(openGraph.Count != 0);

            Vector2Int tileToClear = new Vector2Int(Mathf.RoundToInt(endTile.x), Mathf.RoundToInt(endTile.y));

            int count = 10;

            while(tileToClear != startTile && count > 0) {
                if(terrainMap[tileToClear.x, tileToClear.y] == 1) {
                    terrainMap[tileToClear.x, tileToClear.y] = 0;
                }
                Debug.Log(tileToClear);
                tileToClear = parentGraph[tileToClear.x, tileToClear.y];
                count--;
            }

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
