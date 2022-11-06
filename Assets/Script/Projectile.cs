using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Grabbable
{
    private Rigidbody _rb;

    private bool _isPlayerProj = false;
    private Vector2 _direction;
    private float _speed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector2 direction, float speed, GameObject launcher)
    {
        _direction = direction;
        _speed = speed;
        _isPlayerProj = launcher.tag == "Player";
    }

    public void Initialize(Vector2 direction, GameObject launcher)
    {
        Initialize(direction, _speed, launcher);
    }

    private void FixedUpdate()
    {
        if(!_rb.isKinematic) _rb.velocity = _direction * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isPlayerProj)
        {
            if(other.tag == "Player" && other.TryGetComponent(out Player player))
            {
                Debug.Log("Damage Player");
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.tag == "Enemy" && other.TryGetComponent(out Enemy enemy))
            {
                Debug.Log("Damage Enemy");
                Destroy(gameObject);
            }
        }
    }

    public override bool IsHeavy()
    {
        return false;
    }

    public override void OnGrabbed(PlayerGrab grab)
    {
        base.OnGrabbed(grab);
        
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
    }

    public override void OnUnGrabbed(PlayerGrab grab)
    {
        base.OnUnGrabbed(grab);

        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
