using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
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
    [SerializeField] double stateTimer;

    void Start() {
        controls = FindObjectOfType<Controls>();
        if (!controls.com)
        {
            Destroy(this.gameObject);
        }
        state = 0;
    }

    
    void Update() {
        ResetInputs();
        DetermineState();
        if (state == 0)
        {
            Neutral();
        }
        if (state == 1)
        {
            Advantage();
        }
        if (state == -1)
        {
            Disadvantage();
        }
        ForwardBackLeftRight();
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
            stateTimer = Random.Range(.25f, .75f);
        }
        if (player.hitstun > 0.0f) {
            state = -1;
            stateTimer = Random.Range(.5f, 1.25f);
        }
    }

    void Neutral() {
        if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 1.5f)
        {
            inputAttack = true;
            inputDown = Random.Range(0f, 1f) < .5f;
            inputForward = Random.Range(0f, 3f) < 1f;
        }
    }

    void Advantage() {
        if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 1.5f)
        {
            inputAttack = true;
            inputDown = Random.Range(0f, 1f) < .5f;
            inputForward = Random.Range(0f, 3f) < 1f;
        } else if (Vector3.Distance(player.trans.position, player.other.trans.position) <= 6f)
        {
            inputAttack = Random.Range(0f, Vector3.Distance(player.trans.position, player.other.trans.position)) 
                                        < 1.5f;
            inputForward = inputAttack;
            if (inputAttack == false)
            {
                stateTimer = 0.0f;
                state = 0;
            }
        }
    }

    void Disadvantage() {

    }

    void ForwardBackLeftRight() {
        if (player.trans.localScale.x == 1)
        {
            inputRight = inputForward;
            inputLeft = inputBackward;
        } else
        {
            inputLeft = inputForward;
            inputRight = inputBackward;
        }
    }
}
