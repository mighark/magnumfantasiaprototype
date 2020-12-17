using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour {
    public Boss boss;
    public GameObject healthBar;
    public AudioClip bossMusic;
    
    private CameraControl cameraControl;
    
    void Start() {
        cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.CompareTag("Player")) {
            boss.startBoss();
            cameraControl.changeMusic(bossMusic);
            healthBar.SetActive(true);
            Destroy(gameObject);
        }

	}
}
