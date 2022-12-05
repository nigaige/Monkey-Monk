using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : Grabbable
{
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override bool IsHeavy()
    {
        return true;
    }

    public override void OnGrabbed(PlayerGrab grab)
    {
        base.OnGrabbed(grab);

        //Physics.IgnoreCollision(grab.GetComponent<Collider>(), GetComponent<Collider>());
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
    }

    public override void OnUnGrabbed(PlayerGrab grab)
    {
        base.OnUnGrabbed(grab);

        //Physics.IgnoreCollision(grab.GetComponent<Collider>(), GetComponent<Collider>(), false);
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
