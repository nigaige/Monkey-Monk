using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour, IGrabbable
{
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnGrabbed(PlayerGrab grab)
    {
        Physics.IgnoreCollision(grab.GetComponent<Collider>(), GetComponent<Collider>());
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
    }

    public void OnUnGrabbed(PlayerGrab grab)
    {
        //Physics.IgnoreCollision(grab.GetComponent<Collider>(), GetComponent<Collider>(), false);
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public bool IsHeavy()
    {
        return true;
    }
}
