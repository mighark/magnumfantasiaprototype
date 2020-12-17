using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    public Vida user;
    public bool isMagic = false;
    public bool isAttack = true;
    public float atkMult = 1f;
    public float dmgMult = 1f;
    public float critMult = 2f;
    
    private bool isCrit = false;
    
    private SpriteRenderer sprite;
    private Animator anim;
    private AudioSource attackAudio;

	// Use this for initialization
	void Awake() {
		sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        attackAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy")) {
            Enemy enemy = other.GetComponent<Enemy>();
            if(enemy) {
                enemy.takeDamage(this);
                if(isAttack) {
                    gameObject.SendMessageUpwards("onAttackEffect", SendMessageOptions.DontRequireReceiver);
                }
            }
            onImpact();
        }
    }
    
    public void setUser(Vida user) {
        this.user = user;
        GetComponent<Collider2D>().enabled = true;
    }
    
    public void readyAttack() {
        anim.SetBool("Attack", true);
        if(user.critHit()){
            sprite.color = Color.red;
            isCrit = true;
        } else {
            sprite.color = Color.white;
            isCrit = false;
        }
        user.buffs.onStartAttackEffect(user);
        //attackAudio.Play();
    }
    
    public void finishAttack() {
        //attackAudio.Stop();
        gameObject.SetActive(false);
    }
    
    virtual public int getDamage(Vida target) {
        int atk;
        int dmg;
        float crit = 1f;
        
        if(isAttack) {
            if(isCrit) {
                crit = critMult;
            }
            user.critUp();
        }
        
        if(isMagic) {
            atk = (int) Mathf.Floor(user.magatk * atkMult);
            dmg = atk - target.magdef;
        } else {
            atk = (int) Mathf.Floor(user.attack * atkMult);
            dmg = atk - target.defense;
        } 
        return (int) Mathf.Floor(dmg * dmgMult * crit);
    }
    
    virtual public void onImpact() {
        
    }
    
}
