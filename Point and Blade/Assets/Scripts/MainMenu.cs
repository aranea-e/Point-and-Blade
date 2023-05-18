using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] RectTransform swords;
    void Update() {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene(1);
        }
        swords.anchoredPosition = new Vector2(0, -192 + 15 * Mathf.Cos(Time.realtimeSinceStartup));
    }
}
