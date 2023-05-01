using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlManager : MonoBehaviour
{
    public int playerNumber;

    public bool left;
    public bool block;
    public bool down;
    public bool up;
    public bool right;
    public bool attack;

    public bool stickJump;

    void Awake() {
        playerNumber = FindObjectsOfType<PlayerControlManager>().Length - 1;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Left(InputAction.CallbackContext context) {
        if (context.started) { left = true; } else if (context.canceled) { left = false; }
    }

    public void Block(InputAction.CallbackContext context) {
        if (context.started) { block = true; } else if (context.canceled) { block = false; }
    }

    public void Down(InputAction.CallbackContext context) {
        if (context.started) { down = true; } else if (context.canceled) { down = false; }
    }

    public void Up(InputAction.CallbackContext context) {
        if (context.started) { up = true; } else if (context.canceled) { up = false; }
    }

    public void StickUp(InputAction.CallbackContext context) {
        if (context.started && stickJump) { up = true; } else if (context.canceled) { up = false; }
    }

    public void Right(InputAction.CallbackContext context) {
        if (context.started) { right = true; } else if (context.canceled) { right = false; }
    }

    public void Attack(InputAction.CallbackContext context) {
        if (context.started) { attack = true; } else if (context.canceled) { attack = false; }
    }

    public void setStickJump(bool state) {
        stickJump = state;
        if (!state)
        {
            up = false;
        }
    }
}
