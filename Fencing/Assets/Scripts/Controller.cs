using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] Controller other;
    
    Rigidbody2D rb;
    Transform trans;
    BoxCollider2D playerCollider;

    [SerializeField] KeyCode l;
    [SerializeField] KeyCode r;
    [SerializeField] KeyCode j;

    bool moveleft;
    bool moveright;
    bool jump;
    bool grounded;
    public bool actionable;
    public bool lag;
    public float hitstun;
    RaycastHit hit;
    [SerializeField] float moveMod;
    [SerializeField] float jumpMod;
    
    void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        trans = gameObject.GetComponent<Transform>();
        playerCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update() {
        moveleft = Input.GetKey(l);
        moveright = Input.GetKey(r);
        jump = Input.GetKeyDown(j) && IsGrounded();

        if (hitstun > 0.0f || lag) {
            actionable = false;
        } 

        if (actionable) {
            Vector3 final  = Vector3.zero;
            final.y = rb.velocity.y;
            if (moveleft) { final.x -= moveMod; }
            if (moveright) { final.x += moveMod; }
            if (jump) { final.y += jumpMod; }
            rb.velocity = final;
        } else if (lag == false) {
            hitstun -= Time.deltaTime;
            if (hitstun <= 0.0f) {
                actionable = true;
            }
        }
    }

    public bool IsGrounded() {
        return (trans.position.y < 1.1f);
    }

    public void applyHitstun(float hitstun) {
        this.hitstun = hitstun;
        actionable = false;
    }
}
