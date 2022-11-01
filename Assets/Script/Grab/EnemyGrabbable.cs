using MonkeyMonk.Enemies;
using MonkeyMonk.Enemies.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrabbable : MonoBehaviour, IGrabbable
{
    private Enemy _enemy;
    private Rigidbody2D _rb2d;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rb2d = GetComponent<Rigidbody2D>();
    }

    public void OnGrabbed()
    {
        _enemy.StateMachine.SwitchState(null);
        _rb2d.velocity = Vector2.zero;
        _rb2d.isKinematic = true;
    }

    public void OnUnGrabbed()
    {
        _enemy.Knock();
        _rb2d.isKinematic = false;
    }
}
