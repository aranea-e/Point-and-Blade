using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] Controller other;
    
    Rigidbody2D rb;
    public Transform trans;
    public Animator anim;
    [SerializeField] BoxCollider2D bladeCollider;
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] BoxCollider2D foot;
    [SerializeField] BoxCollider2D hitbox;

    [SerializeField] KeyCode l;
    [SerializeField] KeyCode r;
    [SerializeField] KeyCode j;
    [SerializeField] KeyCode d;
    [SerializeField] KeyCode a;
    [SerializeField] KeyCode b;
    [SerializeField] PhysicsMaterial2D[] materials;

    bool moveLeft;
    bool moveRight;
    bool inputDown;
    bool jump;
    bool inputAttack;
    bool inputBlock;
    bool footstool;
    bool slip; 
    bool hit;     
    float fscd;
    float spcd;
    float tripcd;
    public bool actionable;
    public float hitstun;
    [SerializeField] float moveMod;
    [SerializeField] float jumpMod;
    [SerializeField] LayerMask fslm;
    [SerializeField] LayerMask splm;
    [SerializeField] LayerMask hurt;
    [SerializeField] Attack[] attackList;
    static Attack[] Attacks;
    public Attack currentAttack;
    
    void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        trans = gameObject.GetComponent<Transform>();
        if (attackList != null) { Attacks = attackList; }
    }

    void Update() {
        moveLeft = Input.GetKey(l);
        moveRight = Input.GetKey(r);
        inputDown = Input.GetKey(d);
        jump = Input.GetKeyDown(j) && IsGrounded();
        inputAttack = Input.GetKeyDown(a);
        inputBlock = Input.GetKey(b);
        footstool = Input.GetKeyDown(j) && foot.IsTouchingLayers(fslm);
        slip = foot.IsTouchingLayers(splm) && rb.velocity.y < 0;

        if (hit) {
            bladeCollider.enabled = false;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            hit = false;
        }

        if (hitbox.IsTouchingLayers(hurt) && !other.hit) {
            other.currentAttack.hit(this);
            other.hit = true;
            actionable = false;
        }

        if (tripcd > 0f) {
            tripcd -= Time.deltaTime;
        }

        if (other.trans.position.x > trans.position.x) {
            trans.localScale = new Vector3(1f, 1f, 1f);
        } else {
            trans.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (hitstun > 0.0f || !(anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) || inputBlock) {
            actionable = false;
        } 

        if (actionable && inputAttack) {
            if (IsGrounded()) {
                if (!(moveLeft ^ moveRight)) {
                    if (inputDown) {
                        if (tripcd > 0)
                        {
                            Attacks[5].attack(this);
                            tripcd = .5f;
                        } else {
                            Attacks[4].attack(this);
                            tripcd = 2.0f;
                        }
                    } else {
                        Attacks[0].attack(this);
                    }
                }
                
                if ((moveRight && trans.localScale.x == 1) || (moveLeft && trans.localScale.x == -1)) {
                    Attacks[1].attack(this);
                    rb.AddForce(new Vector3(trans.localScale.x * 250, 0, 0));
                }
                if ((moveRight && trans.localScale.x == -1) || (moveLeft && trans.localScale.x == 1))
                {
                    Attacks[2].attack(this);
                }
            } else {
                Attacks[3].attack(this);
            }
            
            actionable = false;
        }

        if (actionable && !slip) {
            rb.sharedMaterial = materials[0];
            playerCollider.sharedMaterial = materials[0];

            Vector3 final = Vector3.zero;
            final.y = rb.velocity.y;
            if (IsGrounded() || footstool) {
                if (moveLeft) { final.x -= moveMod; }
                if (moveRight) { final.x += moveMod; }
                if (jump) { final.y += jumpMod; }
            } else {
                if (!footstool) {
                    final.x = rb.velocity.x;
                }
            }
            if (footstool && fscd <= 0) { 
                fscd = 2f; 
                spcd = 0f;
                final.y += jumpMod / 2f; 
                other.applyHitstun(2f); 
            }
            rb.velocity = final;
        } else if (slip) {
            Vector3 final = Vector3.zero;
            if (spcd <= 0f) {
                applyHitstun(2f);
            }
            spcd = 3f;
            final.y += jumpMod / 2f;
            if (other.trans.position.x > trans.position.x) {
                final.x = -moveMod / 2;
            } else {
                final.x = moveMod / 2;
            }
            rb.velocity = final;
        } else {
            rb.sharedMaterial = materials[1];
            playerCollider.sharedMaterial = materials[1];

            hitstun -= Time.deltaTime;
            if (hitstun <= 0.0f) {
                actionable = true;
            }
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
        rb.velocity = new Vector3(knockback.x * other.trans.localScale.x, knockback.y);
    }

    public bool bladeActive() {
        return bladeCollider.enabled;
    }
}
