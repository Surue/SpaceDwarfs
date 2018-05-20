using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Linq;

public class MapAutomata : MonoBehaviour {

    [Header("Map generation")]

    [Range(0, 100)]
    public int rule_initialChance;

    [Range(1, 8)]
    public int rule_birthLimit;

    [Range(1, 8)]
    public int rule_deathLimit;

    [Range(1, 10)]
    public int rule_numberOfIteration;

    [Range(1, 50)]
    public int rule_maximumTileForClosedRegion;

    [Range(1, 500)]
    public int rule_minimumTileForRegion;

    [Range(1, 50)]
    public int rule_maximumTileToBefusionned;

    [Range(1, 20)]
    public int rule_numberTileForClosing;

    [Header("Ore generation")]
    [Range(1,10)]
    public int rule_maxOrePerVein;

    [Range(1, 20)]
    public int rule_chunkSize;

    [Range(1,10)]
    public int rule_veinPerChunk;

    public List<SO_Ore> ores;

    [Header("Rule for tiles")]
    public List<SO_RuleTile> rulesForTiles;

    [Header("Spawnable item & rules")]
    public int maxItemPerRegion = 3;
    public List<SO_Item> items;

    public Vector2Int tilemapSize;

    public List<Tile> debugTiles;

    public Tilemap debugTilemap;

    public bool isGenerating = false;

