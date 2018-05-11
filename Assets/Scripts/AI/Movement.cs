using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    PlayerController player;

    Rigidbody2D body;

    PathFinding aStar;

    List<Vector2> path;

    int index = 0;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerController>();

        aStar = FindObjectOfType<PathFinding>();

        path = null;

        StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(1);

        path = aStar.GetPathFromTo(transform, player.transform);
    }
	
	// Update is called once per frame
	void Update () {
        if(path != null && index != path.Count) {
            if(Vector2.Distance(transform.position, path[index]) < 0.2f) {
                index++;
            }

            Vector2 movement = path[index] - (Vector2)transform.position;

            movement = movement.normalized;

            body.velocity = movement * 4.5f;
        } 
    }

    private void OnDrawGizmos() {
        if(path != null)
        foreach(Vector2 pos in path) { 
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, 0), 0.1f);
        }
    }
}
