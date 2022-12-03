using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grabbable : MonoBehaviour
{
    public abstract bool IsHeavy();

    public virtual void OnGrabbed(PlayerGrab grab)
    {
        foreach (IGrabbableModifier modifier in GetComponents<IGrabbableModifier>())
        {
            modifier.OnGrabbed(grab);
        }
    }

    public virtual void OnUnGrabbed(PlayerGrab grab)
    {
        foreach (IGrabbableModifier modifier in GetComponents<IGrabbableModifier>())
        {
            modifier.OnUnGrabbed(grab);
        }
    }

    public bool IsGrabbable()
    {
        foreach (IGrabbableModifier modifier in GetComponents<IGrabbableModifier>())
        {
            if (!modifier.IsGrabbable()) return false;
        }

        return true;
    }
}
