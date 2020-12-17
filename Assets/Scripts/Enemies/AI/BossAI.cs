using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour {
    [SerializeField] private LayerMask sightLayerMask;
    private Collider2D bc;
    //public float rayDistance = 10f;
    public float attackDistance = 1f;

	// Use this for initialization
	void Start () {
		bc = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public int nextMove(bool right) {
        RaycastHit2D hit;

        if(right) {
            hit = Physics2D.BoxCast(bc.bounds.center, new Vector3(bc.bounds.size.x / 2, bc.bounds.size.y / 1.5f, bc.bounds.size.z / 2), 0f, Vector2.right, attackDistance, sightLayerMask);
        } else {
            hit = Physics2D.BoxCast(bc.bounds.center, new Vector3(bc.bounds.size.x / 2, bc.bounds.size.y / 1.5f, bc.bounds.size.z / 2), 0f, Vector2.left, attackDistance, sightLayerMask);
        }
        
        if(hit) {
            if(hit.collider.CompareTag("Player")) {
                //jugador en rango de ataque
                return 2;
            } else {
                //pared en rango de ataque
                return 1;
            }
        } else {
            //nada en vision
            return 0;
        }
    }
}
