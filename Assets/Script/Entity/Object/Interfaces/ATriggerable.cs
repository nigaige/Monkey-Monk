using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATriggerable : MonoBehaviour
{
    protected bool IsActive { get; set; }
    [SerializeField] AActivable activableObject;
    public abstract void ActivateTrigger(bool status);

    public virtual void TriggerAction()
    {
        activableObject.Activate();
    }
}
