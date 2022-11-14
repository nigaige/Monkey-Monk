using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, ITriggerable
{
    public bool IsActive { get; set; }

    [SerializeField] GameObject activableObject;
    private IActivable activableObjectScript;


    private void Start()
    {
        IsActive = false;
        var buttonBody = gameObject.transform.GetChild(1).gameObject;
        var buttonRenderer = buttonBody.GetComponent<Renderer>();
        buttonRenderer.material.SetColor("_Color", Color.red);
        activableObjectScript = activableObject.GetComponent<IActivable>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(IsActive == false)
        {
            // Change the button to mark it as activated
            var buttonBody = gameObject.transform.GetChild(1).gameObject;
            var buttonBodyRotation = buttonBody.transform.localRotation;
            var buttonBodyPosition= buttonBody.transform.localPosition;
            buttonBodyPosition.y -= 0.1f;
            buttonBody.transform.SetLocalPositionAndRotation(buttonBodyPosition, buttonBodyRotation);
            var buttonRenderer = buttonBody.GetComponent<Renderer>();
            buttonRenderer.material.SetColor("_Color", Color.green);
            // ButtonAction
            ActivateTrigger(true);
        }

    }

    public void ActivateTrigger(bool status)
    {
        IsActive = status;
        if (status)
        {
            TriggerAction();
        }
    }

    public void TriggerAction()
    {
        activableObjectScript.Activate();
    }
}
