using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : Grabbable
{
    [SerializeField] private LayerMask blockingLayers;

    protected Rigidbody _rb;

    protected bool _isPlayerProj = false;
    protected Vector2 _direction;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public virtual void Initialize(Vector2 direction, GameObject launcher)
    {
        _direction = direction;
        _isPlayerProj = launcher.tag == "Player";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled) return;

        if (!_isPlayerProj)
        {
            if(other.tag == "Player" && other.TryGetComponent(out TEST_PlayerMovement player))
            {
                player.Damage(1);
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.tag == "Enemy" && other.TryGetComponent(out Enemy enemy))
            {
                enemy.Damage(1);
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
