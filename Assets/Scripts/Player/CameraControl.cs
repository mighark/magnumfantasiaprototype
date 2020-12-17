using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    private Transform player;
    private AudioSource music;
    
    public float x = 0;
    public float y = 0;
    public float z = 0;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
        music = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
         // Temporary vector
         Vector3 temp = player.position;
         temp.x = temp.x + x;
         temp.y = temp.y + y;
         temp.z = z;
         // Assign value to Camera position
         transform.position = temp;
	}
    
    public void changeMusic(AudioClip newMusic) {
        music.clip = newMusic;
        music.Play();
    }
}
