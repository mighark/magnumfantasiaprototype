using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JugadorControl : MonoBehaviour {

    //Colliders
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    //Colliders de habilidades
    public GameObject arma;
    private Weapon armaScript;
    [SerializeField] private LayerMask groundLayerMask; //para isGrounded
    //Variables para sprites, animación y audio
    private SpriteRenderer mySpriteRenderer;
    private Animator animator;
    public AudioSource sonidoDaño;
    //Variables de HUD
    public HealthBar healthBar;
    public HealthBar manaBar;
    public GameOver gameOver;
    //Inventario
    
    //Datos de personaje
    public float speed = 8.0f;
    public float speedJump = 30.0f;
    private float currentSpeed;
    private float currentSpeedJump;
    private Vida vida;
    public int manaPerHit = 2;
    public int manaPerGetHit = 1;
    public int manaPerBlock = 1;
    public bool dead = false;

    //Variables que controlan los saltos
    private bool canControl = true;
    private bool jump = false;
    
    //Variables que controlan el uso de habilidades
    private bool attacking = false;
    private bool blocking = false;
    private string casting = "";
    private SkillManager skillManager;
    private float jugadorScale;

    void Start () {
        //Obtener componentes
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        armaScript = arma.GetComponent<Weapon>();

        //Componentes audiovisual
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        //Inicializar variables internas
        vida = GetComponent<Vida>();
        currentSpeed = speed;
        currentSpeedJump = speedJump;
        jugadorScale = transform.localScale.x;
        manaBar.UpdateBar(vida.currentMana, vida.maxMana);
        skillManager = GetComponent<SkillManager>();
        
    }
	
	// Update is called once per frame
	void Update () {
        
        if(Time.timeScale == 1) { //Si el juego no está en pausa

            //Obtienes el input
            float horizontalInput = Input.GetAxis("Horizontal");
            bool jumpInput = Input.GetButtonDown("Jump"); 
            bool ataqueInput = Input.GetButtonDown("Attack");
            bool blockInput = Input.GetButtonDown("Block");
            bool[] skillInput = new bool[6];

            for(int i = 0; i < skillInput.Length; i++) {
                skillInput[i] = Input.GetButtonDown("Skill" + i);
            }
        
            //Controla el giro del personaje y de los colliders de sus habilidades
            if(checkControl()){
                if (horizontalInput < 0){
                    transform.localScale = new Vector3(-jugadorScale, transform.localScale.y, transform.localScale.z);
                }else if (horizontalInput > 0){
                    transform.localScale = new Vector3(jugadorScale, transform.localScale.y, transform.localScale.z);
                }
            }

            //Controla el salto
            if (jumpInput && !jump && isGrounded() && checkControl()) {
                jump = true;
                animator.SetBool("Jump", true);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, speedJump), ForceMode2D.Impulse);
            } else if (jump && isGrounded()){
                jump = false;
                ///doblejump = false;
                animator.SetBool("Jump", false);
            }

            comprobarAtaque(ataqueInput);
            comprobarBlock(blockInput);
            comprobarSkill(skillInput);

            //Controla el movimiento
            if (checkControl()){
                animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
                rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
            } else {
                animator.SetFloat("Speed", 0);
                if(canControl) {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }

            //Controla la muerte
            if(vida.currentHealth <= 0 && !dead){
                dead = true;
                animator.SetTrigger("Dead");
                gameObject.layer = LayerMask.NameToLayer("Dead Units");
                Invoke("die", 0.8f);
            }
        }
        
    }

    public bool isGrounded(){
        RaycastHit2D ray;
        ray = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.1f, groundLayerMask);
        return ray.collider != null;
    }
    
    public bool checkControl() {
        return canControl && !attacking && !blocking && !dead && casting == "";
    }

    //Controla el uso de habilidades
    void comprobarAtaque(bool ataqueInput){
        if(ataqueInput && checkControl() && isGrounded()){
            attacking = true;
            animator.SetTrigger("Attack");
            ///audioSource[1].PlayDelayed(0.05f);
            ///Invoke("ataqueOn", 0.15f);
        }
    }
    
    void comprobarBlock(bool blockInput){
        if(blockInput && checkControl() && isGrounded()){
            blocking = true;
            animator.SetTrigger("Block");
        }
    }
    
    void comprobarSkill(bool[] skillInput) {
        if(checkControl() && isGrounded()) {
            for(int i = 0; i < skillInput.Length; i++) {
                if(skillInput[i]) {
                    Skill skill = skillManager.skills[i];
                    //if its not on cd and have enough mana
                    if(skillManager.canCastSkill(i) && vida.currentMana >= skill.manaCost) {
                        spendMana(skill.manaCost);
                        skillManager.cooldownSkill(i);
                        skill.attemptSkill(vida);
                        if(skill.castTime > 0) {
                            casting = skill.castAnim;
                            animator.SetBool(casting, true);
                            Invoke("endCast", skill.castTime);
                        }
                    }
                    break;
                }
            }
        }
    }
    
    void endCast() {
        animator.SetBool(casting, false);
        casting = "";
    }
    
    public bool isFacingRight() {
        return transform.localScale.x < 0;
    }
    
    public void takeDamage(EnemyWeapon attack) {
        if(blocking) {
            if(transform.position.x - attack.transform.position.x > 0) {
                if(!isFacingRight()) {
                    gainMana(manaPerBlock);
                    return;
                }
            } else {
                if(isFacingRight()) {
                    gainMana(manaPerBlock);
                    return;
                }
            }
        }
        vida.damageToHealth(attack.getDamage(vida));
        attack.applyKnockback(transform);
        vida.critReset();
        gainMana(manaPerGetHit);
        damageTaken();
    }
    
    private void damageTaken(){
        if(!dead) {
            //feedback visual y mercy invincibility
            healthBar.UpdateBar(vida.currentHealth, vida.maxHealth);
            mySpriteRenderer.color = Color.red;
            animator.SetTrigger("Hurt");
            if(attacking)
                ataqueOff();
            if(blocking)
                blockOff();
            sonidoDaño.Play();
        }
    }
        
    private void knockback(float dir) {
        CancelInvoke("returnControl");
        canControl = false;
        rb.AddForce(new Vector2(dir, 0), ForceMode2D.Impulse);
        Invoke("returnControl", 0.4f);
    }
    
    private void returnControl() {
        canControl = true;
        mySpriteRenderer.color = Color.white;
    }

    //Controla el proceso de muerte y "respawn"
    private void die(){
        gameOver.gameOver();
    }
    
    private void respawn(){

    }

    public void damageOverTime(int dmg) {
        
    }

    public void damageSpeed(float speed, float speedJump){
	this.speed = speed;
	this.speedJump = speedJump;
	cambioColor(Color.yellow);
	CancelInvoke("restoreSpeed");
	Invoke("restoreSpeed", 2f);
    }

    public void damageEnergyOverTime(int dmg){
	    
    }

    public void restoreSpeed(){
        this.speed = currentSpeed;
        this.speedJump = currentSpeedJump;
        ResetColor();
    }
    
    private void gainMana(int mana) {
        vida.gainMana(mana);
        manaBar.UpdateBar(vida.currentMana, vida.maxMana);
    }
    
    private void spendMana(int mana) {
        gainMana(-mana);
    }
    
    private void ataqueOn(){
        if(attacking) {
            arma.SetActive(true);
            armaScript.readyAttack();
        }
    }
    
    private void ataqueOff(){
        armaScript.finishAttack();
        attacking = false;
    }
    
    public void onAttackEffect() {
        gainMana(manaPerHit);
    }
    
    private void blockOff(){
        blocking = false;
    }


    private void cambioColor(Color color) {
        mySpriteRenderer.color = color;
    }

    private void ResetColor() {
        mySpriteRenderer.color = Color.white;
    }

    private void ResetTos(){
        
    }

}
