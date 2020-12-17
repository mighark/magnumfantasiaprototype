using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageFinish : MonoBehaviour {
    public string nextScene = "Map";
    public GameObject victoryScreen;
    public AudioClip fanfare;
    
    private CameraControl cameraControl;

    void Start() {
        cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void bossDead() {
        Invoke("playFanfare", 0.5f);
        Invoke("showVictory", 2f);
        //Invoke("finishStage", 5f);
    }
    
    public void playFanfare() {
        cameraControl.changeMusic(fanfare);
    }
    
    public void showVictory() {
        victoryScreen.SetActive(true);
    }
    
    public void finishStage() {
        SceneManager.LoadScene(nextScene);
    }
}
