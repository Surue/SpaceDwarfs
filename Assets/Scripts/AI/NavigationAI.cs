using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavigationAI : MonoBehaviour {

    struct Node {
        public Vector2 position;
        public List<Node> neighbors;

        public bool isActive;
    }

    public bool DebugMode = true;

    [HideInInspector]
    public Tilemap solidTilemap;

    private Node[,] graph;

    public void GenerateNavigationGraph() {
        //Get width and height of tilemap
        int width = solidTilemap.cellBounds.size.x;
        int height = solidTilemap.cellBounds.size.y;

        //If graph does not existe -> instanciate it
        if(graph == null) {
            graph = new Node[width, height];
        }

        //Go through tilemap to find free tile
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if(graph[x, y].neighbors == null) {
                    graph[x, y].neighbors = new List<Node>();
                    graph[x, y].position = new Vector2(x + solidTilemap.cellSize.x / 2.0f, y + solidTilemap.cellSize.y / 2.0f);
                } else {
                    graph[x, y].neighbors.Clear();
                }

                if(!solidTilemap.HasTile(new Vector3Int(x, y, 0))) {
                    graph[x, y].isActive = true;
                } else {
                    graph[x, y].isActive = false;
                }
            }
        }

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for(int x = 0;x < width; x++) {
            for(int y = 0;y < height; y++) {
                if(graph[x, y].isActive) {

                    foreach(Vector3Int b in bounds.allPositionsWithin) {
                        if(b.x == 0 && b.y == 0) continue;
                        if(x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height) {

                            if(graph[x + b.x, y + b.y].isActive) {
                                graph[x, y].neighbors.Add(graph[x + b.x, y + b.y]);
                            }
                        }
                    }
                } 
            }
        }
    }

    public List<Vector2> GetPathTo(Transform target) {
        List<Vector2> path = new List<Vector2>();

        return path;
    }

    private void OnDrawGizmos() {
        if(DebugMode) {
            if(graph != null) {
                foreach(Node node in graph) {
                    if(node.isActive) {
                        Gizmos.DrawWireSphere(new Vector3(node.position.x, node.position.y, 0), 0.1f);
                    }

                    foreach(Node neighbor in node.neighbors) {
                        Gizmos.DrawLine(node.position, neighbor.position);
                    }
                }
            }
        }
    }
}
