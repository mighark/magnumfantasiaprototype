using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Buff {
    public string ID;
    public float duration;
    public bool isNegative;
    
    virtual public void onStartAttackEffect(Vida user) {
        
    }
}

public class BuffInstance {
    public Buff buff;
    public float durationLeft;
    
    public BuffInstance(Buff buffToApply) {
        buff = buffToApply;
        durationLeft = buff.duration;
    }
}

public class BuffManager {
    private Dictionary<string, BuffInstance> tempBuffs;
    private Dictionary<string, BuffInstance> permBuffs;
    
    public BuffManager() {
        tempBuffs = new Dictionary<string, BuffInstance>();
        permBuffs = new Dictionary<string, BuffInstance>();
    }
    
    public bool hasBuff(string buffID) {
        return hasTempBuff(buffID) || hasPermBuff(buffID);
    }
    
    public bool hasTempBuff(string buffID) {
        return tempBuffs.ContainsKey(buffID);
    }
    
    public bool hasPermBuff(string buffID) {
        return permBuffs.ContainsKey(buffID);
    }
    
    public void addBuff(Buff buff) {
        if(buff.duration <= 0) {
            if(!hasPermBuff(buff.ID)) {
                permBuffs.Add(buff.ID, new BuffInstance(buff));
            }
        } else {
            if(!hasTempBuff(buff.ID)) {
                tempBuffs.Add(buff.ID, new BuffInstance(buff));
            }
        }
        
    }
    
    public void removeBuff(string buffID) {
        removeTempBuff(buffID);
        removePermBuff(buffID);
    }
    
    public void removeTempBuff(string buffID) {
        tempBuffs.Remove(buffID);
    }
    
    public void removePermBuff(string buffID) {
        permBuffs.Remove(buffID);
    }
    
    public void updateBuffs(float deltaTime) {
        foreach(KeyValuePair<string, BuffInstance> entry in tempBuffs) {
            BuffInstance buff = entry.Value;
            buff.durationLeft -= deltaTime;
            if(buff.durationLeft <= 0) {
                removeTempBuff(entry.Key);
            }
        }
    }
    
    public void onStartAttackEffect(Vida user) {
        List<BuffInstance> buffsCopy = new List<BuffInstance>(tempBuffs.Values);
        foreach(BuffInstance buff in buffsCopy) {
            Buff buffInfo = buff.buff;
            buffInfo.onStartAttackEffect(user);
        }
        buffsCopy = new List<BuffInstance>(permBuffs.Values);
        foreach(BuffInstance buff in buffsCopy) {
            Buff buffInfo = buff.buff;
            buffInfo.onStartAttackEffect(user);
        }
    }
}
