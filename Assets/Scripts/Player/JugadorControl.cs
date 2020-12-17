using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JugadorControl : MonoBehaviour {

    //Colliders
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    ///private BoxCollider2D crouchCollider;
    //Colliders de habilidades
    public GameObject arma;
    private Weapon armaScript;
    ///private GameObject llamaCollider;
    ///private GameObject flecha;
    ///private BoxCollider2D guantesCollider;
    ///private GameObject bomba;
    [SerializeField] private LayerMask groundLayerMask; //para isGrounded
    //Variables para sprites, animación y audio
    private SpriteRenderer mySpriteRenderer;
    private Animator animator;
    public AudioSource sonidoDaño;
    //Variables de HUD
    public HealthBar healthBar;
    public HealthBar manaBar;
    public GameOver gameOver;
    ///private ArmaInterfaz armaInterfaz;
    ///private GameObject muerteTexto;
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
    ///private bool doblejump = false;
    ///Variable que controla el estar agachado
    //private bool crouch = false;
    //Variables que controlan el cambio de habilidades
    ///private int armaEquipada = -1;
    
    //Variables que controlan el uso de habilidades
    private bool attacking = false;
    private bool blocking = false;
    private string casting = "";
    private SkillManager skillManager;
    ///private bool gafas = false;
    private float jugadorScale;
    ///public GameObject llamaFuego;
    ///public GameObject flechaPrefab;
    ///public GameObject bombaPrefab;
    ///public GameObject sifPrefab;
    ///private GameObject sif;
    ///private GameObject objetosInvisibles;
    ///private GameObject objetosFalsos;
    ///private GameObject panelGafas;
    ///public float atacandoTime = 1f;
    ///private int[] costeEnergia = {20, 25, 30, 20, 20, 1, 1, 40}; 
    //Variables que controlan el recibir daño
    ///public float spriteBlinkingTimer = 0.0f;
    ///public float spriteBlinkingMiniDuration = 0.1f;
    ///public float spriteBlinkingTotalTimer = 0.0f;
    ///public float spriteBlinkingTotalDuration = 1.0f;
    ///public bool dañado = false;
    //Variables para plataformas
    ///private Rigidbody2D platform = null;

    void Start () {
        //Obtener componentes
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        armaScript = arma.GetComponent<Weapon>();

        //Componentes audiovisual
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        //Componentes hud
        ///armaInterfaz = GameObject.FindGameObjectWithTag("InterfazArma").GetComponent<ArmaInterfaz>();
        ///muerteTexto = GameObject.FindGameObjectWithTag("GameController").transform.Find("Muerto").gameObject;
        //Componentes gafas
        ///objetosInvisibles = GameObject.FindGameObjectWithTag("Invisible");
        ///objetosFalsos = GameObject.FindGameObjectWithTag("Falso");
        ///panelGafas = GameObject.FindGameObjectWithTag("GameController").transform.Find("Gafas").gameObject;
        //Inicializar variables internas
        vida = GetComponent<Vida>();
        currentSpeed = speed;
        currentSpeedJump = speedJump;
        ///crearColeccionables();
        ///crouchCollider.enabled = false;
        ///espadaCollider.enabled = false;
        jugadorScale = transform.localScale.x;
        ///InvokeRepeating("recargaEnergia", 0f, 0.05f); //Recarga tu energía según pasa el tiempo
        ///cargarPartida();
        //Modo dios
        //Cheat();
        manaBar.UpdateBar(vida.currentMana, vida.maxMana);
        skillManager = GetComponent<SkillManager>();
        
    }
	
	// Update is called once per frame
	void Update () {
        
        if(Time.timeScale == 1) { //Si el juego no está en pausa

            //Obtienes el input
            float horizontalInput = Input.GetAxis("Horizontal");
            bool jumpInput = Input.GetButtonDown("Jump"); 
            ///float crouchInput = Input.GetAxisRaw("Crouch");
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

            //Controla el agacharse
            ///if ((crouchInput == 1) && isGrounded() && !attacking){
                ///crouch = true;
                ///crouchCollider.enabled = true;
                ///bc.enabled = false;
            ///}else{
                ///crouch = false;
                ///crouchCollider.enabled = false;
                ///bc.enabled = true;
            ///}

            ///animator.SetBool("Crouch", crouch);

            //Comprueba el uso y cambio de habilidades
            ///comprobarArma();

            comprobarAtaque(ataqueInput);
            comprobarBlock(blockInput);
            comprobarSkill(skillInput);

            //Controla el parpadeo de "mercy invincibility"
            ///if(dañado){
                ///SpriteBlinkingEffect();
            ///}

            //Controla el movimiento
            if (checkControl()){
                animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
                ///if(crouch){
                ///    rb.velocity = new Vector2(horizontalInput * (speed / 2), rb.velocity.y);
                ///}else{
                    rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
                ///}
                /*if(platform) {
                    if(platform.velocity.y > 0) {
                        rb.velocity += new Vector2(platform.velocity.x, 0);
                    } else {
                        rb.velocity += platform.velocity;
                    }
                }*/
            } else {
                animator.SetFloat("Speed", 0);
                /*if (platform){
                    if (platform.velocity.y > 0){
                        rb.velocity = new Vector2(platform.velocity.x, 0);
                    }else{
                        rb.velocity = platform.velocity;
                    }
                }else{*/
                if(canControl) {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
                ///}
            }

            //Controla la muerte
            if(vida.currentHealth <= 0 && !dead){
                dead = true;
                animator.SetTrigger("Dead");
                gameObject.layer = LayerMask.NameToLayer("Dead Units");
                Invoke("die", 0.8f);
                //Evita que el parpadeo pare con Artorias invisible
                ///mySpriteRenderer.enabled = true;
            }
        }
        
    }

    public bool isGrounded(){
        RaycastHit2D ray;
        ///if (crouch) {
            ///ray = Physics2D.BoxCast(crouchCollider.bounds.center, crouchCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayerMask);
        ///}
        ///else {
            ray = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, 0.1f, groundLayerMask);
        ///}
        return ray.collider != null;
    }
    
    public bool checkControl() {
        return canControl && !attacking && !blocking && !dead && casting == "";
    }

    //Controla el cambio de habilidad
    /*private void comprobarArma(){

        if(!attacking && !gafas){
            if (habilidadInput[(int)ArmasJugador.Espada] && armas[(int)ArmasJugador.Espada]) {
                if(armaEquipada == (int)ArmasJugador.Espada){
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Espada;
                }
            } else if (habilidadInput[(int)ArmasJugador.Arco] && armas[(int)ArmasJugador.Arco]) {
                if(armaEquipada == (int)ArmasJugador.Arco) {
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Arco;
                }
            } else if (habilidadInput[(int)ArmasJugador.Llama] && armas[(int)ArmasJugador.Llama]) {
                if(armaEquipada == (int)ArmasJugador.Llama) {
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Llama;
                }
            } else if (habilidadInput[(int)ArmasJugador.Hojas] && armas[(int)ArmasJugador.Hojas]) {
                if(armaEquipada == (int)ArmasJugador.Hojas) {
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Hojas;
                }
            } else if (habilidadInput[(int)ArmasJugador.Guantes] && armas[(int)ArmasJugador.Guantes]) {
                if(armaEquipada == (int)ArmasJugador.Guantes) {
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Guantes;
                }
            } else if (habilidadInput[(int)ArmasJugador.Gafas] && armas[(int)ArmasJugador.Gafas]) {
                if(armaEquipada == (int)ArmasJugador.Gafas) {
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Gafas;
                }
            } else if (habilidadInput[(int)ArmasJugador.Sif] && armas[(int)ArmasJugador.Sif]) {
                if(armaEquipada == (int)ArmasJugador.Sif) {
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Sif;
                }
            } else if (habilidadInput[(int)ArmasJugador.Bombas] && armas[(int)ArmasJugador.Bombas]) {
                if(armaEquipada == (int)ArmasJugador.Bombas) {
                    armaEquipada = -1;
                }else{
                    armaEquipada = (int)ArmasJugador.Bombas;
                }
            }
        }

        armaInterfaz.actualizarHUD(armaEquipada);
        animator.SetInteger("Arma", armaEquipada);
        
    }*/

    //Controla el uso de habilidades
    void comprobarAtaque(bool ataqueInput){
        if(ataqueInput && checkControl() && isGrounded()){
            attacking = true;
            animator.SetTrigger("Attack");
            ///audioSource[1].PlayDelayed(0.05f);
            ///Invoke("ataqueOn", 0.15f);
        }
    }
        /*}else if(armaEquipada == (int)ArmasJugador.Arco){
            if(!attacking && (currentEnergy > 0) && !crouch && !jump){
                attacking = true;
                atacandoTimer = true;
                currentEnergy -= costeEnergia[(int)ArmasJugador.Arco];
                hudEnergia.UpdateBar(currentEnergy, maxEnergy);
                animator.SetTrigger("Atacando");
                audioSource[5].PlayDelayed(0.15f);
                Invoke("ataqueArcoOn", 0.5f);
                CancelInvoke("atacandoTimerReinicia");
                Invoke("atacandoTimerReinicia", atacandoTime);
            }
        }else if(armaEquipada == (int)ArmasJugador.Llama){
            if(!attacking && (currentEnergy > 0) && !crouch && !jump){
                attacking = true;
                atacandoTimer = true;
                currentEnergy -= costeEnergia[(int)ArmasJugador.Llama];
                hudEnergia.UpdateBar(currentEnergy, maxEnergy);
                animator.SetTrigger("Atacando");
                audioSource[2].PlayDelayed(0.10f);
                Invoke("ataqueLlamaOn", 0.35f);
                CancelInvoke("atacandoTimerReinicia");
                Invoke("atacandoTimerReinicia", atacandoTime);
            }
        }else if(armaEquipada == (int)ArmasJugador.Hojas){
            if(!doblejump && (currentEnergy > 0) && jump){
                doblejump = true;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                atacandoTimer = true;
                currentEnergy -= costeEnergia[(int)ArmasJugador.Hojas];
                hudEnergia.UpdateBar(currentEnergy, maxEnergy);
                animator.SetTrigger("Saltando");
                rb.AddForce(new Vector2(0, speedJump), ForceMode2D.Impulse);
                CancelInvoke("atacandoTimerReinicia");
                Invoke("atacandoTimerReinicia", atacandoTime);
            }
        }else if(armaEquipada == (int)ArmasJugador.Guantes){
            if(!attacking && (currentEnergy > 0) && !crouch && !jump){
                attacking = true;
                atacandoTimer = true;
                currentEnergy -= costeEnergia[(int)ArmasJugador.Guantes];
                hudEnergia.UpdateBar(currentEnergy, maxEnergy);
                animator.SetTrigger("Atacando");
                audioSource[7].PlayDelayed(0.15f);
                Invoke("ataqueGuantesOn", 0.3f);
                CancelInvoke("atacandoTimerReinicia");
                Invoke("atacandoTimerReinicia", atacandoTime);
            }
        }else if(armaEquipada == (int)ArmasJugador.Gafas){
            if(!attacking && (currentEnergy > 0)){
                if(!gafas){
                    atacandoTimer = true;
                    gafas = true;
                    audioSource[8].Play();
                    if (objetosInvisibles){
                        foreach (Transform objetoInvsible in objetosInvisibles.GetComponentInChildren<Transform>()){
                            objetoInvsible.gameObject.SetActive(true);
                        }
                    }
                    if (objetosFalsos){
                        foreach (Transform objetoFalso in objetosFalsos.GetComponentInChildren<Transform>()){
                            objetoFalso.gameObject.SetActive(false);
                        }
                    }
                    panelGafas.SetActive(true);
                    InvokeRepeating("gafasGastoEnergia", 0f, 0.05f);
                    CancelInvoke("atacandoTimerReinicia");
                }else{
                    gafas = false;
                    audioSource[9].Play();
                    if (objetosInvisibles){
                        foreach (Transform objetoInvsible in objetosInvisibles.GetComponentInChildren<Transform>()){
                            objetoInvsible.gameObject.SetActive(false);
                        }
                    }
                    if (objetosFalsos) {
                        foreach (Transform objetoFalso in objetosFalsos.GetComponentInChildren<Transform>()) {
                            objetoFalso.gameObject.SetActive(true);
                        }
                    }
                    panelGafas.SetActive(false);
                    Invoke("atacandoTimerReinicia", atacandoTime);
                    CancelInvoke("gafasGastoEnergia");
                }
            }
        }else if(armaEquipada == (int)ArmasJugador.Sif){
            if((currentEnergy > 0) && !crouch && !jump){
                if(!attacking){
                    attacking = true;
                    atacandoTimer = true;
                    sif = Instantiate(sifPrefab, transform.position, Quaternion.identity);
                    CancelInvoke("atacandoTimerReinicia");
                    InvokeRepeating("sifComprobarVida", 0f, 0.05f);
                    InvokeRepeating("sifGastoEnergia", 0f, 0.05f);
                }else{
                    Destroy(sif);
                    attacking = false;
                    Invoke("atacandoTimerReinicia", atacandoTime);
                    CancelInvoke("sifGastoEnergia");
                    CancelInvoke("sifComprobarVida");
                }
            }
        }else if(armaEquipada == (int)ArmasJugador.Bombas){
            if(!attacking && (currentEnergy > 0) && !crouch && !jump){
                attacking = true;
                atacandoTimer = true;
                currentEnergy -= costeEnergia[(int)ArmasJugador.Bombas];
                hudEnergia.UpdateBar(currentEnergy, maxEnergy);
                animator.SetTrigger("Atacando");
                Invoke("ataqueBombaOn", 0.15f);
                CancelInvoke("atacandoTimerReinicia");
                Invoke("atacandoTimerReinicia", atacandoTime);
            }
        }*/
    ///}
    
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
        /*muerteTexto.SetActive(true);
        audioSource[11].Play();
        mySpriteRenderer.enabled = false;
        bc.enabled = false;
        Invoke("respawn", 5f);*/
    }
    
    private void respawn(){
        /*if(PlayerPrefs.HasKey("Escena")){
            SceneManager.LoadSceneAsync(PlayerPrefs.GetString("Escena"));
        }else{
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }*/
    }

    public void damageOverTime(int dmg) {
        /*if(!dead) {
            currentHealth -= dmg;
            cambioColor(Color.green);
            if(!audioSource[10].isPlaying){
                audioSource[10].Play();
            }
            hudVida.UpdateBar(currentHealth, maxHealth);
            CancelInvoke("ResetTos");
            Invoke("ResetTos", 0.6f);
            CancelInvoke("ResetColor");
            Invoke("ResetColor", 0.1f);
        }*/
        
    }
    
    /*public void damageInstant(int dmg) {
        currentHealth -= dmg;
        ///hudVida.UpdateBar(currentHealth, maxHealth);
    }*/

    public void damageSpeed(float speed, float speedJump){
        ///if (!dañado){
            this.speed = speed;
            this.speedJump = speedJump;
            cambioColor(Color.yellow);
            CancelInvoke("restoreSpeed");
            Invoke("restoreSpeed", 2f);
        ///}
    }

    /*public void damageEnergy(int dmg){
        ///if(!dañado){
            if (currentEnergy > 0){
                currentEnergy -= dmg;
                ///hudEnergia.UpdateBar(currentEnergy, maxEnergy);
            }
            cambioColor(Color.blue);
            CancelInvoke("ResetColor");
            Invoke("ResetColor", 0.5f);
            CancelInvoke("atacandoTimerReinicia");
            ///Invoke("atacandoTimerReinicia", atacandoTime);
        ///}
    }*/

    public void damageEnergyOverTime(int dmg){
        /*if(currentEnergy > 0){
            currentEnergy -= dmg;
            hudEnergia.UpdateBar(currentEnergy, maxEnergy);
        }
        cambioColor(Color.blue);
        if (!audioSource[10].isPlaying){
            audioSource[10].Play();
        }
        CancelInvoke("ResetTos");
        Invoke("ResetTos", 0.6f);
        CancelInvoke("ResetColor");
        Invoke("ResetColor", 0.1f);
        atacandoTimer = true;
        CancelInvoke("atacandoTimerReinicia");
        Invoke("atacandoTimerReinicia", atacandoTime);*/
    }

    public void restoreSpeed(){
        this.speed = currentSpeed;
        this.speedJump = currentSpeedJump;
        ResetColor();
    }

    //Permite recuperar vida
    /*public void recargarVida(int vida) {
        currentHealth += vida;
        if(currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        ///hudVida.UpdateBar(currentHealth, maxHealth);
    }*/
    
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

    //Controlan el proceso de ataque con arco
    /*private void ataqueArcoOn(){
        audioSource[6].PlayDelayed(0.15f);
        Invoke("ataqueArcoWhile", 0.5f);
    }

    private void ataqueArcoWhile(){
        GameObject flechaAux = Instantiate(flechaPrefab, flecha.transform.position, Quaternion.identity);
        if(transform.localScale.x < 0){
            flechaAux.GetComponent<Flecha>().speed = -flechaAux.GetComponent<Flecha>().speed;
            flechaAux.transform.localScale = new Vector3(-flechaAux.transform.localScale.x, flechaAux.transform.localScale.y, flechaAux.transform.localScale.z);
        }
        Invoke("ataqueArcoOff", 0.1f);
    }

    private void ataqueArcoOff(){
        attacking = false;
    }

    //Controlan el proceso de ataque con llama
    private void ataqueLlamaOn(){
        Instantiate(llamaFuego, llamaCollider.transform.position, Quaternion.identity);
        Invoke("ataqueLlamaOff", 0.35f);
    }

    private void ataqueLlamaOff(){
        attacking = false;
    }

    //Controlan el proceso de ataque con gafas
    private void gafasGastoEnergia(){
        if (currentEnergy > 0){
            currentEnergy -= costeEnergia[(int)ArmasJugador.Gafas];
            hudEnergia.UpdateBar(currentEnergy, maxEnergy);
        }else{
            gafas = false;
            audioSource[9].Play();
            if (objetosInvisibles){
                foreach (Transform objetoInvsible in objetosInvisibles.GetComponentInChildren<Transform>()){
                    objetoInvsible.gameObject.SetActive(false);
                }
            }
            if (objetosFalsos){
                foreach (Transform objetoFalso in objetosFalsos.GetComponentInChildren<Transform>()){
                    objetoFalso.gameObject.SetActive(true);
                }
            }
            panelGafas.SetActive(false);
            CancelInvoke("atacandoTimerReinicia");
            Invoke("atacandoTimerReinicia", atacandoTime);
            CancelInvoke("gafasGastoEnergia");
        }
    }

    public bool getGafas(){
        return gafas;
    }

    //Controlan el proceso de Sif
    public GameObject getSif(){
        return sif;
    }

    private void sifComprobarVida(){
        if(!sif){
            attacking = false;
            CancelInvoke("atacandoTimerReinicia");
            Invoke("atacandoTimerReinicia", atacandoTime);
            CancelInvoke("sifGastoEnergia");
            CancelInvoke("sifComprobarVida");
        }
    }

    private void sifGastoEnergia(){
        if(currentEnergy > 0){
            currentEnergy -= costeEnergia[(int)ArmasJugador.Sif];
            hudEnergia.UpdateBar(currentEnergy, maxEnergy);
        }else{
            Destroy(sif);
            attacking = false;
            CancelInvoke("atacandoTimerReinicia");
            Invoke("atacandoTimerReinicia", atacandoTime);
            CancelInvoke("sifComprobarVida");
            CancelInvoke("sifGastoEnergia");
        }
    }

    //Controlan el proceso de ataque con bomba
    private void ataqueBombaOn(){
        GameObject bombaAux =  Instantiate(bombaPrefab, bomba.transform.position, Quaternion.identity);
        if(transform.localScale.x < 0){
            bombaAux.GetComponent<Bomba>().fuerza = new Vector2(-bombaAux.GetComponent<Bomba>().fuerza.x, bombaAux.GetComponent<Bomba>().fuerza.y);
            bombaAux.transform.localScale = new Vector3(-bombaAux.transform.localScale.x, bombaAux.transform.localScale.y, bombaAux.transform.localScale.z);
        }
        Invoke("ataqueBombaOff", 0.5f);
    }

    private void ataqueBombaOff(){
        attacking = false;
    }*/

    //Controlan la recarga de energía
    /*private void recargaEnergia(){

        if(currentEnergy < maxEnergy && !attackingTimer){
            currentEnergy++;
            hudEnergia.UpdateBar(currentEnergy, maxEnergy);
        }
        
    }*/

    //Controla el bloqueo del botón de ataque durante un ataque
    ///private void atacandoTimerReinicia() {
    ///    atacandoTimer = false;
    ///}

    //Objetos
    /*public void obtenerObjeto(string tipoObjeto, int identificador){

        if(tipoObjeto == "arma"){
            obtenerArma(identificador);
        }else if(tipoObjeto == "llave"){
            obtenerLlave(identificador);
        }else if(tipoObjeto == "coleccionable"){
            obtenerColeccionable(identificador);
        }
        
    }*/

    //Permiten ver y modificar las armas actuales
   /* public bool[] getArmas() {
        return armas;
    }

    public void obtenerArma(int arma){
        armas[arma] = true;
    }

    //LLaves
    public bool[] getLlaves(){
        return llaves;
    }

    public void obtenerLlave(int llave){
        llaves[llave] = true;
    }*/

    //Coleccionables
   /* private void crearColeccionables(){
        coleccionables.Add("Piedras", 0);
        coleccionables.Add("Monedas", 0);
        coleccionables.Add("Flores", 0);
        coleccionables.Add("Tomos", 0);
    }

    public Dictionary<string, int> getColeccionables(){
        return coleccionables;
    }

    public void obtenerColeccionable(int coleccionable){

        if(coleccionable == 0){
            coleccionables["Piedras"]++;
        }else if(coleccionable == 1){
            coleccionables["Monedas"]++;
        }else if(coleccionable == 2){
            coleccionables["Flores"]++;
        }else if(coleccionable == 3){
            coleccionables["Tomos"]++;
        }
        
    }*/

    private void cambioColor(Color color) {
        mySpriteRenderer.color = color;
    }

    private void ResetColor() {
        mySpriteRenderer.color = Color.white;
    }

    private void ResetTos(){
        ///audioSource[10].Stop();
    }

    //Controla el tiempo de "mercy invincibility"
    /*private void SpriteBlinkingEffect(){
        if(!dead) {
            spriteBlinkingTotalTimer += Time.deltaTime;

            if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration){
                dañado = false;
                spriteBlinkingTotalTimer = 0.0f;
                mySpriteRenderer.enabled = true;
                return;
            }

            spriteBlinkingTimer += Time.deltaTime;

            if (spriteBlinkingTimer >= spriteBlinkingMiniDuration){
                spriteBlinkingTimer = 0.0f;
                if (mySpriteRenderer.enabled == true)
                {
                    mySpriteRenderer.enabled = false;
                }else{
                    mySpriteRenderer.enabled = true;
                }
            }
        }
    }*/
    
    //Controla si estas en una plataforma
    /*public void StandOnPlatform(Rigidbody2D platform) {
        this.platform = platform;
    }
    
    public void ExitPlatform(Rigidbody2D platform) {
        if(this.platform == platform) {
            this.platform = null;
        }
    }*/

    //Comprueba si hay una partida guardada
    /*private void cargarPartida(){

        if (PlayerPrefs.HasKey("Escena")){
            transform.position = new Vector3(PlayerPrefs.GetFloat("Jugador_x"), PlayerPrefs.GetFloat("Jugador_y"), 0);
            for (int arma = 0; arma < armas.Length; arma++){
                if (PlayerPrefs.GetInt("Arma" + arma) == 1){
                    armas[arma] = true;
                }else{
                    armas[arma] = false;
                }
            }
            for (int llave = 0; llave < llaves.Length; llave++){
                if (PlayerPrefs.GetInt("Llave" + llave) == 1){
                    llaves[llave] = true;
                }else{
                    llaves[llave] = false;
                }
            }
            Dictionary<string, int> coleccionablesAux = new Dictionary<string, int> (coleccionables);
            foreach (KeyValuePair<string, int> coleccionable in coleccionablesAux){
                coleccionables[coleccionable.Key] =  PlayerPrefs.GetInt(coleccionable.Key);
            }
        }

    }*/

    //Modo dios
    /*private void Cheat(){
        //currentHealth = 10000;
        //currentEnergy = 10000;
        for (int i = 0; i < armas.Length; i++){
            armas[i] = true;
        }
        for (int i = 0; i < llaves.Length; i++){
            llaves[i] = true;
        }
    }*/

}
