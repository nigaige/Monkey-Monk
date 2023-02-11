using MonkeyMonk.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : Menu
{
    public void Awake()
    {
        CloseMenu();
        GameManager.Instance.OnLoseEvent += OpenMenu;
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
        GameMaster.Instance.GiveLives();
        SceneMaster.Instance.LoadLevel(SceneMaster.Instance.CurrentLevel);
    }
}