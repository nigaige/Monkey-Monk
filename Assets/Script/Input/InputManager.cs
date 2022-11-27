using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MonkeyMonk.Inputs
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private EventSystem eventSystem;

        [SerializeField] private InputMap currentMap;

        private void Awake()
        {
            Instance = this;
            SwitchInputMap(currentMap);
        }

        public void SwitchInputMap(InputMap map)
        {
            switch (map)
            {
                case InputMap.Game:

                    if (currentMap == InputMap.UI) eventSystem.gameObject.SetActive(false);
                    playerInput.SwitchCurrentActionMap("Player");
                    break;
                case InputMap.Map:

                    if(currentMap == InputMap.UI) eventSystem.gameObject.SetActive(false);
                    playerInput.SwitchCurrentActionMap("Map");
                    break;
                case InputMap.UI:

                    eventSystem.gameObject.SetActive(true);
                    playerInput.SwitchCurrentActionMap("UI");
                    break;
                default:
                    break;
            }

            currentMap = map;
        }

        public void DisableInput()
        {
            playerInput.DeactivateInput();
        }

        public void EnableInput()
        {
            playerInput.ActivateInput();
        }

    }

    public enum InputMap
    {
        Game,
        Map,
        UI
    }
}