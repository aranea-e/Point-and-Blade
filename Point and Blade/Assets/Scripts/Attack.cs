using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack {
    [SerializeField] public string name;

    [SerializeField] string trigger;

    [SerializeField] public int damage;
    [SerializeField] Vector2 knockback;
    [SerializeField] float stun;

    public void hit(Controller reciever) {
        reciever.applyHitstun(stun);
        if (name == "Weak Trip Attack") {
            reciever.applyHitstun(stun / reciever.other.tripCounter);
            if (stun / reciever.other.tripCounter < 1 / 16.0)
            {
                reciever.applyHitstun(0f);
            }
        }
        reciever.applyKnockback(knockback);
        reciever.spcd = 3.0f;
        if (reciever.playerNumber == 0) {
            if (reciever.ui.getVal(0) - damage <= 0) {
                reciever.audioManager.play("Kill");
            } else {
                if (name == "Block") {
                    reciever.audioManager.play("Block");
                } else {
                    reciever.audioManager.play("Hit");
                }
            }
            reciever.ui.addVal(0, -damage);
        } else {
            if (reciever.ui.getVal(1) - damage <= 0) {
                reciever.audioManager.play("Kill");
            } else {
                if (name == "Block") {
                    reciever.audioManager.play("Block");
                } else {
                    reciever.audioManager.play("Hit");
                }
            }
            reciever.ui.addVal(1, -damage);
        }
        reciever.actionable = false;
        reciever.anim.SetTrigger("Interrupt");
        reciever.anim.SetBool("Blocking", false);
        reciever.block = false;

        if (reciever.playerNumber == 0 && reciever.ui.getVal(0) != 100f) {
            reciever.ui.addVal(5, damage * 2);
        } else if (reciever.ui.getVal(1) != 100f) {
            reciever.ui.addVal(4, damage * 2);
        }
    }

    public void attack(Controller attacker) {
        attacker.anim.SetTrigger(trigger);
        attacker.currentAttack = this;
        if (attacker.tripcd > 0.0f && attacker.tripcd < stun && attacker.tripcd > 0f) {
            attacker.tripcd = stun + .2f;
        }
        if (attacker.tripcd < 1f && attacker.tripcd > 0f) {
            attacker.tripcd = 1f;
        }
    }
}
