using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using MonkeyMonk.Utilities;

namespace MonkeyMonk.Inputs
{
    public class InputLinker : MonoBehaviour
    {
        [SerializeField] private InputActionAsset actions;
        public PlayerInput.ActionEvent[] actionEvents;

        private MultiValueDictionary<string, UnityAction<InputAction.CallbackContext>> _actions = new();


        delegate void InputActionDelegate(InputAction.CallbackContext context);

        private void OnEnable()
        {
            foreach (var item in actionEvents)
            {
                if (item.GetPersistentEventCount() == 0) continue;

                for (int i = 0; i < item.GetPersistentEventCount(); i++)
                {
                    InputActionDelegate inputDelegate = (InputActionDelegate)System.Delegate.CreateDelegate(typeof(InputActionDelegate), item.GetPersistentTarget(i), item.GetPersistentMethodName(i));
                    UnityAction<InputAction.CallbackContext> inputAction = new(inputDelegate);

                    _actions.Add(item.actionId, inputAction);

                    PlayerInput.GetPlayerByIndex(0).actionEvents.First(x => x.actionId == item.actionId).AddListener(inputAction);
                }
            }
        }

        private void OnDisable()
        {
            if (PlayerInput.GetPlayerByIndex(0) == null) return;

            foreach (var listeners in _actions)
            {
                foreach (var listener in listeners.Value)
                {
                    PlayerInput.GetPlayerByIndex(0).actionEvents.First(x => x.actionId == listeners.Key).RemoveListener(listener);
                }
            }

            _actions.Clear();
        }

    }
}