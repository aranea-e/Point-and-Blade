using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HowToPlayMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
        }
    }

    public void SetMenu(string text) {
        this.text.text = text;
    }
}
