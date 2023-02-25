using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : ATriggerable
{

    private void Start()
    {
        IsActive = false;
        var buttonBody = gameObject.transform.GetChild(1).gameObject;
        var buttonRenderer = buttonBody.GetComponent<Renderer>();
        buttonRenderer.material.SetColor("_BaseColor", Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(IsActive == false)
        {

            #region visual
            // Change the button to mark it as activated
            var buttonBody = gameObject.transform.GetChild(1).gameObject;
            var buttonBodyRotation = buttonBody.transform.localRotation;
            var buttonBodyPosition= buttonBody.transform.localPosition;
            buttonBodyPosition.y -= 0.1f;
            buttonBody.transform.SetLocalPositionAndRotation(buttonBodyPosition, buttonBodyRotation);
            var buttonRenderer = buttonBody.GetComponent<Renderer>();
            buttonRenderer.material.SetColor("_BaseColor", Color.green);
            #endregion
            // ButtonAction
            ActivateTrigger(true);
            //Sound of button pressed
            GetComponent<AudioSource>().Play();
        }

    }

    public override void ActivateTrigger(bool status)
    {
        IsActive = status;
        if (status)
        {
            TriggerAction();
        }
    }

}
