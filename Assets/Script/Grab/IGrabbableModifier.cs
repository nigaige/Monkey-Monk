using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbableModifier
{
    public abstract void OnGrabbed(PlayerGrab grab);
    public abstract void OnUnGrabbed(PlayerGrab grab);
    public abstract bool IsGrabbable();
}
