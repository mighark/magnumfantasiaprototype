using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {
    private Vida vida;
    public float knockbackForce = 5f;

	// Use this for initialization
	void Start () {
		vida = transform.parent.gameObject.GetComponent<Vida>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    
    void OnTriggerEnter2D(Collider2D collider){
        //calcula el daño por contacto
        if (collider.CompareTag("Player")) {
            JugadorControl player = collider.GetComponent<JugadorControl>();
            if(player) {
                player.takeDamage(this);
            }
            
        }

	}
    
    public void applyKnockback(Transform target) {
        int dir = 1;
        if(transform.position.x - target.position.x > 0) {
            dir = -1;
        }
        target.SendMessage("knockback", knockbackForce * dir);
    }
    
    public int getDamage(Vida target) {
        return vida.attack - target.defense;
    }
}
