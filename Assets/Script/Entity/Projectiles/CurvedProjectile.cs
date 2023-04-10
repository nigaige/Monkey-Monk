using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedProjectile : Projectile
{
    [SerializeField] private float force = 1;
    Coroutine RotateCoroutine;

    public override void Initialize(Vector2 direction, GameObject launcher)
    {
        base.Initialize(direction, launcher);

        _rb.velocity = _direction * force;
        transform.Rotate(new Vector3(0, 180, 0));
        RotateCoroutine = StartCoroutine(RotateProjectile());
    }

    private IEnumerator RotateProjectile()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, 0, 2f* _direction.x));
            yield return new WaitForSeconds(0.005f);

        }
    }
    public override void OnGrabbed(PlayerGrab grab)
    {
        base.OnGrabbed(grab);
        StopCoroutine(RotateCoroutine);
        RotateCoroutine = null;
        transform.SetLocalPositionAndRotation(transform.localPosition + new Vector3(0.2f,-1f,-0.5f), Quaternion.identity);
        transform.Rotate(new Vector3(0, 0, -50));
    }
    public override void OnUnGrabbed(PlayerGrab grab)
    {
        base.OnUnGrabbed(grab);
        transform.Rotate(new Vector3(0, 180, 0));
        RotateCoroutine = StartCoroutine(RotateProjectile());
    }

}
