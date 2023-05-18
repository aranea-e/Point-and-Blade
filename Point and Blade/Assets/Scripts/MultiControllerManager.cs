using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiControllerManager : MonoBehaviour
{
    void Awake() {
        if (FindObjectsOfType<MultiControllerManager>().Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
