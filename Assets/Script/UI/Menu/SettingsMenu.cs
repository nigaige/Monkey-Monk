using MonkeyMonk.Inputs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    [SerializeField] private TMP_Dropdown profileDropdown;
    [SerializeField] private Toggle autoSwitchToggle;

    private void InitUI()
    {
        // Switch
        autoSwitchToggle.isOn = !InputManager.Instance.PlayerInput.neverAutoSwitchControlSchemes;

        // Dropdown
        string currentScheme = InputManager.Instance.PlayerInput.currentControlScheme;

        int currentSchemeIndex = -1;
        List<string> schemes = new();
        for (int i = 0; i < InputManager.Instance.PlayerInput.actions.controlSchemes.Count; i++)
        {
            string curr = InputManager.Instance.PlayerInput.actions.controlSchemes[i].name;

            schemes.Add(curr);
            if (curr == currentScheme) currentSchemeIndex = i;
        }

        profileDropdown.ClearOptions();
        profileDropdown.AddOptions(schemes);

        profileDropdown.value = currentSchemeIndex;
        profileDropdown.interactable = !autoSwitchToggle.isOn;
    }

    public override void OpenMenu()
    {
        base.OpenMenu();

        InitUI();

        profileDropdown.onValueChanged.AddListener(OnProfileChange);
        autoSwitchToggle.onValueChanged.AddListener(OnAutoSwitchChange);
    }

    public override void CloseMenu()
    {
        base.CloseMenu();

        profileDropdown.onValueChanged.RemoveListener(OnProfileChange);
        autoSwitchToggle.onValueChanged.RemoveListener(OnAutoSwitchChange);
    }

    private void OnProfileChange(int i)
    {
        var res = InputManager.Instance.PlayerInput.actions.FindControlScheme(profileDropdown.options[i].text).Value.PickDevicesFrom(InputSystem.devices);
        if (!res.isSuccessfulMatch) return;

        InputManager.Instance.PlayerInput.SwitchCurrentControlScheme(profileDropdown.options[i].text, res.devices.ToArray());
        res.Dispose();
    }

    private void OnAutoSwitchChange(bool autoswitch)
    {
        InputManager.Instance.PlayerInput.neverAutoSwitchControlSchemes = !autoswitch;
        profileDropdown.interactable = !autoswitch;
    }


}
