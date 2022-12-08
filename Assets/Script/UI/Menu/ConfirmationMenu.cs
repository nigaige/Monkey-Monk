using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private UnityEngine.UI.Button yesButton; // TODO : Put Button in a namespace
    [SerializeField] private UnityEngine.UI.Button noButton;

    public void Initialize(string confirmationText, UnityAction yesCallback, UnityAction noCallback)
    {
        text.text = confirmationText;
        yesButton.onClick.AddListener(yesCallback);
        noButton.onClick.AddListener(noCallback);
    }
}
