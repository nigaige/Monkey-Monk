using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATriggerable : MonoBehaviour
{
    bool IsActive { get; }
    public abstract void ActivateTrigger(bool status);

    public abstract void TriggerAction();
}
