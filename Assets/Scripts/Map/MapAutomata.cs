using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class MapAutomata : MonoBehaviour {

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

    int width;
    int height;

    public void GenerateMap(Tilemap solidTilemap, Tilemap groundTilemap) {
        width = tilemapSize.x;
        height = tilemapSize.y;

        if(terrainMap == null) {
            terrainMap = new int[width, height];
            InitPosition();
        }

        for(int i = 0; i < numberOfIteration;i++) {
            terrainMap = GenTilePos(terrainMap);
        }

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height;  y++) {
                if(terrainMap[x, y] == 1) {
                    solidTilemap.SetTile(new Vector3Int(x, y, 0), topTile);
                } else {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), botTile);
                }
            }
        }
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
