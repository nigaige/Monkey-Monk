using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrified : MonoBehaviour, IGrabbableModifier
{
    public bool IsGrabbable()
    {
        return false;
    }

    public void OnGrabbed(PlayerGrab grab)
    {

    }

    public void OnUnGrabbed(PlayerGrab grab)
    {

    }
}
