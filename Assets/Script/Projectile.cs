using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Grabbable
{
    [SerializeField] private LayerMask blockingLayers;

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
        if (!this.enabled) return;

        if (!_isPlayerProj)
        {
            if(other.tag == "Player" && other.TryGetComponent(out TEST_PlayerMovement player))
            {
                player.Damage(1);
                Debug.Log("Damage Player");
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.tag == "Enemy" && other.TryGetComponent(out Enemy enemy))
            {
                enemy.Damage(1);
                Debug.Log("Damage Enemy");
                Destroy(gameObject);
            }
        }
        
        if(((1 << other.gameObject.layer) & blockingLayers) > 0) Destroy(gameObject);
    }

    public override bool IsHeavy()
    {
        return false;
    }

    public override void OnGrabbed(PlayerGrab grab)
    {
        Debug.Log("HA");
        base.OnGrabbed(grab);

        this.enabled = false;
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
    }

    public override void OnUnGrabbed(PlayerGrab grab)
    {
        base.OnUnGrabbed(grab);

        this.enabled = true;
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
