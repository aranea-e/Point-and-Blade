using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    [SerializeField] string name;
    
    [SerializeField] float startup;
    [SerializeField] float active;
    [SerializeField] float lag;

    [SerializeField] float damage;
    [SerializeField] Vector2 knockback;
    [SerializeField] float stun;

    public void hit(Controller reciever) {
        reciever.applyHitstun(stun);
        reciever.applyKnockback(knockback);
    }

    public void attack(Controller attacker) {
        attacker.lag = lag;
        attacker.attackTime = active;
        attacker.startupTime = startup;
    }
}
