using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerable
{
    bool IsActive { get; }
    void ActivateTrigger(bool status);

    void TriggerAction();
}
