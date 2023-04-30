using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] RectTransform[] elements;
    [SerializeField] float[] vals;
    public Controller P1;
    public Controller P2;

    void Start() {
        for (int i = 0; i < 4; i++)
        {
            vals[i] = 100;
        }
    }

    void Update() {
        elements[0].anchoredPosition = new Vector2((vals[0] / 100f) * 345 - 345, 0);
        elements[1].anchoredPosition = new Vector2((vals[1] / 100f) * -345 - 39, 0);
        elements[2].anchoredPosition = new Vector2((vals[2] / 100f) * 297 - 297, 0);
        elements[3].anchoredPosition = new Vector2((vals[3] / 100f) * -297 - 87, 0);
        elements[4].anchoredPosition = new Vector2((vals[4] / 100f) * 249 - 249, 0);
        elements[5].anchoredPosition = new Vector2((vals[5] / 100f) * -249 - 135, 0);
    }

    public void setVal(int val, float set) {
        vals[val] = set;
        if (vals[val] > 100) {
            vals[val] = 100;
        }
    }

    public void addVal(int val, float add) {
        vals[val] += add;
        if (vals[val] > 100) {
            vals[val] = 100;
        }
    }
    
    public float getVal(int val) {
        return vals[val];
    }
}
