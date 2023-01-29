using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLianeAttach : MovableLianeAttach
{
    private Enemy _enemy;
    private Rigidbody _rb;

    [SerializeField] private EnemyDamageBox enemyDamageBox;

    protected override void Awake()
    {
        base.Awake();

        _enemy = GetComponent<Enemy>();
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnAttach()
    {
        base.OnAttach();

        _enemy.StateMachine.SwitchState(null);
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
    }

    public override void OnDetach()
    {
        base.OnDetach();

        if (enemyDamageBox != null) enemyDamageBox.enabled = true;
        _enemy.Knock();
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
