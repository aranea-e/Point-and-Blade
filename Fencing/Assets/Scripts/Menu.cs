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
    [SerializeField] Toggle computer;
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

        if (!PlayerPrefs.HasKey("C1")) {
            PlayerPrefs.SetInt("C1", 1);
        }
        if (!PlayerPrefs.HasKey("C2")) {
            PlayerPrefs.SetInt("C2", 1);
        }
        if (!PlayerPrefs.HasKey("S1")) {
            PlayerPrefs.SetInt("S1", 0);
        }
        if (!PlayerPrefs.HasKey("S2")) {
            PlayerPrefs.SetInt("S2", 0);
        }
        if (!PlayerPrefs.HasKey("Com")) {
            PlayerPrefs.SetInt("Com", 0);
        } else {
            computer.isOn = PlayerPrefs.GetInt("Com") == 1;
        }
        
        if (FindObjectsOfType<PlayerControlManager>().Length > 0 
                && PlayerPrefs.HasKey("C1") && PlayerPrefs.GetInt("C1") == 1)
        {
            controllerSelection[0].isOn = true;
            if (PlayerPrefs.HasKey("S1") && PlayerPrefs.GetInt("S1") == 1)
            {
                controllerSelection[2].isOn = true;
            }
        }
        if (FindObjectsOfType<PlayerControlManager>().Length > 1 
             && PlayerPrefs.HasKey("C2") && PlayerPrefs.GetInt("C2") == 1)
        {
            controllerSelection[1].isOn = true;
            if (PlayerPrefs.HasKey("S2") && PlayerPrefs.GetInt("S2") == 1)
            {
                controllerSelection[3].isOn = true;
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

        controllerSelection[2].interactable = controllerSelection[0].isOn;
        controllerSelection[3].interactable = controllerSelection[1].isOn;

        if (controllerSelection[2].interactable) {
            controllerSelection[2].isOn = PlayerPrefs.GetInt("S1") == 1;
        } else {
            controllerSelection[2].isOn = false;
        }
        if (controllerSelection[3].interactable) {
            controllerSelection[3].isOn = PlayerPrefs.GetInt("S2") == 1;
        } else {
            controllerSelection[3].isOn = false;
        }
    }

    public void selectButton(int i) {
        selectedButton = i;
    }

    public void startGame(int index) {
        if (index == 1)
        {
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
            controlObject.com = computer.isOn;
        }
        
        SceneManager.LoadScene(index);
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

    public void updatePlayerPrefs(int button) {
        if (button < 2)
        {
            if (controllerSelection[button].isOn)
            {
                PlayerPrefs.SetInt("C" + (button + 1), 1);
            } else {
                PlayerPrefs.SetInt("C" + (button + 1), 0);
            }
        } else if (button < 4 && controllerSelection[button].isOn)
        {
            PlayerPrefs.SetInt("S" + (button - 1), 1);
        } else if (button < 4 && controllerSelection[button].interactable) {
            PlayerPrefs.SetInt("S" + (button - 1), 0);
        } else {
            if (computer.isOn) {
                PlayerPrefs.SetInt("Com", 1);
            } else {
                PlayerPrefs.SetInt("Com", 0);
            }
        }
    }

    public void controllerActivate(int controller) {
        
        if (controller == 0
                && PlayerPrefs.HasKey("C1") && PlayerPrefs.GetInt("C1") == 1)
        {
            controllerSelection[0].isOn = true;
            if (PlayerPrefs.HasKey("S1") && PlayerPrefs.GetInt("S1") == 1)
            {
                controllerSelection[2].isOn = true;
            }
        }
        if (controller == 1 
                && PlayerPrefs.HasKey("C2") && PlayerPrefs.GetInt("C2") == 1)
        {
            controllerSelection[1].isOn = true;
            if (PlayerPrefs.HasKey("S2") && PlayerPrefs.GetInt("S2") == 1)
            {
                controllerSelection[3].isOn = true;
            }
        }
    }
}
