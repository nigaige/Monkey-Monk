using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : Projectile
{
    [SerializeField] private float speed = 1;

    private void FixedUpdate()
    {
        if (!_rb.isKinematic) _rb.velocity = _direction * speed;
    }
}
