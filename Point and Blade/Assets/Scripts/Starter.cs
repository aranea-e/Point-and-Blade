using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Starter : MonoBehaviour
{
    [SerializeField] Controller P1;
    [SerializeField] Controller P2;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Sprite[] sprites;
    [SerializeField] UI ui;
    [SerializeField] Transform trans;
    [SerializeField] CameraScript cam;

    [SerializeField] TextMeshProUGUI P1txt;
    [SerializeField] TextMeshProUGUI P2txt;

    public float timer;
    public bool firstTime;

    [SerializeField] int[] scores;

    public bool tutorial;

    void Start()
    {
        if (!tutorial) {
            timer = 3.0f;
            firstTime = true;
            FindObjectOfType<AudioManager>().play("Starter");
        } else {
            timer = 0.0f;
        }
        
    }

    public void reStart(int winner) {
        scores[winner]++;
        timer = 3.0f;
        firstTime = false;
        P1txt.text = scores[0].ToString();
        P2txt.text = scores[1].ToString();
        FindObjectOfType<AudioManager>().play("Starter");
            
        ui.Start();
    }
    
    void Update()
    {
        if (sprite.enabled && timer <= 0)
        {
            FindObjectOfType<AudioManager>().play("Start");
        }
        P1.starting = false;
        P2.starting = false;
        sprite.enabled = false;
        P1txt.enabled = false;
        P2txt.enabled = false;
        if (timer > 0)
        {
            ui.Start();
            timer -= Time.deltaTime;
            if (sprite.sprite != sprites[(int) timer])
            {
                FindObjectOfType<AudioManager>().play("Starter");
            }
            sprite.sprite = sprites[(int) timer];
            P1.starting = true;
            P2.starting = true;
            sprite.enabled = true;
            P1txt.enabled = !firstTime;
            P2txt.enabled = !firstTime;
            if (timer < 1.5f || firstTime)
            {
                P1.trans.position = new Vector3(-3, 0.9400001f, 0);
                P2.trans.position = new Vector3(3, 0.9400001f, 0);
            }
        }
        cam.Update();
        trans.position = new Vector3(cam.trans.position.x, 3, 0);
    }
}
