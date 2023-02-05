using MonkeyMonk.Inputs;
using MonkeyMonk.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathMenu : Menu
{
    public void Awake()
    {
        CloseMenu();
        GameManager.Instance.Player.OnDeathEvent += OpenMenu;
    }

    public override void OpenMenu()
    {
        base.OpenMenu();
        InputManager.Instance.SwitchInputMap(InputMap.UI);
    }

    // =============== BUTTON METHODS ======================

    public void GoBackToMenu()
    {
        CloseMenu();
        SceneMaster.Instance.LoadHubWorld();
    }

    public void RetryLevel()
    {
        SceneMaster.Instance.LoadLevel(SceneMaster.Instance.CurrentLevel);
    }
}