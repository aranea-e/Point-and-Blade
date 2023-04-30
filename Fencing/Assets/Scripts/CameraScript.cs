using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Controller P1;
    [SerializeField] Controller P2;
    [SerializeField] float zoomMod;
    public Transform trans;

    void Update()
    {
        float distance = Vector3.Distance(P1.trans.position, P2.trans.position);
        float average = (P1.trans.position.x + P2.trans.position.x) / 2.0f;

        if (distance < 9f) {
            distance = 9f;
        }

        if (distance > 10f) {
            distance = 10f;
        }

        if (average > 2.5f) {
            average = 2.5f;
        }

        if (average < -2.5f) {
            average = -2.5f;
        }

        Camera.main.orthographicSize = distance * zoomMod;
        trans.position = new Vector3(average, 5, -12.5f);
    }
}
