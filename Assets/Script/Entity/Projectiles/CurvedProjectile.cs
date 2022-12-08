using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedProjectile : Projectile
{

    public override void Initialize(Vector2 direction, float speed, GameObject launcher)
    {
        base.Initialize(direction, speed, launcher);

        _rb.velocity = _direction * _speed;
    }

}
