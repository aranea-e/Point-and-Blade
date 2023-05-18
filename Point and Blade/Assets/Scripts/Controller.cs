using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {
    public Controller other;
    public UI ui;
    public int playerNumber;
    Controls controls;
    [SerializeField] Starter starter;
    public AudioManager audioManager;
    Computer computerController;
    
    public Rigidbody2D rb;
    public Transform trans;
    public Animator anim;
    [SerializeField] BoxCollider2D bladeCollider;
    public BoxCollider2D playerCollider;
    [SerializeField] BoxCollider2D foot;
    [SerializeField] BoxCollider2D hitbox;
    [SerializeField] BoxCollider2D footstoolHitbox;

    [SerializeField] KeyCode l;
    [SerializeField] KeyCode r;
    [SerializeField] KeyCode j;
    [SerializeField] KeyCode d;
    [SerializeField] KeyCode a;
    [SerializeField] KeyCode b;
    [SerializeField] KeyCode menu;
    [SerializeField] PhysicsMaterial2D[] materials;

    bool inputLeft;
    bool inputRight;
    bool inputDown;
    bool inputAttack;
    bool aHeld;
    bool inputBlock;
    bool inputJump;
    public bool jHeld;

    bool jump;
    public float jAttackWindow;
    bool buffered;
    public bool block;
    bool footstool;
    bool slip; 
    [SerializeField] bool hit;     
    float fscd;
    public float spcd;
    public float tripcd;
    public int tripCounter;
    [SerializeField] SpriteRenderer tripCounterSprite;
    [SerializeField] Sprite[] tripCounterSpriteSprites;
    public bool actionable;
    public bool starting;
    public float hitstun;
    public float moveMod;
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
        controls = FindObjectOfType<Controls>();
        audioManager = FindObjectOfType<AudioManager>();
        computerController = FindObjectOfType<Computer>();
        if (attackList != null) { Attacks = attackList; }

        l = controls.controls[0 + playerNumber * 6];
        d = controls.controls[1 + playerNumber * 6];
        r = controls.controls[2 + playerNumber * 6];
        b = controls.controls[3 + playerNumber * 6];
        j = controls.controls[4 + playerNumber * 6];
        a = controls.controls[5 + playerNumber * 6];
    }

    void Update() {
        if (Input.GetKey(menu) || 
            (controls.P1controller && controls.playerControlManagers[0].menu) ||
            (controls.P2controller && controls.playerControlManagers[1].menu)) {
            Destroy(controls.gameObject);
            SceneManager.LoadScene(1);
        }
        if (playerNumber == 1 && controls.com)
        {
            computerController.computerUpdate();
            inputLeft = computerController.inputLeft;
            inputRight = computerController.inputRight;
            inputDown = computerController.inputDown;
            inputBlock = computerController.inputBlock;
            inputAttack = computerController.inputAttack;
            inputJump = computerController.inputJump && !jHeld;
            aHeld = computerController.inputAttack && ((!aHeld) || buffered);
            jHeld = computerController.inputJump;
        } else if ((controls.P1controller && playerNumber == 0) || (controls.P2controller && playerNumber == 1))
        {
            inputLeft = controls.playerControlManagers[playerNumber].left;
            inputRight = controls.playerControlManagers[playerNumber].right;
            inputDown = controls.playerControlManagers[playerNumber].down;
            inputBlock = controls.playerControlManagers[playerNumber].block;
            inputAttack = controls.playerControlManagers[playerNumber].attack && ((!aHeld) || buffered);
            inputJump = controls.playerControlManagers[playerNumber].up && !jHeld;
            aHeld = controls.playerControlManagers[playerNumber].attack;
            jHeld = controls.playerControlManagers[playerNumber].up;
        } else {
            inputLeft = Input.GetKey(l);
            inputRight = Input.GetKey(r);
            inputDown = Input.GetKey(d);
            inputBlock = Input.GetKey(b);
            inputAttack = Input.GetKeyDown(a) || (Input.GetKey(a) && buffered);
            inputJump = Input.GetKeyDown(j);
        }
        
        jump = inputJump && IsGrounded();
        if (!Input.GetKey(a) || inputAttack) {
            buffered = false;
        }
        footstool = inputJump && foot.IsTouchingLayers(fslm);
        slip = foot.IsTouchingLayers(splm) && rb.velocity.y < 0;
        if (slip && Mathf.Abs(trans.position.x - other.trans.position.x) > .51)
        {
            slip = false;
            trans.position = new Vector3(other.trans.position.x - .52f * trans.localScale.x, trans.position.y, 0);
        }
        if (hit || hitstun > 0f) {
            bladeCollider.enabled = false;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Block")) {
            hit = false;
        }

        if (hitbox.IsTouchingLayers(hurt) && !other.hit && other.bladeCollider.enabled
                && other.currentAttack.name != "Block"
                && !(block && other.currentAttack.name != "Trip Attack" 
                && other.currentAttack.name != "Weak Trip Attack")) {
            other.currentAttack.hit(this);
            other.hit = true;
            actionable = false;
        }

        if (block && (bladeCollider.IsTouchingLayers(hurt) || hitbox.IsTouchingLayers(hurt))
                && other.currentAttack.name != "Block") {
            ui.addVal(2 + playerNumber, -50);
            if (ui.getVal(2 + playerNumber) <= 0) {
                ui.setVal(2 + playerNumber, -50);
                audioManager.play("NoBlock");
            }
            ui.addVal(4 + playerNumber, other.currentAttack.damage * 3);
            currentAttack.hit(other);
        }

        if (hitstun > 0.0f && tripcd > 0f) {
            tripcd = 0f;
            tripCounterSprite.enabled = false;
            tripCounter = 0;
        } else if (tripcd > 0f) {
            tripcd -= Time.deltaTime;
            tripCounterSprite.enabled = true;
            if (tripCounter < 4)
            {
                tripCounterSprite.sprite = tripCounterSpriteSprites[tripCounter];
            } else {
                tripCounterSprite.sprite = tripCounterSpriteSprites[3];
            }
        } else {
            tripCounterSprite.enabled = false;
        }

        if (other.trans.position.x > trans.position.x) {
            trans.localScale = new Vector3(1f, 1f, 1f);
        } else {
            trans.localScale = new Vector3(-1f, 1f, 1f);
        }

        footstoolHitbox.enabled = true;
        if (hitstun > 0.0f || !(anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) 
                || (!starter.firstTime && starter.timer > 1.5f)) {
            actionable = false;
            if (inputAttack) {
                buffered = true;
            }
            if (hitstun > 0.0f)
            {
                footstoolHitbox.enabled = false;
            }
        } 
        
        if (inputBlock && hitstun <= 0.0f && ui.getVal(2 + playerNumber) > 0 
                && (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") 
                || anim.GetCurrentAnimatorStateInfo(0).IsName("Block")))
        {
            anim.SetBool("Blocking", true);
            block = true;
            currentAttack = Attacks[6];
            actionable = false;
            ui.addVal(2 + playerNumber, -Time.deltaTime * 30f);
            if (ui.getVal(2 + playerNumber) <= 0) {
                ui.setVal(2 + playerNumber, -25);
                audioManager.play("NoBlock");
            }
            
        } else {
            anim.SetBool("Blocking", false);
            block = false;
            ui.addVal(2 + playerNumber, Time.deltaTime * 10f);
        }

        if (ui.getVal(4 + playerNumber) == 100 && block && hitstun <= 0.0f) {
            ui.setVal(4 + playerNumber, 0);
            ui.addVal(playerNumber, 33);
            ui.setVal(2 + playerNumber, 100);
            audioManager.play("Adren");
        }

        if (actionable && inputAttack) {
            if (IsGrounded() || jAttackWindow > 0.0f) {
                if (!(inputLeft ^ inputRight) && !inputDown) {
                    
                    Attacks[0].attack(this);
                }
                
                if (inputDown) {
                    if (tripcd > 0)
                    {
                        tripCounter++;
                        Attacks[5].attack(this);
                    } else {
                        Attacks[4].attack(this);
                        tripcd = 2.2f;
                        fscd = 2.0f;
                        tripCounter = 0;
                    }
                    Vector3 final = new Vector3(0, rb.velocity.y, 0);
                    if (inputLeft) { final.x -= moveMod; }
                    if (inputRight) { final.x += moveMod; }
                    rb.velocity = final;
                } else if ((inputRight && trans.localScale.x == 1) || (inputLeft && trans.localScale.x == -1)) {
                    Attacks[1].attack(this);
                    rb.velocity = new Vector3(moveMod * trans.localScale.x, rb.velocity.y, 0);
                    rb.AddForce(new Vector3(trans.localScale.x * 250, 0, 0));
                }
                if ((inputRight && trans.localScale.x == -1) || (inputLeft && trans.localScale.x == 1))
                {
                    Attacks[2].attack(this);
                }
            } else {
                Attacks[3].attack(this);
            }
            
            actionable = false;
        }
        
        if (jAttackWindow > 0.0f)
        {
            jAttackWindow -= Time.deltaTime;
        }

        if (actionable && !slip && !starting) {
            rb.sharedMaterial = materials[0];
            playerCollider.sharedMaterial = materials[0];

            Vector3 final = Vector3.zero;
            final.y = rb.velocity.y;
            
            if (inputLeft) { final.x -= moveMod; }
            if (inputRight) { final.x += moveMod; }
            if (jump) { final.y = jumpMod; audioManager.play("Jump"); jAttackWindow = 0.05f; }
            
            if (footstool && fscd <= 0) { 
                fscd = 2f; 
                spcd = 0f;
                final.y += jumpMod / 2f; 
                other.applyHitstun(2f); 
                audioManager.play("Footstool");
            }
            rb.velocity = final;
        } else if (slip) {
            Vector3 final = Vector3.zero;
            audioManager.play("Slip");
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
            anim.SetTrigger("Interrupt");
        } else {
            rb.sharedMaterial = materials[1];
            playerCollider.sharedMaterial = materials[1];
            if (hitstun > 0.0f) {
                hitstun -= Time.deltaTime;
            }
            if (hitstun <= 0.0f) {
                actionable = true;
            }
        }

        if (fscd > 0) {
            fscd -= Time.deltaTime;
        }
        if (spcd > 0) {
            if (IsGrounded() && hitstun <= 0) {
                spcd = 0;
            }
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
