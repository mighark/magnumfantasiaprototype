using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour {
    public Skill[] skills = new Skill[6];
    private float[] skillCooldowns;
    
    private bool[] frozen;
    public Text[] cooldownText;
    
    private Vida vida;

	// Use this for initialization
	void Start () {
		skillCooldowns = new float[skills.Length];
        
        for(int i = 0; i < skillCooldowns.Length; i++) {
            skillCooldowns[i] = 0;
        }
        
        frozen = new bool[skills.Length];
        for(int i = 0; i < frozen.Length; i++) {
            frozen[i] = false;
        }
        
        vida = GetComponent<Vida>();
	}
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < skillCooldowns.Length; i++) {
            if(skillCooldowns[i] > 0 && !frozen[i]) {
                skillCooldowns[i] -= Time.deltaTime;
            }
        }
		updateCooldowns();
	}
    
        
    void updateCooldowns() {
        string[] defaultText = {"Q", "1", "2", "3", "4", "5"};
        for(int i = 0; i < skillCooldowns.Length; i++) {
            if(frozen[i]) {
                cooldownText[i].text = defaultText[i];
                cooldownText[i].color = Color.blue;
            } else if(skillCooldowns[i] > 0) {
                cooldownText[i].text = Mathf.CeilToInt(skillCooldowns[i]).ToString();
                cooldownText[i].color = Color.blue;
            } else {
                cooldownText[i].text = defaultText[i];
                cooldownText[i].color = Color.red;
            }
        }
    }
    
    public void freezeSkill(string skillName) {
        for(int i = 0; i < skills.Length; i++) {
            if(skills[i].skillName == skillName) {
                frozen[i] = true;
                break;
            }
        }
    }
    
    public void unfreezeSkill(string skillName) {
        for(int i = 0; i < skills.Length; i++) {
            if(skills[i].skillName == skillName) {
                frozen[i] = false;
                break;
            }
        }
    }
    
    public bool canCastSkill(int slot) {
        return (skills[slot].canUseSkill(vida) && skillCooldowns[slot] <= 0);
    }
    
    public void cooldownSkill(int slot) {
        skillCooldowns[slot] = skills[slot].cooldown;
    }
}
