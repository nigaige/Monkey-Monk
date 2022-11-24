using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ATriggerable
{
    public bool IsActive { get; set; }
    [SerializeField] bool isOneWay;
    [SerializeField] AActivable activableObject;


    private void Start()
    {
        IsActive = false;
        var leverBody = gameObject.transform.GetChild(1).GetChild(1).gameObject;
        var leverRenderer = leverBody.GetComponent<Renderer>();
        leverRenderer.material.SetColor("_Color", Color.red);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check liane
        if(IsActive == false)
        {
            // Change the lever to mark it as activated
            var leverBody = gameObject.transform.GetChild(1).gameObject;
            var leverBodyRotation = leverBody.transform.localRotation;
            var leverBodyPosition= leverBody.transform.localPosition;
            leverBodyPosition.x += 0.5f;
            leverBody.transform.SetLocalPositionAndRotation(leverBodyPosition, leverBodyRotation);
            var leverRenderer = leverBody.transform.GetChild(1).GetComponent<Renderer>();
            leverRenderer.material.SetColor("_Color", Color.green);
            var leverCollider = gameObject.GetComponent<SphereCollider>();
            var leverColliderPosition = leverCollider.center;
            leverColliderPosition.x += 0.5f;
            leverCollider.center = leverColliderPosition;


            // leverAction
            ActivateTrigger(true);
        } else if (isOneWay == false)
        {

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

    public override void TriggerAction()
    {
        activableObject.Activate();
    }
}
