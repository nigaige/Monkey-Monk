using MonkeyMonk.Enemies;
using MonkeyMonk.Enemies.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrabbable : Grabbable
{
    private Enemy _enemy;
    private Rigidbody _rb;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rb = GetComponent<Rigidbody>();
    }

    public override bool IsHeavy()
    {
        return true;
    }

    public override void OnGrabbed(PlayerGrab grab)
    {
        _enemy.StateMachine.SwitchState(null);
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
    }

    public override void OnUnGrabbed(PlayerGrab grab)
    {
        _enemy.Knock();
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
