using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinLeader : Boss {
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    private BossAI ai;
    //private GameObject player;
    public GameObject weapon;
    
    private AudioSource sonidoDaño;
    private AudioSource sonidoMuerte;
    private AudioSource sonidoAtaque;
    
    private Vida vida;
    private bool active = false;
    private bool dead = false;
    private bool right = false;
    public bool attacking = false;
    
    public float speed = 3.5f;
    public float maxPosition = 5f;
    
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<BossAI>();
        //player = GameObject.FindGameObjectWithTag("Player");
        
        sonidoDaño = GetComponent<AudioSource>();
        
		vida = gameObject.GetComponent<Vida>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!dead && vida.currentHealth <= 0){
            die();
        }
        
        if(active && !dead && !attacking) {
            int nextMove = ai.nextMove(right);
            switch(nextMove) {
                case 0:
                    move();
                    break;
                case 1:
                    turn();
                    attack();
                    break;
                case 2:
                    attack();
                    break;
            }
        }
	}
    
    override public void startBoss() {
        Invoke("startMoving", 0.5f);
        //barra de vida y tal
    }
    
    private void startMoving() {
        active = true;
        animator.SetTrigger("Start");
    }
    
    private void move() {
        float speedX;

        if (right){
            speedX = speed;
        }else{
            speedX = -speed;
        }
        
        execMove(speedX);
    }
    
    private void turn() {
        right = !right;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    
    private void execMove(float speedX) {
        //ejecucion del movimiento
        if(rb.velocity.y <= 0.000001f && rb.velocity.y >= -0.000001f) {
            rb.velocity = new Vector2(speedX, rb.velocity.y);
        }
    }
    
    /*private void chase() {
        float speedX;
        if(player.transform.position.x > transform.position.x) {
            speedX = speed;
        } else {
            speedX = -speed;
        }
        
        execMove(speedX);
    }*/
    
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
        weapon.SetActive(false);
        attacking = false;
    }
    
    override public void takeDamage(Weapon attack) {
        vida.damageToHealth(attack.getDamage(vida));
        damageTaken();
    }
    
    private void damageTaken(){
        //feedback visual
        healthBar.UpdateBar(vida.currentHealth, vida.maxHealth);
        spriteRenderer.color = Color.red;
        Invoke("colorNormal", 0.5f);
        //cancelar ataques
        weapon.SetActive(false);
        attacking = false;
        sonidoDaño.Play();
    }
    
    private void colorNormal(){
        spriteRenderer.color = Color.white;
    }
    
    private void die() {
        dead = true;
        animator.SetTrigger("Die");
        gameObject.layer = LayerMask.NameToLayer("Dead Units");
        //sonidoMuerte.Play();
        activateDeathEvent();
    }
    
    private void disappear() {
        healthBar.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
