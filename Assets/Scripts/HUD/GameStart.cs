using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void startGame() {
        SceneManager.LoadSceneAsync("Dungeon1");
    }
    
    public void quitGame() {
        Application.Quit();
    }
}
