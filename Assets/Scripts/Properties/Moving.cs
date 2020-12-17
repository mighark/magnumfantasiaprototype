using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {
    private Rigidbody2D rb;
    public Vector3 velocity;
    public float moveDir = 0;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		rb.velocity = new Vector3(velocity.x * moveDir, velocity.y, velocity.z);
	}
    
    public void setMoveDir(float dir) {
        moveDir = dir;
        transform.localScale = new Vector3(transform.localScale.x * moveDir, transform.localScale.y, transform.localScale.z);
    }
}
