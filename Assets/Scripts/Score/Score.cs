using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public int score = 0;

    ScoreManager manager;

    private void Start() {
        manager = FindObjectOfType<ScoreManager>();
    }

    public void DisplayScore() {
        PlayerInfo.Instance.AddScore(score);
        manager.CreateText(score.ToString(), transform);
    }
}
