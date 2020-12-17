using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSpearBuff : Buff {
    public GameObject flames;
    public Vector3 offset;
    public Vector3 rotation;
    public string skillName;
    
    public FlameSpearBuff(Buff buffInfo, GameObject flames, Vector3 offset, Vector3 rotation, string skillName) {
        this.ID = buffInfo.ID;
        this.duration = buffInfo.duration;
        this.isNegative = buffInfo.isNegative;
        this.flames = flames;
        this.offset = offset;
        this.rotation = rotation;
        this.skillName = skillName;
    }
    
    override public void onStartAttackEffect(Vida user) {
        Vector3 position = user.transform.position - new Vector3(offset.x * user.transform.localScale.x, offset.y, offset.z);
        Vector3 adjRotation = rotation * user.transform.localScale.x;
        GameObject flameSpear = MonoBehaviour.Instantiate(flames, position, Quaternion.Euler(adjRotation));
        Weapon flameWeapon = flameSpear.GetComponent<Weapon>();
        flameWeapon.setUser(user);
        user.buffs.removePermBuff(ID);
        SkillManager sm = user.GetComponent<SkillManager>();
        if(sm) {
            sm.unfreezeSkill(skillName);
        }
    }
}

public class FlameSpear : BuffSkill {
    public Vector3 offset;
    public Vector3 rotation;
    
    override protected void useSkill(Vida vida) {
        vida.buffs.addBuff(new FlameSpearBuff(buff, skillPrefab, offset, rotation, skillName));
        SkillManager sm = vida.GetComponent<SkillManager>();
        if(sm) {
            sm.freezeSkill(skillName);
        }
    }
}
