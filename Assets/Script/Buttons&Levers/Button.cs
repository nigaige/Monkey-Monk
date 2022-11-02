using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, ITriggerable
{
    public bool IsActive { get; set; }

    [SerializeField] GameObject wall;

    private void Start()
    {
        IsActive = false;
        var buttonBody = gameObject.transform.GetChild(1).gameObject;
        var buttonRenderer = buttonBody.GetComponent<Renderer>();
        buttonRenderer.material.SetColor("_Color", Color.red);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(IsActive == false)
        {
            // Change the button to mark it as activated
            var buttonBody = gameObject.transform.GetChild(1).gameObject;
            var buttonBodyRotation = buttonBody.transform.rotation;
            var buttonBodyPosition= buttonBody.transform.position;
            buttonBodyPosition.y -= 0.1f;
            buttonBody.transform.SetPositionAndRotation(buttonBodyPosition, buttonBodyRotation);
            var buttonRenderer = buttonBody.GetComponent<Renderer>();
            buttonRenderer.material.SetColor("_Color", Color.green);
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
        Destroy(wall);
    }
}
