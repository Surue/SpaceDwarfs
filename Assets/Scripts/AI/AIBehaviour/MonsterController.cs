using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

    public SO_MonsterStats stats;

    private bool aiActive = true;

    public SO_State currentState;

    public SO_State defaultState;

    Animator animator;

    bool lookingRight = true;

    [SerializeField]
    GameObject bloodSplasherPrefab;
    [SerializeField]
    float life = 50;

    [HideInInspector]
    public NavigationAI graph;
    [HideInInspector]
    public PathFinding aStart;
    [HideInInspector]
    public List<Vector2> wayPointList;
    [HideInInspector]
    public int nextWayPoint = 0;
    [HideInInspector]
    public List<Vector2> path = null;
    [HideInInspector]
    public Rigidbody2D body;
    [HideInInspector]
    public Transform chaseTarget;
    [HideInInspector]
    public Vector2 viewDirection;
    [HideInInspector]
    public float stateTimeElapsed = 0;

    PlayerController player;

    // Use this for initialization
    void Awake () {
        graph = FindObjectOfType<NavigationAI>();
        aStart = FindObjectOfType<PathFinding>();

        wayPointList = new List<Vector2>();
        path = new List<Vector2>();

        body = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerController>();

        animator = GetComponentInChildren<Animator>();
    }

    void Update() {
        if(!aiActive) return;

        currentState.UpdateState(this);

        if(animator != null) {
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
    }

    private void OnDrawGizmos() {
        if(path != null) {
            foreach(Vector2 pos in path) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, 0), 0.1f);
            }
        }

        if(wayPointList != null) {
            foreach(Vector2 pos in wayPointList) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, 0), 0.15f);
            }
        }
    }

    public void TransitionToState(SO_State nextState) {
        if(nextState != defaultState) {
            currentState = nextState;
            OnExistState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration) {
        stateTimeElapsed += Time.deltaTime;

        if( stateTimeElapsed >= duration) {
            stateTimeElapsed = 0;
            return true;
        }

        return false;
    }

    private void OnExistState() {
        path = null;
        stateTimeElapsed = 0;
    }

    public void Attack(float damagePoint) {
        player.TakeDamage(damagePoint);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<Bullet>()) {

            Instantiate(bloodSplasherPrefab, collision.transform.position, Quaternion.identity);

            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            float diff = Mathf.Abs(life - bullet.damage);

            if(bullet.damage > 0) {
                life -= bullet.damage;
            }

            bullet.damage -= diff;

            if(life <= 0) {
                Score s = GetComponent<Score>();
                if(s != null) {
                    s.DisplayScore();
                }

                Destroy(gameObject);
            }
        }
    }

}
