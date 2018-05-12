using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerController : MonoBehaviour {

    CircleCollider2D zoneDetection;

    Rigidbody2D body;

    NavigationAI graph;
    PathFinding aStart;

    List<Vector2> path;

    Transform target;

    enum State {
        IDLE,
        MOVING,
        ATTACKING,
        MOVE_TOWARDS_PLAYER
    }

    State state = State.IDLE;

	// Use this for initialization
	void Start () {
        zoneDetection = GetComponent<CircleCollider2D>();
        body = GetComponent<Rigidbody2D>();

        graph = FindObjectOfType<NavigationAI>();
        aStart = FindObjectOfType<PathFinding>();

        path = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {
        bool playerSeen = CheckPlayerPresence();

        switch(state) {
            case State.IDLE:
                if(playerSeen) {
                    state = State.MOVE_TOWARDS_PLAYER;
                    break;
                }

                //Find new points to patrol
                NavigationAI.Node node = graph.GetRandomPatrolsPoint();
                path = null;
                path = aStart.GetPathFromTo(transform, node);
                if(path == null) {
                        
                } else {
                    state = State.MOVING;
                }
                
                break;

            case State.MOVING:
                if(playerSeen) {
                    path = null;
                    state = State.MOVE_TOWARDS_PLAYER;
                    break;
                }

                if(Vector2.Distance(transform.position, path[0]) < 0.2f) {
                    path.RemoveAt(0);

                    if(path.Count == 0) {
                        state = State.IDLE;
                        break;
                    }
                }

                Vector2 movement = path[0] - (Vector2)transform.position;

                movement = movement.normalized;

                body.velocity = movement * 2.5f;
                break;

            case State.ATTACKING:
                Debug.Log("ATTACK MOTHERFUCKER");
                break;

            case State.MOVE_TOWARDS_PLAYER:
                if(Vector2.Distance(transform.position, target.position) < 0.75f) {
                    state = State.ATTACKING;
                    break;
                }

                if(!playerSeen) {
                    state = State.IDLE;
                    break;
                }


                Vector2 movement2 = (Vector2)target.position - (Vector2)transform.position;

                movement = movement2.normalized;

                body.velocity = movement * 2.5f;
                break;
        }
	}

    bool CheckPlayerPresence() {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] colliders = new Collider2D[100];

        int count = zoneDetection.OverlapCollider(contactFilter, colliders);

        for(int i = 0; i < count; i++) {
            if(colliders[i].tag == "Player") {

                Collider2D[] selfColliders = GetComponents<Collider2D>();

                RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, colliders[i].transform.position - transform.position, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Monster")));
                Debug.DrawRay(transform.position, colliders[i].transform.position - transform.position);
                if(hitPlayer.collider.tag != "Player") {
                    return false;
                } else {
                    target = colliders[i].transform;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos() {
        if(path != null)
            foreach(Vector2 pos in path) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, 0), 0.1f);
            }
    }
}
