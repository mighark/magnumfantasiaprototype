using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {
    public GameObject skillPrefab;
    public string skillName;
    public int manaCost = 100;
    public float cooldown = 10f;
    public float castTime = 0;
    public string castAnim = "Casting";

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	}
    
    public void attemptSkill(Vida vida) {
        useSkill(vida);
    }
    
    virtual protected void useSkill(Vida vida) {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject skillObject = Instantiate(skillPrefab, player.transform.position, Quaternion.identity);
        skillObject.GetComponent<Weapon>().user = vida;
        Moving moving = skillObject.GetComponent<Moving>();
        if(moving) {
            moving.setMoveDir(vida.transform.localScale.x);
        }
    }
    
    virtual public bool canUseSkill(Vida vida) {
        return true;
    }
}
