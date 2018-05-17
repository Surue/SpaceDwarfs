using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUpEffect : MonoBehaviour {

    Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
    }
}
