using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Weapon {
    public GameObject explosionPrefab;
    
    // Use this for initialization
	void Start () {

	}
    
    override public void onImpact() {
        Instantiate(explosionPrefab);
        Destroy(gameObject);
    }

}
