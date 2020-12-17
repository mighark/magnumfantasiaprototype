using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy {
    public HealthBar healthBar;
    public StageFinish onDeathEvent;

	virtual public void startBoss() {
        
    }
    
    virtual public void activateDeathEvent() {
        onDeathEvent.bossDead();
    }
}
