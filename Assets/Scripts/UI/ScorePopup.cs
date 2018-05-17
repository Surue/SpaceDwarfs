using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePopup : MonoBehaviour {

    Animator animator;
    public Text scoreText;

	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
	}
	
	public void SetText(string t) {
        scoreText.text = t;
    }
}
