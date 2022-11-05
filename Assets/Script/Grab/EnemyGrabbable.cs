using MonkeyMonk.Enemies;
using MonkeyMonk.Enemies.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrabbable : MonoBehaviour, IGrabbable
{
    private Enemy _enemy;
    private Rigidbody _rb;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rb = GetComponent<Rigidbody>();
    }

    public void OnGrabbed(PlayerGrab grab)
    {
        _enemy.StateMachine.SwitchState(null);
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
    }

    public void OnUnGrabbed(PlayerGrab grab)
    {
        _enemy.Knock();
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public bool IsHeavy()
    {
        return true;
    }
}
