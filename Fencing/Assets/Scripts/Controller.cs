using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] Controller other;
    
    Rigidbody2D rb;
    public Transform trans;
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] BoxCollider2D foot;

    [SerializeField] KeyCode l;
    [SerializeField] KeyCode r;
    [SerializeField] KeyCode j;
    [SerializeField] PhysicsMaterial2D[] materials;

    bool moveleft;
    bool moveright;
    bool jump;
    bool footstool;
    bool slip;      
    float fscd;
    float spcd;
    public bool actionable;
    public float lag;
    public float hitstun;
    public float attackTime;
    public float startupTime;
    [SerializeField] float moveMod;
    [SerializeField] float jumpMod;
    [SerializeField] LayerMask fslm;
    [SerializeField] LayerMask splm;
    [SerializeField] Attack[] attackList;
    static Attack[] Attacks;
    
    void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        trans = gameObject.GetComponent<Transform>();
        if (attackList != null) { Attacks = attackList; }
    }

    void Update() {
        moveleft = Input.GetKey(l);
        moveright = Input.GetKey(r);
        jump = Input.GetKeyDown(j) && IsGrounded();
        footstool = Input.GetKeyDown(j) && foot.IsTouchingLayers(fslm);
        slip = foot.IsTouchingLayers(splm) && rb.velocity.y < 0 && spcd <= 0f;

        if (other.trans.position.x > trans.position.x) {
            trans.localScale = new Vector3(1f, 1f, 1f);
        } else {
            trans.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (hitstun > 0.0f || lag > 0.0f) {
            actionable = false;
        } 

        if (actionable && !slip) {
            rb.sharedMaterial = materials[0];
            playerCollider.sharedMaterial = materials[0];

            Vector3 final = Vector3.zero;
            final.y = rb.velocity.y;
            if (moveleft) { final.x -= moveMod; }
            if (moveright) { final.x += moveMod; }
            if (jump) { final.y += jumpMod; }
            if (footstool && fscd <= 0) { 
                fscd = 2f; 
                final.y += jumpMod / 2f; 
                other.applyHitstun(2f); 
                final.x = 0;
                rb.velocity = Vector3.down;
            }
            rb.velocity = final;
        } else if (slip) {
            Vector3 final = Vector3.zero;
            applyHitstun(2f);
            spcd = .1f;
            final.y += jumpMod / 2f;
            if (other.trans.position.x > trans.position.x) {
                final.x = -moveMod / 2;
            } else {
                final.x = moveMod / 2;
            }
            rb.velocity = final;
        } else if (lag <= 0) {
            rb.sharedMaterial = materials[1];
            playerCollider.sharedMaterial = materials[1];

            hitstun -= Time.deltaTime;
            if (hitstun <= 0.0f) {
                actionable = true;
            }
        } else {
            if (startupTime > 0) {
                startupTime -= Time.deltaTime;
            }
            if (attackTime > 0) {
                attackTime -= Time.deltaTime;
            }
            lag -= Time.deltaTime;
        }

        if (fscd > 0) {
            fscd -= Time.deltaTime;
        }
        if (spcd > 0) {
            spcd -= Time.deltaTime;
        }
    }

    public bool IsGrounded() {
        return (trans.position.y < 1.1f);
    }

    public void applyHitstun(float hitstun) {
        this.hitstun = hitstun;
        actionable = false;
    }

    public void applyKnockback(Vector2 knockback) {
        rb.velocity += knockback;
    }
}
