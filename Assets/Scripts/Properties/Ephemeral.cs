using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ephemeral : MonoBehaviour {
    public float lifeTime = 0.5f;
	// Use this for initialization
	void Start () {
		Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
