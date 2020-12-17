using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public GameObject gameOverText;
    public GameObject continueButton;
    public GameObject quitButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void gameOver() {
        Animator anim = gameOverText.GetComponent<Animator>();
        if(anim) {
            anim.SetTrigger("FadeIn");
        }
        Invoke("showButtons", 5f);
    }
    
    void showButtons() {
        continueButton.SetActive(true);
        quitButton.SetActive(true);
    }
    
    public void continueGame() {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void quitGame() {
        SceneManager.LoadScene("MainMenu");
    }
}
