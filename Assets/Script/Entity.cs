using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Entity Stats")]
    [SerializeField] protected int maxHeathPoint;
    public int MaxHeathPoint { get => maxHeathPoint; }
    public int HealthPoints { get; protected set; }

    public event Action OnDamageEvent;
    public event Action OnDeathEvent;

    protected virtual void Awake()
    {
        HealthPoints = maxHeathPoint;
    }

    public void Damage(int damage)
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
