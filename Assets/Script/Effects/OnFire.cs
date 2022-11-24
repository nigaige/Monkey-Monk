using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFire : MonoBehaviour, IGrabbableModifier, ILianeAttachModifier
{
    public bool IsAttachable()
    {
        return false;
    }

    public bool IsGrabbable()
    {
        return false;
    }

    public void OnAttach()
    {

    }

    public void OnDetach()
    {

    }

    public void OnGrabbed(PlayerGrab grab)
    {

    }

    public void OnUnGrabbed(PlayerGrab grab)
    {

    }
}
