using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public GameObject floatingScorePrefab;
    public GameObject canvas;

    [Header("Size text")]
    public int minSizeFont = 24;
    public int maxSizeFont = 34;
    public int lowestScore = 1;
    public int heighestScore = 1000;

    public void CreateText(int score, Vector3 location) {
        ScorePopup instance = Instantiate(floatingScorePrefab).GetComponent<ScorePopup>();
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location);
        instance.transform.SetParent(canvas.transform, false);

        instance.transform.position = screenPosition;
        instance.SetText(score.ToString());

        score = Mathf.Clamp(score, lowestScore, heighestScore);

        float percent = (heighestScore / 100.0f) * (score / 100.0f);

        float size = (maxSizeFont - minSizeFont) * (percent / 100);
        size += minSizeFont;
        instance.SetSize((int)size);

    }

    public void DisplayScore(int score, Vector3 location) {
        FindObjectOfType<PlayerInfo>().AddScore(score);
        CreateText(score, location);
    }
}
