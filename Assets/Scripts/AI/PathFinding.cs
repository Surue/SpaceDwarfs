using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour {

    //public Transform target;
    NavigationAI navigationGraph;

    List<Vector2> path;

    Rigidbody2D body;

	// Use this for initialization
	void Start () {
        navigationGraph = FindObjectOfType<NavigationAI>();
        //target = FindObjectOfType<PlayerController>().transform;
        body = GetComponent<Rigidbody2D>();

        //navigationGraph.GetPathTo(target);
        path = new List<Vector2>();
        path.Insert(0, new Vector2(0, 0));
        path.Insert(1, new Vector2(-10, 0));
        path.Insert(2, new Vector2(-10, -10));
        path.Insert(3, new Vector2(0, -10));

        Debug.Log(path.Count);
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(path[0]);
		if(Vector2.Distance(transform.position, path[0]) < 1) {
            path.RemoveAt(0);
        }

        Vector2 movement;

        if(transform.position.x < path[0].x) {
            movement.x = 1;
        } else {
            movement.x = -1;
        }

        if(transform.position.y < path[0].y) {
            movement.y = 1;
        } else {
            movement.y = -1;
        }

        body.velocity = movement * 4.5f;
    }
}
