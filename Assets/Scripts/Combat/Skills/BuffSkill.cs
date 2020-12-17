using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSkill : Skill {
    public Buff buff;
    
    override protected void useSkill(Vida vida) {
        vida.buffs.addBuff(buff);
    }
    
    override public bool canUseSkill(Vida vida) {
        return !vida.buffs.hasBuff(buff.ID);
    }
}
