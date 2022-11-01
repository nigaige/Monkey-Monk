using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    public abstract void OnGrabbed();
    public abstract void OnUnGrabbed();
}
