using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerController : MonoBehaviour {

    float life = 50;

    [Header("Movements")]
    float speed = 2.5f;
    float maxSpeed = 2.5f;

    CircleCollider2D zoneDetection;

    Rigidbody2D body;

    NavigationAI graph;
    PathFinding aStart;

    List<Vector2> path;

    Transform target;

    Animator animator;

    [SerializeField]
    GameObject bloodSplasherPrefab;

    bool lookingRight = true;

    float attackTime = 0.8f;
    float timer = 0;

    List<Vector2> lastsPosition;

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
        animator = GetComponentInChildren<Animator>();

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
                path = aStart.GetPathFromTo(transform, node, false);
                if(path == null) {
                        
                } else {
                    state = State.MOVING;
                    lastsPosition = new List<Vector2>();
                }
                
                break;

            case State.MOVING:
                if(playerSeen) {
                    path = null;
                    state = State.MOVE_TOWARDS_PLAYER;
                    break;
                }

                if(Vector2.Distance(transform.position, path[0]) < 0.1f) {
                    path.RemoveAt(0);

                    if(path.Count == 0) {
                        state = State.IDLE;
                        break;
                    }
                }

                Vector2 direction = path[0] - (Vector2)transform.position;

                direction.Normalize();

                body.velocity = speed * direction;

                if(body.velocity.magnitude > maxSpeed) {
                    body.velocity = body.velocity.normalized * maxSpeed;
                }

                lastsPosition.Add(transform.position);

                if(lastsPosition.Count > 10) {
                    Vector2 pos = lastsPosition[lastsPosition.Count - 1];
                    bool notMoving = true;
                    for(int i = 1; i < 10;i++) {
                        if(pos != lastsPosition[lastsPosition.Count - 1 - i]) {
                            notMoving = false;
                            continue;
                        }
                    }

                    if(notMoving) {
                        state = State.IDLE;
                    }
                }
                break;

            case State.ATTACKING:
                body.velocity = Vector2.zero;

                timer -= Time.deltaTime;

                if(timer < 0) {
                    state = State.IDLE;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach(Collider2D collider in colliders) {
                        if(collider.GetComponent<PlayerController>()) {
                            collider.GetComponent<PlayerController>().TakeDamage(10);
                        }
                    }
                }
                break;

            case State.MOVE_TOWARDS_PLAYER:
                if(Vector2.Distance(transform.position, target.position) < 0.75f) {
                    state = State.ATTACKING;
                    animator.SetTrigger("attack");
                    timer = attackTime;
                    break;
                }

                if(!playerSeen) {
                    state = State.IDLE;
                    break;
                }


                Vector2 movement2 = (Vector2)target.position - (Vector2)transform.position;

                direction = movement2.normalized;

                body.velocity = direction * 2.5f;
                break;
        }

        if(lookingRight && body.velocity.x < 0) {
            animator.SetBool("lookingRight", false);
            lookingRight = false;
        }

        if(!lookingRight && body.velocity.x > 0) {
            animator.SetBool("lookingRight", true);
            lookingRight = true;
        }

        animator.SetFloat("speed", body.velocity.x);
	}

    bool CheckPlayerPresence() {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] colliders = new Collider2D[100];

        int count = zoneDetection.OverlapCollider(contactFilter, colliders);

        for(int i = 0; i < count; i++) {
            if(colliders[i].tag == "Player") {

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

    public float TakeDamage(float d) {
        if(life - d < 0) {
            Destroy(this.gameObject);
            return Mathf.Abs(life-d);
        }

        life -= d;

        return 0;
    }

    private void OnDrawGizmos() {
        if(path != null) {
            foreach(Vector2 pos in path) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, 0), 0.1f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<Bullet>()) {

            Instantiate(bloodSplasherPrefab, collision.transform.position, Quaternion.identity);

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            float diff = Mathf.Abs(life - bullet.energy);

            if(bullet.energy > 0) {
                life -= bullet.energy;
            }

            bullet.energy -= diff;

            if(life <= 0) {
                Score s = GetComponent<Score>();
                if(s != null) {
                    s.DisplayScore();
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<Bullet>()) {

            //Instantiate(bloodSplasherPrefab, collision.transform.position, Quaternion.identity);

            //Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            //float diff = Mathf.Abs(life - bullet.energy);

            //if(bullet.energy > 0) {
            //    life -= bullet.energy;
            //}

            //bullet.energy -= diff;

            //if(life <= 0) {

            //    Destroy(gameObject);
            //}
        }
    }
}
