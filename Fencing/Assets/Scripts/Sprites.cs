using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour
{
    [SerializeField] SpriteRenderer torso;
    [SerializeField] SpriteRenderer head;
    [SerializeField] SpriteRenderer blade;

    [SerializeField] Sprite[] sprites;

    [SerializeField] Controller control;

    void Update()
    {
        if (control.hitstun > 0f) {
            torso.sprite = sprites[1];
            head.sprite = sprites[3];
        } else {
            torso.sprite = sprites[0];
            head.sprite = sprites[2];
        }

        if (control.attackTime > 0f) {
            blade.sprite = sprites[5];
        } else {
            blade.sprite = sprites[4];
        }
    }
}
