using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedProjectile : Projectile
{
    [SerializeField] private float force = 1;

    public override void Initialize(Vector2 direction, GameObject launcher)
    {
        base.Initialize(direction, launcher);

        _rb.velocity = _direction * force;
    }

}
