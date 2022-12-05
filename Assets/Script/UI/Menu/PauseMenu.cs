using MonkeyMonk.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup menu;
    [SerializeField] private GameObject confirmationMenuPrefab;
    private ConfirmationMenu _confirmationMenuObj;

    private bool _isOpen;

    private void Awake()
    {
        CloseMenu();
    }

    public void OpenMenu()
    {
        menu.gameObject.SetActive(true);
        Time.timeScale = 0;
        InputManager.Instance.SwitchInputMap(InputMap.UI);

        _isOpen = true;
    }

    public void CloseMenu()
    {
        menu.gameObject.SetActive(false);
        Time.timeScale = 1;
        InputManager.Instance.SwitchInputMap(InputMap.Game);

        _isOpen = false;
    }

    // =============== BUTTON METHODS ======================

    public void AskToQuitToLevelSelection()
    {
        menu.interactable = false;
        _confirmationMenuObj = Instantiate(confirmationMenuPrefab, transform).GetComponent<ConfirmationMenu>();
        _confirmationMenuObj.Initialize("Are you sure blablabla progression will be lost blablabla",
            () =>
            {
                QuitToLevelSelection();
                Destroy(_confirmationMenuObj.gameObject);
            },
            () =>
            {
                Destroy(_confirmationMenuObj.gameObject);
                menu.interactable = true;
            });
    }

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
