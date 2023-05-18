using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] RectTransform[] elements;
    [SerializeField] float[] vals;
    [SerializeField] Starter starter;
    public Controller P1;
    public Controller P2;
    public Image vignette;
    public RectTransform vignetteTrans;
    Color color;

    public void Start() {
        for (int i = 0; i < 4; i++)
        {
            vals[i] = 100;
        }
        vals[4] = 0;
        vals[5] = 0;
    }

    void Update() {
        elements[0].anchoredPosition = new Vector2((vals[0] / 100f) * 345 - 345, 0);
        elements[1].anchoredPosition = new Vector2((vals[1] / 100f) * -345 - 39, 0);
        elements[2].anchoredPosition = new Vector2((vals[2] / 100f) * 297 - 297, 0);
        elements[3].anchoredPosition = new Vector2((vals[3] / 100f) * -297 - 87, 0);
        elements[4].anchoredPosition = new Vector2((vals[4] / 100f) * 249 - 249, 0);
        elements[5].anchoredPosition = new Vector2((vals[5] / 100f) * -249 - 135, 0);

        color = vignette.color;
        if (vals[0] < vals[1]) {
            if (vals[0] < 33) {
                color.a = (33f - vals[0]) / 66f;
            } else {
                color.a = 0f;
            }
        } else {
            if (vals[1] < 33) {
                color.a = (33f - vals[1]) / 66f;
            } else {
                color.a = 0f;
            }
        }
        if (!starter.firstTime && starter.timer > 1.5f) {
            color.a = .5f;
        }
        vignette.color = color;
        vignetteTrans.localScale = new Vector3(1.25f - color.a / 3f, 1.25f - color.a / 3f, 0);
    }

    public void setVal(int val, float set) {
        vals[val] = set;
        if (vals[val] > 100) {
            vals[val] = 100;
        }
    }

    public void addVal(int val, float add) {
        if (val > 3 && vals[val] + add >= 100 && vals[val] < 100)
        {
            FindObjectOfType<AudioManager>().play("AdrenFull");
        }

        vals[val] += add;
        if (vals[val] > 100) {
            vals[val] = 100;
        }

        if (vals[0] <= 0)
        {
            starter.reStart(1);
        } else if (vals[1] <= 0) {
            starter.reStart(0);
        }
    }
    
    public float getVal(int val) {
        return vals[val];
    }
}
