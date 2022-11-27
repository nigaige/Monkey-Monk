using MonkeyMonk.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private bool _isOpen;

    private void Awake()
    {
        CloseMenu();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0;
        InputManager.Instance.SwitchInputMap(InputMap.UI);

        _isOpen = true;
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        InputManager.Instance.SwitchInputMap(InputMap.Game);

        _isOpen = false;
    }

    // =============== BUTTON METHODS ======================

    public void QuitToLevelSelection()
    {
        CloseMenu();
        SceneMaster.Instance.LoadHubWorld();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (_isOpen) CloseMenu();
        else OpenMenu();
    }

}
