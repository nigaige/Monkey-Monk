using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonkeyMonk.Inputs;

public class MainMenu : MonoBehaviour
{

    private void Awake()
    {
        InputManager.Instance.SwitchInputMap(InputMap.UI);
    }

    public void LoadGame()
    {
        SceneMaster.Instance.LoadHubWorld();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
