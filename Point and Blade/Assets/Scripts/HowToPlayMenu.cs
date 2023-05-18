using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HowToPlayMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image background;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(1);
        }
    }

    public void SetMenu(string text) {
        this.text.text = text;
        background.enabled = true;
    }
}
