using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplasher : MonoBehaviour {

    SpriteRenderer spriteRendere;

    Animator animatorController;

    [SerializeField]
    List<AnimationClip> animations;

	// Use this for initialization
	void Start () {
        animatorController = GetComponent<Animator>();
        AnimationClip anim = animations[Random.Range(0, animations.Count)];
        animatorController.Play(anim.name);

        Destroy(gameObject, anim.length);
	}
}
