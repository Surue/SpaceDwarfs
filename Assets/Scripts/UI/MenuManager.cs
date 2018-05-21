using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    private void Update() {
        
    }

    public void LoadSceneByName(string sceneName) {
        FindObjectOfType<GameManager>().LoadSceneByName(sceneName);
    }

    public void LoadNextLevel() {
        FindObjectOfType<GameManager>().LoadNextLevel();
    }
}
