using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : Projectile
{
    private void FixedUpdate()
    {
        if (!_rb.isKinematic) _rb.velocity = _direction * _speed;
    }
}
