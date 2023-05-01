using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] int selectedButton = -1;
    [SerializeField] KeyCode[] controls;
    [SerializeField] TextMeshProUGUI[] buttonText;
    [SerializeField] GameObject controlPrefab;
    [SerializeField] Toggle[] controllerSelection;
    Controls controlObject;

    void Start() {
        if (PlayerPrefs.HasKey("0")) {
            for (int i = 0; i < 12; i++)
            {
                controls[i] = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(i.ToString()));
            }
        } else {
            for (int i = 0; i < 12; i++)
            {
                PlayerPrefs.SetString(i.ToString(), controls[i].ToString());
            }
        }
        
    }

    void Update()
    {
        for (int i = 0; i < 12; i++)
        {
            buttonText[i].text = controls[i].ToString();
        }

        for (int i = 0; i < 4; i++)
        {
            controllerSelection[i].interactable = FindObjectsOfType<PlayerControlManager>().Length > i % 2;
            if (!controllerSelection[i].interactable) { controllerSelection[i].isOn = false; }
        }
    }

    public void selectButton(int i) {
        selectedButton = i;
    }

    public void startGame() {
        DontDestroyOnLoad(Instantiate(controlPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        controlObject = FindObjectOfType<Controls>();
        controlObject.controls = controls;
        controlObject.P1controller = controllerSelection[0].isOn;
        controlObject.P2controller = controllerSelection[1].isOn;
        PlayerControlManager[] tempArr = FindObjectsOfType<PlayerControlManager>();
        if (tempArr.Length > 0 && tempArr[0].playerNumber == 1)
        {
            PlayerControlManager temp = tempArr[0];
            tempArr[0] = tempArr[1];
            tempArr[1] = temp;
        }
        if (tempArr.Length > 0) { tempArr[0].setStickJump(controllerSelection[2].isOn); }
        if (tempArr.Length > 1) { tempArr[1].setStickJump(controllerSelection[3].isOn); }
        controlObject.playerControlManagers = tempArr;
        SceneManager.LoadScene(1);
    }

    public void quit() {
        Application.Quit();
    }

    void OnGUI()
    {
        if (Event.current.isKey && Event.current.type == EventType.KeyDown && selectedButton != -1 && Event.current.keyCode != KeyCode.None)
        {
            controls[selectedButton] = Event.current.keyCode;
            PlayerPrefs.SetString(selectedButton.ToString(), Event.current.keyCode.ToString());
        }
    }
}
