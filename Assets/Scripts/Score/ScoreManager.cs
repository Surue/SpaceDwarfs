using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public GameObject floatingScorePrefab;
    public GameObject canvas;

    public void CreateText(string text, Transform location) {
        ScorePopup instance = Instantiate(floatingScorePrefab).GetComponent<ScorePopup>();
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.SetParent(canvas.transform, false);

        instance.transform.position = screenPosition;

        instance.SetText(text);
    }
}
