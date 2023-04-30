using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    [SerializeField] public string name;

    [SerializeField] string trigger;

    [SerializeField] public int damage;
    [SerializeField] Vector2 knockback;
    [SerializeField] float stun;

    public void hit(Controller reciever) {
        reciever.applyHitstun(stun);
        reciever.applyKnockback(knockback);
        if (reciever.playerNumber == 0)
        {
            reciever.ui.addVal(0, -damage);
        } else {
            reciever.ui.addVal(1, -damage);
        }
        reciever.actionable = false;
        reciever.anim.SetTrigger("Interrupt");
        reciever.anim.SetBool("Blocking", false);
        reciever.block = false;

        if (reciever.playerNumber == 0)
        {
            reciever.ui.addVal(5, damage * 2);
        } else {
            reciever.ui.addVal(4, damage * 2);
        }
    }

    public void attack(Controller attacker) {
        attacker.anim.SetTrigger(trigger);
        attacker.currentAttack = this;
        if (attacker.tripcd > 0.0f) {
            attacker.tripcd = stun + .2f;
        }
    }
}
