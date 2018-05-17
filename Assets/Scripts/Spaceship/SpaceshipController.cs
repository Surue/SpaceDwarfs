using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour {

    public enum State {
        LANDING, 
        OPENING,
        WAINTING,
        CLOSING,
        TAKING_OFF
    }

    public State state = State.LANDING;
    public bool canTakeOff = false;
    public bool playerIsInside = false;

    Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        switch(state) {
            case State.LANDING:
                break;

            case State.OPENING:
                animator.SetTrigger("Opening");
                state = State.WAINTING;
                break;

            case State.WAINTING:
                if(canTakeOff && playerIsInside) {
                    state = State.CLOSING;
                }
                break;

            case State.CLOSING:
                animator.SetTrigger("Closing");
                state = State.TAKING_OFF;
                break;

            case State.TAKING_OFF:
                animator.SetTrigger("TakeOff");
                break;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            playerIsInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player") {
            playerIsInside = false;
        }
    }
}
