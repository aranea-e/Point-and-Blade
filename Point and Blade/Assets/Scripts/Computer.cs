using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour {
    public Controller player;
    Controls controls;
    
    public bool inputLeft;
    public bool inputRight;
    public bool inputDown;
    public bool inputAttack;
    public bool inputBlock;
    public bool inputJump;

    public bool inputForward;
    public bool inputBackward;

    [SerializeField] int state;
    [SerializeField] float stateTimer;
    [SerializeField] float actionTimer;
    [SerializeField] bool moveForward;
    [SerializeField] bool move;
    [SerializeField] float moveDirectionTimer;
    [SerializeField] bool defend;
    [SerializeField] float defendTimer;
    [SerializeField] float lungeReaction;
    [SerializeField] bool lungeReacted;

    void Start() {
        controls = FindObjectOfType<Controls>();
        if (!controls.com)
        {
            Destroy(this.gameObject);
        }
        state = 0;
    }

    
    public void computerUpdate() {
        ResetInputs();
        DetermineState();
        if (state == 0) {
            Neutral();
        }
        if (state == 1) {
            Advantage();
        }
        if (state == -1) {
            Disadvantage();
        }
        ForwardBackLeftRight();
        footstoolAttempt();      
    }

    void ResetInputs() {
        inputLeft = false;
        inputRight = false;
        inputDown = false;
        inputAttack = false;
        inputBlock = false;
        inputJump = false;
        inputForward = false;
        inputBackward = false;
    }

    void DetermineState() {
        if (stateTimer > 0.0f)
        {
            stateTimer -= Time.deltaTime;
        } else {
            state = 0;
        }
        if (player.other.hitstun > 0.0f) {
            state = 1;
            stateTimer = Random.Range(.25f, .5f);
        }
        if (player.hitstun > 0.0f) {
            state = -1;
            stateTimer = Random.Range(.25f, .75f);
        }
    }

    void Neutral() {
        if (!(player.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || 
                player.block))
        {
            actionTimer = Random.Range(0.05f, 0.15f);
        }

        moveDirectionTimer -= Time.deltaTime;
        if (moveDirectionTimer <= 0.0f) {
            chooseDirection();
        }
        inputForward = moveForward && move;
        inputBackward = !moveForward && move;
        inputJump = Random.Range(0f, 1f) < 0.0025f 
                        && Vector3.Distance(player.trans.position, player.other.trans.position) > 1.5f;

        if (player.other.anim.GetCurrentAnimatorStateInfo(0).IsName("Lunging Strike") && Vector3.Distance(player.trans.position, player.other.trans.position) <= 6f)
        {
            if (!lungeReacted)
            {
                lungeReacted = true;
                lungeReaction = Random.Range(0.05f, 0.1f);
            }
            if (lungeReaction > 0f)
            {
                lungeReaction -= Time.deltaTime;
            } else {
                defend = true;
                defendTimer = Random.Range(0.15f, 0.2f);
            }
        } else {
            lungeReacted = false;
        }

        if (actionTimer <= 0.0f)
        {
            if (player.ui.getVal(5) == 100 && player.ui.getVal(1) < 55 || (player.ui.getVal(1) < 66 && player.ui.getVal(3) < 33))
            {
                defend = true;
                defendTimer = .1f;
            }
            if (defend) {
                if (defendTimer <= 0.0f || (player.ui.getVal(5) == 100 && player.ui.getVal(1) > 66))
                {
                    defend = false;
                } else {
                    defendTimer -= Time.deltaTime;
                }
            } else if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 2f) {
                defend = Random.Range(0f, 1f) > .67f;
                defendTimer = Random.Range(.2f, .5f);
            }
            if (Vector3.Distance(player.trans.position, player.other.trans.position) <= .75f && !defend) {
                inputAttack = Random.Range(0f, 1f) > .67f;
                if (!player.IsGrounded()) {
                    inputAttack = Random.Range(0f, 1f) > .05f;
                }
                if (player.trans.position.y > .5f || player.rb.velocity.y > 0f || player.other.block) {
                    inputAttack = Random.Range(0f, 1f) > .05f;
                }
                inputDown = Random.Range(0f, 1f) < .5f;
                inputForward = Random.Range(0f, 3f) < 1f;
            } else if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 3f) {
                if (player.jAttackWindow > 0f)
                {
                    inputAttack = Random.Range(0f, 1f) < .2f && !player.other.block;
                    if (inputAttack)
                    {
                        inputForward = true;
                    }
                    if (inputAttack && player.tripCounter < 4)
                    {
                        inputDown = Random.Range(0f, 5 - player.tripCounter) > 1.5f;
                    }
                }
                inputAttack = Random.Range(0f, 1f) < .1f && !player.other.block;
                if (inputAttack)
                {
                    inputForward = true;
                }
                if (Random.Range(0f, 5 - player.tripCounter) > 1.75f
                        && Random.Range(0f, 1) > .25f)
                {
                    inputAttack = true;
                    inputForward = true;
                    inputDown = true;
                }
            }
        } else {
            actionTimer -= Time.deltaTime;
        }
        if (defend) {
            inputBlock = true;
            inputJump = Random.Range(0f, 1f) > .33f;
        }
        if (!player.IsGrounded() && Vector3.Distance(player.trans.position, player.other.trans.position) <= 1f) {
            ResetInputs();
            inputBackward = true;
        }
    }

    void Advantage() {
        if (!(player.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") 
            || defendTimer > 0f)) {
            actionTimer = Random.Range(0.05f, 0.25f);
        }
        if (defendTimer > 0f) {
            actionTimer = defendTimer;
            inputBlock = true;
            defendTimer -= Time.deltaTime;
        }

        if (player.other.anim.GetCurrentAnimatorStateInfo(0).IsName("Lunging Strike") && Vector3.Distance(player.trans.position, player.other.trans.position) <= 6f)
        {
            Neutral();
            return;
        } else {
            lungeReacted = false;
        }

        if (actionTimer <= 0.0f) {
            if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 2f) {
                inputAttack = true;
                if (player.tripCounter < 3)
                {
                    inputDown = Random.Range(0f, 5 - player.tripCounter) > 1f;
                }
                if (player.other.block && Random.Range(0f, 3f) < 2f && player.tripCounter < 3) {
                    inputDown = true;
                }
                if (player.tripCounter > 1 && Random.Range(0f, 1f) < .3f)
                {
                    inputDown = false;
                }
                inputForward = Random.Range(0f, 1f) < .2f;
                if (Random.Range(0f, 4f) < 1f) {
                    ResetInputs();
                    inputJump = true;
                }
            } else if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 6f)
            {
                inputAttack = Random.Range(0f, Vector3.Distance(player.trans.position, player.other.trans.position)) 
                                            < 1.5f;
                if (player.jAttackWindow > 0.0f) {
                    inputAttack = true;
                }
                if (inputAttack && player.other.block && Random.Range(0f, 3f) < 2f) {
                    inputAttack = false;
                }
                inputForward = inputAttack;
                if (Random.Range(0f, Vector3.Distance(player.trans.position, player.other.trans.position))
                        > 4f) {
                    ResetInputs();
                    inputJump = true;
                    stateTimer = 0.2f;
                }
                if (!(inputAttack || inputJump == false || player.IsGrounded())) {
                    stateTimer = 0.0f;
                    state = 0;
                } 
                if (player.other.block)
                {
                    ResetInputs();
                    if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 2f
                            && Random.Range(0f, 5 - player.tripCounter) > 1.75f
                            && Random.Range(0f, 1) > .25f)
                    {
                        inputAttack = true;
                        inputForward = true;
                        inputDown = true;
                    }
                }
            }
            if (player.ui.getVal(5) == 100 && player.ui.getVal(1) < 25)
            {
                ResetInputs();
                defend = true;
                defendTimer = .1f;
                state = 0;
                stateTimer = 0.0f;
            }
        } else {
            actionTimer -= Time.deltaTime;
        }
        
    }

    void Disadvantage() {
        if (player.other.anim.GetCurrentAnimatorStateInfo(0).IsName("Lunging Strike") && Vector3.Distance(player.trans.position, player.other.trans.position) <= 6f)
        {
            Neutral();
            return;
        } else {
            lungeReacted = false;
        }
        inputBackward = true;
        inputForward = false;
        move = Random.Range(0f, 1f) < .8f;
        inputJump = Random.Range(0f, 1f) < .75f;
        inputBlock = Random.Range(0f, 1f) < .5f;
        if (!(player.other.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") 
            || player.other.block)) {
            inputBlock = Random.Range(0f, 1f) < .75f;
        }
        if (player.other.anim.GetCurrentAnimatorStateInfo(0).IsName("Tripping Attack"))
        {
            inputBlock = Random.Range(0f, 1f) < .12;
            inputJump = true;
        }

        if (!inputBlock && Vector3.Distance(player.trans.position, player.other.trans.position) <= 2f) {
            inputAttack = Random.Range(0f, 1f) < .75f;
        }
        if (inputAttack) {
            inputDown = Random.Range(0f, 1f) < .5f;
            inputForward = Random.Range(0f, 1f) < .4f;
            if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 1f) {
                inputBackward = Random.Range(0f, 1f) < .8f;
            }
            if (player.other.block)
            {
                inputDown = Random.Range(0f, 1f) < .88f;
                inputForward = Random.Range(0, 1f) < .05f;
                inputBackward = Random.Range(0, 1f) < .05f;
            }
        }
    }

    void ForwardBackLeftRight() {
        if (player.trans.localScale.x == 1) {
            inputRight = inputForward;
            inputLeft = inputBackward;
        } else {
            inputLeft = inputForward;
            inputRight = inputBackward;
        }
    }

    void chooseDirection() {
        if (Vector3.Distance(player.trans.position, player.other.trans.position) > 6)
        {
            moveForward = true;
            move = true;
            moveDirectionTimer = Random.Range(0.1f, 0.5f);
        } else if (Vector3.Distance(player.trans.position, player.other.trans.position) < 1) {
            moveForward = false;
            move = true;
            moveDirectionTimer = Random.Range(0.1f, 0.5f);
        } else {
            moveForward = Random.Range(0f, 1f) > .33f;
            move = Random.Range(0f, 1f) < .8f;
            moveDirectionTimer = Random.Range(0.2f, 0.5f);
        }
    }

    void footstoolAttempt() {
        if (!player.IsGrounded())
        {
            player.jHeld = false;
            inputJump = true;
        }
    }
}
