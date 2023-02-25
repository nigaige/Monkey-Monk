using UnityEngine;
using UnityEngine.Events;

public abstract class ATriggerable : MonoBehaviour
{
    protected bool IsActive { get; set; }
    [SerializeField] AActivable activableObject;
    [SerializeField] protected UnityEvent _actions;

    public abstract void ActivateTrigger(bool status);

    public virtual void TriggerAction()
    {
        activableObject.Activate();
        _actions.Invoke();
    }
}
