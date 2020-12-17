using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy {
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private BasicAI ai;
    private GameObject player;
    public GameObject weapon;
    private AudioSource sonidoDaño;
    private AudioSource sonidoMuerte;
    private AudioSource sonidoAtaque;
    private Vida vida;
    private bool dead = false;
    private bool right = true;
    public bool attacking = false;
    public bool hitstunned = false;
    public float speed = 3.5f;
    public float maxPosition = 5f;
    private Vector3 startPosition;
    private float facing;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //evita que se solapen sprites de enemigos distintos
        spriteRenderer.sortingOrder = Random.Range(0, 10000);
        
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<BasicAI>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        sonidoDaño = GetComponent<AudioSource>();
        
		vida = gameObject.GetComponent<Vida>();
        
        startPosition = transform.position;
        facing = transform.localScale.x;
        
	}
	
	// Update is called once per frame
	void Update () {
		if (!dead && vida.currentHealth <= 0){
            die();
        }
        
        if(!dead && !attacking && !hitstunned) {
            int nextMove = ai.nextMove(right);
            switch(nextMove) {
                case 0:
                    move();
                    break;
                case 1:
                    chase();
                    break;
                case 2:
                    attack();
                    break;
            }
        }
	}
    
    private void move() {
        float speedX;
        if (transform.position.x > (startPosition.x + maxPosition)) {
            right = false;
            transform.localScale = new Vector3(-facing, transform.localScale.y, transform.localScale.z);
        } else if (transform.position.x < (startPosition.x - maxPosition)) {
            right = true;
            transform.localScale = new Vector3(facing, transform.localScale.y, transform.localScale.z);
        }
        
        if (right){
            speedX = speed;
        }else{
            speedX = -speed;
        }
        
        execMove(speedX);
    }
    
    private void execMove(float speedX) {
        //ejecucion del movimiento
        if(rb.velocity.y <= 0.000001f && rb.velocity.y >= -0.000001f) {
            rb.velocity = new Vector2(speedX, rb.velocity.y);
        }
    }
    
    private void chase() {
        float speedX;
        if(player.transform.position.x > transform.position.x) {
            speedX = speed;
        } else {
            speedX = -speed;
        }
        
        execMove(speedX);
    }
    
    private void attack() {
        execMove(0);
        animator.SetTrigger("Attack");
        attacking = true;
    }
    
    private void ataqueOn(){
        if(attacking) {
            //sonidoAtaque.Play();
            weapon.SetActive(true);
        }
    }

    private void ataqueWhile(){
        weapon.SetActive(false);
    }

    private void ataqueOff(){
        attacking = false;
    }
    
    override public void takeDamage(Weapon attack) {
        vida.damageToHealth(attack.getDamage(vida));
        damageTaken();
    }
    
    private void damageTaken(){
        //feedback visual
        spriteRenderer.color = Color.red;
        animator.SetTrigger("Hurt");
        //cancelar ataques
        weapon.SetActive(false);
        attacking = false;
        hitstunned = true;
        sonidoDaño.Play();
    }
    
    private void colorNormal(){
        spriteRenderer.color = Color.white;
        hitstunned = false;
    }
    
    private void die() {
        dead = true;
        animator.SetTrigger("Die");
        gameObject.layer = LayerMask.NameToLayer("Dead Units");
        //sonidoMuerte.Play();
    }
    
    private void disappear() {
        gameObject.SetActive(false);
    }
}
