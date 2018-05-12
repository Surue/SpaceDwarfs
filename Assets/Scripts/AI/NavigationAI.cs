using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Linq;

public class NavigationAI : MonoBehaviour {

    public class Node {
        public Vector2 position;
        public Vector2Int positionInt;

        public List<Node> neighbors = null;

        public bool isActive;

        //A* variables
        public float cost;
        public float totalCost;
        public Node parent = null;
        public bool visited = false;

        public void SetTotalCost(float f) {
            totalCost = f;
        }

        public void SetParent(Node p) {
            parent = p;
        }

        public void SetCost(float f) {
            cost = f;
        }

        public void Reset() {
            visited = false;
            cost = 1;
            totalCost = 0;
            parent = null;
        }
    }

    public bool DebugMode = true;

    [HideInInspector]
    public Tilemap solidTilemap;
    public Node[,] graph;

    public void GenerateNavigationGraph() {

        //Get width and height of tilemap
        int width = solidTilemap.cellBounds.size.x;
        int height = solidTilemap.cellBounds.size.y;

        //If graph does not existe -> instanciate it
        if(graph == null) {
            graph = new Node[width, height];
            for(int x = 0;x < width;x++) {
                for(int y = 0;y < height;y++) {
                    graph[x, y] = new Node();
                }
            }
        }
        
        //Go through tilemap to find free tile
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if(graph[x, y].neighbors == null) {
                    graph[x, y].neighbors = new List<Node>();
                    graph[x, y].position = new Vector2(x + solidTilemap.cellSize.x / 2.0f + solidTilemap.cellBounds.x, y + solidTilemap.cellSize.y / 2.0f + solidTilemap.cellBounds.y);
                    graph[x, y].positionInt = new Vector2Int(x, y);
                } else {
                    graph[x, y].neighbors.Clear();
                }

                if(!solidTilemap.HasTile(new Vector3Int(x + solidTilemap.cellBounds.x, y + solidTilemap.cellBounds.y, 0))) {
                    graph[x, y].isActive = true;
                } else {
                    graph[x, y].isActive = false;
                }
            }
        }

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for(int x = 0;x < width;x++) {
            for(int y = 0;y < height;y++) {
                if(graph[x, y].isActive) {

                    foreach(Vector3Int b in bounds.allPositionsWithin) {
                        if(b.x == 0 && b.y == 0) continue;
                        if(x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height) {

                            if(graph[x + b.x, y + b.y].isActive) {
                                if(b.x == 0 || b.y == 0) {
                                    graph[x, y].neighbors.Add(graph[x + b.x, y + b.y]);
                                } else {
                                    if(graph[x, y + b.y].isActive && graph[x + b.x, y].isActive) {
                                        graph[x, y].neighbors.Add(graph[x + b.x, y + b.y]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public List<Node> GetGraph() {
        List<Node> freeNode = new List<Node>();

        foreach(Node node in graph) {
            if(node.isActive) {
                freeNode.Add(node);
            }
        }

        return freeNode;
    }

    public Node GetClosestNode(Vector2 pos) {
        return graph[(int)pos.x - solidTilemap.cellBounds.x, (int)pos.y - solidTilemap.cellBounds.y];
    } 

    private void OnDrawGizmos() {
        if(DebugMode) {
            if(graph != null) {
                foreach(Node node in graph) {
                    if(node.isActive)
                        Gizmos.DrawWireSphere(new Vector3(node.position.x, node.position.y, 0), 0.1f);

                    foreach(Node neighbor in node.neighbors) {
                        Gizmos.DrawLine(node.position, neighbor.position);
                    }
                }
            }
        }
    }
}
