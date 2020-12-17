using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour {
    [SerializeField] private LayerMask playerLayerMask;
    private Collider2D bc;
    public float rayDistance = 10f;
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
            hit = Physics2D.BoxCast(bc.bounds.center, new Vector3(bc.bounds.size.x / 2, bc.bounds.size.y / 1.5f, bc.bounds.size.z / 2), 0f, Vector2.right, rayDistance, playerLayerMask);
        } else {
            hit = Physics2D.BoxCast(bc.bounds.center, new Vector3(bc.bounds.size.x / 2, bc.bounds.size.y / 1.5f, bc.bounds.size.z / 2), 0f, Vector2.left, rayDistance, playerLayerMask);
        }
        
        if(hit) {
            if(hit.distance < attackDistance) {
                //jugador en rango de ataque
                return 2;
            } else {
                //jugador en rango de vision
                return 1;
            }
        } else {
            //jugador no en vision
            return 0;
        }
    }
}
