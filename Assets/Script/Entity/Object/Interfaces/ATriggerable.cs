using UnityEngine;
using UnityEngine.Events;

public abstract class ATriggerable : MonoBehaviour
{
    protected bool IsActive { get; set; }
    [SerializeField] AActivable activableObject;
    [SerializeField] protected UnityEvent _actions;
    [SerializeField] protected UnityEvent _resets;

    public abstract void ActivateTrigger(bool status);

    public virtual void TriggerAction()
    {
        activableObject.Activate();
        _actions.Invoke();
    }

    public void ResetTriggerAble()
    {
        IsActive = false;
        activableObject.ResetActivalble();
        _resets.Invoke();
    }
}
