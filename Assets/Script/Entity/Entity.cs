using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int MaxHeathPoint { get => maxHeathPoint; }
    public int HealthPoints { get; protected set; }
    public bool IsDead { get => HealthPoints <= 0; }
    
    public event Action OnDamageEvent;
    public event Action OnDeathEvent;
    
    [Header("Entity Stats")]
    [SerializeField] protected int maxHeathPoint;

    [SerializeField] private float invulnerabilityDuration;

    private bool _invulnerable;


    protected virtual void Awake()
    {
        HealthPoints = maxHeathPoint;
    }

    private void OnEnable()
    {
        _invulnerable = false;
    }

    public void Damage(int damage)
    {
        if (_invulnerable) return;

        if (HealthPoints - damage > 0)
        {
            HealthPoints -= damage;

            OnDamage();

            if (invulnerabilityDuration > 0) StartCoroutine(InvulnerabilityCoroutine());
        }
        else
        {
            HealthPoints = 0;

            OnDamage();
            OnDeath();
        }
    }

    public void DamageWithoutInvulnerability(int damage)
    {
        if (HealthPoints - damage > 0)
        {
            HealthPoints -= damage;

            OnDamage();
        }
        else
        {
            HealthPoints = 0;

            OnDamage();
            OnDeath();
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        _invulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        _invulnerable = false;
    }

    protected virtual void OnDamage()
    {
        OnDamageEvent?.Invoke();
    }

    protected virtual void OnDeath()
    {
        OnDeathEvent?.Invoke();
        Destroy(gameObject);
    }



}
