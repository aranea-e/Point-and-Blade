using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    [SerializeField] string name;

    [SerializeField] string trigger;

    [SerializeField] float damage;
    [SerializeField] Vector2 knockback;
    [SerializeField] float stun;

    public void hit(Controller reciever) {
        reciever.applyHitstun(stun);
        reciever.applyKnockback(knockback);
    }

    public void attack(Controller attacker) {
        attacker.anim.SetTrigger(trigger);
        attacker.currentAttack = this;
    }
}
