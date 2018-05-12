using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PathFinding:MonoBehaviour {

    //public Transform target;
    NavigationAI navigationGraph;

    //List<NavigationAI.Node> openGraph;

	// Use this for initialization
	void Start () {
        navigationGraph = FindObjectOfType<NavigationAI>();
	}

    public List<Vector2> GetPathFromTo(Transform from, Transform target) {
        NavigationAI.Node start = navigationGraph.GetClosestNode(from.position);
        NavigationAI.Node end = navigationGraph.GetClosestNode(target.position);

        //Get A* sorted list
        Astar(start, end);

        List<Vector2> path = new List<Vector2>();
        path.Add(end.position);

        BuildShortestPath(path, end);

        path.Reverse();

        return path;
    }

    void BuildShortestPath(List<Vector2> path, NavigationAI.Node node) {
        if(node.parent == null) {
            return;
        }

        path.Add(node.position);
        BuildShortestPath(path, node.parent);
    }

    void Astar(NavigationAI.Node start, NavigationAI.Node end) {
        foreach(NavigationAI.Node node in navigationGraph.graph) {
            node.Reset();
            node.SetCost(Vector2.Distance(node.position, end.position));
        }
        
        //Make sure start position cost == 0
        start.cost = 0;

        List<NavigationAI.Node> openGraph = new List<NavigationAI.Node>();
        openGraph.Add(start);

        do {
            openGraph = openGraph.OrderBy(x => x.totalCost + x.cost).ToList();

            NavigationAI.Node node = openGraph.First();
            openGraph.Remove(node);

            

            foreach(NavigationAI.Node childNode in node.neighbors.OrderBy(x => x.cost + x.totalCost)) {
                Debug.Log(childNode.cost +" "+childNode.totalCost);
                float newCost = node.totalCost + Vector2.Distance(node.position, childNode.position);
                if(childNode.visited) continue;
                //if childNode.totalCost = 0 => childNode == end OR if cost is smaller than previous one
                if(childNode.totalCost == 0 || newCost < childNode.totalCost) {
                    childNode.SetTotalCost(newCost);
                    childNode.SetParent(node);
                    if(!openGraph.Contains(childNode)) {
                        openGraph.Add(childNode);
                    }
                }
            }
            if(node.position == end.position) return;
            node.visited = true;

        } while(openGraph.Count != 0);
    }
}