    public IEnumerator GenerateMap(MapTile[,] mapTiles) {
        isGenerating = true;

        int width = tilemapSize.x;
        int height = tilemapSize.y;

        mapTiles = new MapTile[width, height];

        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                bool isSolid = Random.Range(1, 101) < rule_initialChance ? true : false;

                mapTiles[x, y] = new MapTile(new Vector2Int(x, y), isSolid);
            }
        }

        for(int i = 0; i < rule_numberOfIteration;i++) {
            mapTiles = GenTilePos(mapTiles);
            yield return null;
        }

        isGenerating = false;

        GetComponent<MapController>().SetTiles(mapTiles);
    }

    MapTile[,] GenTilePos(MapTile[,] previousMap) {
        int width = previousMap.GetLength(0);
        int height = previousMap.GetLength(1);
        MapTile[,] newMap = new MapTile[width, height];

        int nbOfNeighbor;
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                nbOfNeighbor = 0;

                foreach(Vector3Int b in bounds.allPositionsWithin) {
                    if(b.x == 0 && b.y == 0) continue;
                    if(x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height) {
                        if(previousMap[x + b.x, y + b.y].isSolid) {
                            nbOfNeighbor += 1;
                        }
                    } else {
                        nbOfNeighbor++;
                    }
                }

                if(previousMap[x, y].isSolid) {
                    bool isSolid;
                    if(nbOfNeighbor < rule_deathLimit) {
                        isSolid = false;
                    } else {
                        isSolid = true;
                    }
                    newMap[x, y] = new MapTile(new Vector2Int(x, y), isSolid);
                }

                if(!previousMap[x, y].isSolid) {
                    bool isSolid;
                    if(nbOfNeighbor > rule_birthLimit) {
                        isSolid = true;
                    } else {
                        isSolid = false;
                    }
                    newMap[x, y] = new MapTile(new Vector2Int(x, y), isSolid);
                }
            }
        }

        return newMap;
    }

    public IEnumerator DigBetweenRegions(MapTile[,] mapTiles) {
        isGenerating = true;
        
        //Get all region defined by solid tile
        List<MapRegion> mapRegion = GetRegionBySolidTile(mapTiles);

        #region keep smalest region and get the bigger one

        //Keep smalest region

        for(int i = 0;i < mapRegion.Count;i++) {
            if(mapRegion[i].tiles.Count < rule_maximumTileForClosedRegion) {
                mapRegion.Remove(mapRegion[i]);
                i--;
            }
        }

        //Dig from all area to each other
        //Get the biggest region
        MapRegion biggestRegion = null;

        int maxTile = 0;

        foreach(MapRegion region in mapRegion) {
            if(region.tiles.Count > maxTile) {
                maxTile = region.tiles.Count;
                biggestRegion = region;
            }
        }

        #endregion

        #region Get all path from biggest region and dig from them

        //Order all other by closest from centerOfRegion
        mapRegion.Remove(biggestRegion);
        mapRegion = mapRegion.OrderBy(x => Vector2.Distance(x.centerOfRegion, biggestRegion.centerOfRegion)).ToList();

        NavigationAI navigationGraph = FindObjectOfType<NavigationAI>();

        PathFinding aStart = FindObjectOfType<PathFinding>();

        //Dig from center to all region 
        while(mapRegion.Count > 0) {
            List<List<Vector2Int>> paths = new List<List<Vector2Int>>();

            foreach(MapRegion region in mapRegion) {
                navigationGraph.GenerateNavigationGraph(mapTiles);
                List<Vector2Int> path = new List<Vector2Int>();

                path = aStart.GetPathFromTo(biggestRegion.centerOfRegion, region.centerOfRegion, true);

                for(int i = 0;i < path.Count;i++) {
                    if(!mapTiles[path[i].x, path[i].y].isSolid) {
                        path.Remove(new Vector2Int(path[i].x, path[i].y));
                        i--;
                    }
                }

                paths.Add(path);
                yield return null;
            }

            float minPath = Mathf.Infinity;
            int index = 0;

            for(int i = 0;i < mapRegion.Count;i++) {
                if(paths[i].Count < minPath) {
                    minPath = paths[i].Count;
                    index = i;
                }
            }

            List<Vector2Int> minimumPath = paths[index];
            mapRegion.RemoveAt(index);

            foreach(Vector2Int node in minimumPath) {
                debugTilemap.SetTile(new Vector3Int(node.x, node.y, 0), debugTiles[1]);
                mapTiles[node.x, node.y].isSolid = false;
            }

            yield return null;
        }

        #endregion
        isGenerating = false;

        GetComponent<MapController>().SetTiles(mapTiles);
    }

    public IEnumerator GetRegions(MapTile[,] mapTiles) {
        isGenerating = true;

        int width = mapTiles.GetLength(0);
        int height = mapTiles.GetLength(1);

        List<MapRegion> regions = new List<MapRegion>();

        //Get all region defined by solid tile
        List<MapRegion> mainRegion = GetRegionBySolidTile(mapTiles);
        yield return null;

        #region Keep closed region

        //Keep closed one
        for(int i = 0; i < mainRegion.Count; i++) {
            MapRegion region = mainRegion[i];

            if(region.tiles.Count < rule_maximumTileForClosedRegion) {
                mainRegion.Remove(region);
                if(region.type != MapRegion.TypeRegion.CLOSED) {
                    region.SetType(MapRegion.TypeRegion.CLOSED);
                }

                regions.Add(region);

                i--;
            }
        }

        #endregion

        #region Find sub region 

        //In the remeaning regions for create sub regions of a maximum size
        while(mainRegion[0].tiles.Count > 0) {
            BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
            List<MapTile> tilesForNewRegion = new List<MapTile>();

            List<MapTile> tilesToCheck = new List<MapTile>();

            MapTile t = mainRegion[0].tiles.First();

            tilesToCheck.Add(t);

            mainRegion[0].tiles.Remove(t);

            while(tilesForNewRegion.Count < rule_minimumTileForRegion && tilesToCheck.Count > 0) {

                tilesToCheck = tilesToCheck.OrderBy(x => Vector2Int.Distance(x.position, t.position)).ToList();

                MapTile currentTile = tilesToCheck[0];

                foreach(Vector3Int b in bounds.allPositionsWithin) {
                    if(b.x == 0 && b.y == 0) continue;
                    if(b.x != 0 && b.y != 0) continue;
                    
                    if(currentTile.position.x + b.x >= 0 && currentTile.position.x + b.x < width && currentTile.position.y + b.y >= 0 && currentTile.position.y + b.y < height) { //Is in the map
                    MapTile newTile = mapTiles[currentTile.position.x + b.x, currentTile.position.y + b.y];
                        if(mainRegion[0].tiles.Contains(newTile)) {
                            if(!tilesToCheck.Contains(newTile)) {
                                if(!tilesForNewRegion.Contains(newTile)) {
                                    tilesToCheck.Add(mapTiles[currentTile.position.x + b.x, currentTile.position.y + b.y]);
                                }
                            }
                        }
                    }
                }

                tilesToCheck.Remove(currentTile);
                mainRegion[0].tiles.Remove(currentTile);
                tilesForNewRegion.Add(currentTile);
            }

            yield return null;

            regions.Add(new MapRegion(tilesForNewRegion, MapRegion.TypeRegion.NORMAL));
        }

        #endregion

        foreach(MapRegion region in regions) {
            GetComponent<MapController>().AddRegion(region);
        }

        isGenerating = false;
    }

    public IEnumerator AssociateRegions(MapTile[,] mapTile, List<MapRegion> previousRegions) {
        isGenerating = true;

        int width = mapTile.GetLength(0);
        int height = mapTile.GetLength(1);

        List<MapRegion> regionTooSmall = new List<MapRegion>();
        List<MapRegion> regions = new List<MapRegion>();

        foreach(MapRegion region in previousRegions) {
            regions.Add(new MapRegion(region));
        }

        #region Get region that need to be fussioned

        //On récupère les régions plus petite que le nombre minimum et qui ne sont pas fermée
        for(int i = 0;i < regions.Count;i++) {
            MapRegion region = regions[i];

            if(region.tiles.Count < rule_maximumTileToBefusionned && region.type != MapRegion.TypeRegion.CLOSED) {
                regionTooSmall.Add(region);
                regions.Remove(region);
                i--;
            }
        }

        #endregion

        #region Fussion all region with bigger one

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        //On parcourt la lsite des régions trop petite pour les fussionner aux région voisines
        foreach(MapRegion region in regionTooSmall) {
            List<MapTile> tilesToCheck = new List<MapTile>();

            foreach(MapTile t in region.tiles) {
                tilesToCheck.Add(t);
            }

            for(int i = 0;i < tilesToCheck.Count;i++) {
                MapTile currentTile = tilesToCheck[i];

                foreach(Vector3Int b in bounds.allPositionsWithin) {
                    if(b.x == 0 && b.y == 0) continue;
                    if(currentTile.position.x + b.x >= 0 && currentTile.position.x + b.x < width && currentTile.position.y + b.y >= 0 && currentTile.position.y + b.y < height) { //Is in the map
                        MapTile testTile = mapTile[currentTile.position.x + b.x, currentTile.position.y + b.y];

                        foreach(MapRegion r in regions) {
                            if(r.tiles.Contains(testTile)) {
                                r.Fusion(region);
                                i = tilesToCheck.Count;
                                break;
                            }
                        }
                    }
                }
            }

            yield return null;
        }

        #endregion

        #region Associate regions to type of regions

        int corner = Random.Range(0, 3);
        //for the spawn area we use a region in a corner
        Vector2Int closestPoint = new Vector2Int(0,0);
        switch(corner) {
            case 0: //top left
                closestPoint = new Vector2Int(0, mapTile.GetLength(1) - 1);
                break;

            case 1: //top right
                closestPoint = new Vector2Int(mapTile.GetLength(0) - 1, mapTile.GetLength(1) - 1);
                break;

            case 2: // bottom right
                closestPoint = new Vector2Int(mapTile.GetLength(0) - 1, 0);
                break;

            case 3: // bottom left
                break;
        }
        

        bool regionFound = false;

        List<MapTile> possibleTile = new List<MapTile>();

        for(int x = 0;x < mapTile.GetLength(0);x++) {
            for(int y = 0;y < mapTile.GetLength(1);y++) {
                possibleTile.Add(mapTile[x, y]);
            }
        }

        possibleTile = possibleTile.OrderBy(x => Vector2Int.Distance(x.position, closestPoint)).ToList();
        
        while(!regionFound) {
            MapTile currentTile = possibleTile.First();
            possibleTile.Remove(currentTile);

            if(!currentTile.isSolid) {
                foreach(MapRegion region in regions) {
                    if(region.tiles.Contains(currentTile) && region.type != MapRegion.TypeRegion.CLOSED) {
                        region.SetType(MapRegion.TypeRegion.PLAYER_SPAWN);
                        regionFound = true;
                        break;
                    }
                }
            }
        }

        //for the nest region we use some regions in the opposite corner
        switch(corner) {
            case 0: //top left for player => bottom right
                closestPoint = new Vector2Int(mapTile.GetLength(0) - 1, 0);
                break;

            case 1: //top right for player => bottom left
                closestPoint = new Vector2Int(0, 0);
                break;

            case 2: // bottom right for player => top left
                closestPoint = new Vector2Int(0, mapTile.GetLength(1) - 1);
                break;

            case 3: // bottom left for player => top right
                closestPoint = new Vector2Int(mapTile.GetLength(0) - 1, mapTile.GetLength(1) - 1);
                break;
        }

        int regionToFoundForNest = 1; // TO CHANGE

        possibleTile = new List<MapTile>();

        for(int x = 0; x < mapTile.GetLength(0);x++) {
            for(int y = 0; y < mapTile.GetLength(1);y++) {
                possibleTile.Add(mapTile[x, y]);
            }
        }

        possibleTile = possibleTile.OrderBy(x => Vector2Int.Distance(x.position, closestPoint)).ToList();
        Debug.Log("possibleFirst.Count = " + possibleTile.Count);
        while(regionToFoundForNest > 0) {
            Debug.Log("nb regions" + regions.Count);
            MapTile currentTile = possibleTile[0];
            possibleTile.Remove(currentTile);

            if(!currentTile.isSolid) { 
                foreach(MapRegion region in regions) {
                    if(region.tiles.Contains(currentTile) && region.type == MapRegion.TypeRegion.NORMAL) {
                        region.SetType(MapRegion.TypeRegion.NEST);
                        regionToFoundForNest--;
                        break;
                    }
                }
            }
        }

        #endregion

        GetComponent<MapController>().ClearRegions();
        
        foreach(MapRegion region in regions) {
            GetComponent<MapController>().AddRegion(region);
        }

        isGenerating = false;
    }

    public IEnumerator CloseMap(MapTile[,] previousMapTile) {
        foreach(MapTile tile in previousMapTile) {
            if(tile.isSolid) {
                tile.SetType(MapTile.TileType.SOLID);
            } else {
                tile.SetType(MapTile.TileType.FREE);
            }
        }

        MapTile[,] newMap = new MapTile[previousMapTile.GetLength(0) + (2 * rule_numberTileForClosing), previousMapTile.GetLength(1) + (2 * rule_numberTileForClosing)];

        for(int x = 0; x < newMap.GetLength(0);x++) {
            for(int y = 0;y < newMap.GetLength(1);y++) {
                if(x >= 0 + rule_numberTileForClosing && x < newMap.GetLength(0) - rule_numberTileForClosing && y >= 0 + rule_numberTileForClosing && y < newMap.GetLength(1) - rule_numberTileForClosing) { //in the old map
                    MapTile t = previousMapTile[x - rule_numberTileForClosing, y - rule_numberTileForClosing];
                    t.position = new Vector2Int(x, y);
                    if(t.isSolid) {
                        t.SetType(MapTile.TileType.SOLID);
                    } else {
                        t.SetType(MapTile.TileType.FREE);
                    }

                    newMap[x, y] = t;
                } else {
                    MapTile t = new MapTile(new Vector2Int(x, y), true);
                    t.SetType(MapTile.TileType.INVULNERABLE);
                    newMap[x, y] = t;
                }
            }
        }

        GetComponent<MapController>().SetTiles(newMap);

        yield return null;
    }

    public IEnumerator AssociateTiles(MapTile[,] mapTile) {
        isGenerating = true;
        foreach(MapTile tile in mapTile) {
            foreach(SO_RuleTile rule in rulesForTiles) {
                if(rule.IsThisTile(tile, mapTile)) {
                    tile.tile = rule.tile;
                    tile.groundTile = rule.groundTile;
                    break;
                }
            }
        }

        yield return null;

        isGenerating = false;
    }

    public void UpdateTile(MapTile[,] mapTile, MapTile tileToUpdate) {
        isGenerating = true;

        int width = mapTile.GetLength(0);
        int height = mapTile.GetLength(1);

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        foreach(Vector3Int b in bounds.allPositionsWithin) {
            if(tileToUpdate.position.x + b.x >= 0 && tileToUpdate.position.x + b.x < width && tileToUpdate.position.y + b.y >= 0 && tileToUpdate.position.y + b.y < height) {
                MapTile tile = mapTile[tileToUpdate.position.x + b.x, tileToUpdate.position.y + b.y];
                tile.tile = debugTiles[1];

                foreach(SO_RuleTile rule in rulesForTiles) {
                    if(rule.IsThisTile(tile, mapTile)) {
                        tile.tile = rule.tile;
                        break;
                    }
                }
            }
        }

        isGenerating = false;
    }

    List<MapRegion> GetRegionBySolidTile(MapTile[,] mapTiles) {

        int width = mapTiles.GetLength(0);
        int height = mapTiles.GetLength(1);

        List<MapRegion> regionsBySolidTile = new List<MapRegion>();
        List<Vector2Int> freeTiles = new List<Vector2Int>();

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                if(!mapTiles[x, y].isSolid) {
                    freeTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        while(freeTiles.Count > 0) {

            List<MapTile> tilesForRegion = new List<MapTile>();

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

                tilesForRegion.Add(mapTiles[currentTile.x, currentTile.y]);
            }

            regionsBySolidTile.Add(new MapRegion(tilesForRegion));
        }

        //foreach(MapRegion region in regionsBySolidTile) {
        //    Tile d = debugTiles[Random.Range(0, debugTiles.Count)];
        //    foreach(MapTile t in region.tiles) {
        //        debugTilemap.SetTile(new Vector3Int(t.position.x, t.position.y, 0), debugTiles[0]);
        //    }
        //}

        return regionsBySolidTile;
    }

    public IEnumerator AddItems(List<MapRegion> regions) {
        isGenerating = true;

        foreach(MapRegion region in regions) {

            foreach(SO_Item item in items) {
                int chance = Random.Range(1, 101);

                if(item.CanBeInThisRegion(region)) {
                    if(chance > item.apparitionRatePerRegion) {

                        Vector2 pos = new Vector2();

                        while(pos == new Vector2()) {
                            MapTile t = region.tiles[Random.Range(0, region.tiles.Count)];

                            if(!t.isOccuped) {
                                pos = t.position + new Vector2(0.5f, 0.5f);
                                t.AddItem();
                            }
                        }

                        GameObject instance = Instantiate(item.prefab);

                        instance.transform.position = pos;
                    }
                }
            }

        }

        isGenerating = false;
        yield return null;
    }

    public IEnumerator AddOre(MapTile[,] map) {
        isGenerating = true;

        int width = map.GetLength(0);
        int height = map.GetLength(1);

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        int nbChunk = map.GetLength(0) / rule_chunkSize;

        for(int i = 0; i < nbChunk;i++) {
            for(int j = 0; j < nbChunk;j++) {
                #region Get solid tiles in chunk
                List<MapTile> solidTiles = new List<MapTile>();

                for(int x = 0; x < rule_chunkSize;x++) {
                    for(int y = 0; y < rule_chunkSize;y++) {
                        Vector2Int pos = new Vector2Int(x + i*rule_chunkSize, y + j*rule_chunkSize);
                        if(pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height) {
                            MapTile t = map[x + i * rule_chunkSize, y + j * rule_chunkSize];
                            if(t.type == MapTile.TileType.SOLID) {
                                solidTiles.Add(t);
                            }
                        }
                    }
                }

                #endregion

                #region Create vein

                int nbVein = Random.Range(1, rule_veinPerChunk);

                while(nbVein > 0) {

                    MapTile currentTile = solidTiles[Random.Range(0, solidTiles.Count)];

                    solidTiles.Remove(currentTile);

                    if(!currentTile.isOccuped) {
                        SO_Ore selectedOre = GetRandomOre();
                        int veinSize = Random.Range(0, rule_maxOrePerVein + 1);

                        List<MapTile> possibleTile = new List<MapTile>();

                        possibleTile.Add(currentTile);

                        while(possibleTile.Count > 0 && veinSize > 0) {

                            MapTile t = possibleTile[Random.Range(0, possibleTile.Count)];

                            foreach(Vector3Int b in bounds.allPositionsWithin) {
                                if(t.position.x + b.x >= 0 && t.position.x + b.x < width && t.position.y + b.y >= 0 && t.position.y + b.y < height) {
                                    if(solidTiles.Contains(map[t.position.x + b.x, t.position.y + b.y])) {
                                        if(!map[t.position.x + b.x, t.position.y + b.y].isOccuped) {
                                            possibleTile.Add(map[t.position.x + b.x, t.position.y + b.y]);
                                        }
                                    }
                                }
                            }

                            possibleTile.Remove(t);
                            t.AddOre(selectedOre);
                            veinSize--;
                        }
                        nbVein--;
                    }

                }

                #endregion

            }
        }

        isGenerating = false;
        yield return null;
    }

    SO_Ore GetRandomOre() {
        int maxRate = 0;

        foreach(SO_Ore ore in ores) {
            maxRate += ore.apparitionRate;
        }

        int rate = Random.Range(0, maxRate + 1);

        int value = 0;
        int index = 0;

        while(true) {
            value += ores[index].apparitionRate;

            if(rate < value) {
                return ores[index];
            }

            index++;
        }
    }
}
