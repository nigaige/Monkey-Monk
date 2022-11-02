using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour, IGrabbable
{
    private Rigidbody2D _rb2d;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    public void OnGrabbed(PlayerGrab grab)
    {
        Physics2D.IgnoreCollision(grab.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        _rb2d.velocity = Vector2.zero;
        _rb2d.isKinematic = true;
    }

    public void OnUnGrabbed(PlayerGrab grab)
    {
        //Physics2D.IgnoreCollision(grab.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
        _rb2d.isKinematic = false;
    }
}
