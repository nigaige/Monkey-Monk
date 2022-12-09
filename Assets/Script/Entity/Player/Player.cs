using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Ground Movement")]
    [SerializeField] private float horizontalAcceleration = 1f;
    [SerializeField] float horizontalMaxVelocity =1f;

    [Header("Air Movement")]
    [SerializeField] private float airHorizontalAcceleration = 1f;

    [Header("Jump")]
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float jumpForce = 0.4f;
    [SerializeField] private float maxFallVelocity = -1;

    [Header("Others")]

    private int lastDir = 1;

    [SerializeField] int MaxJump = 2;
    public int nbJump = 1;

    [SerializeField] public bool onGround = false;
    
    [SerializeField] private LayerMask platformMask;

    //liane
    [SerializeField] private Liane liane;
    [SerializeField] private float lianeSpeed;
    private float lianeAcceleration = 100000;
    [SerializeField] private float minlianeSpeed = 1;
    

    private Rigidbody _rb;
    private Collider _collider;
    private Vector2 _movementInput;
    private bool _jumpInput;


    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }


    void FixedUpdate()
    {
        Checkground();

        if (liane.isLianeFixed())
        {
            lianeMovment();
        }
        else
        {
            hMovment();
            if (!onGround) vMovment();
        }
    }

    private void Checkground()
    {
        Vector3 RayStart1;
        Vector3 RayStart2;

        float rayDist = 0.1f;


        RayStart1 = new Vector3(
            transform.position.x - _collider.bounds.extents.x,
            transform.position.y - _collider.bounds.extents.y + rayDist,
            transform.position.z
            );
        RayStart2 = new Vector3(
            transform.position.x + _collider.bounds.extents.x,
            transform.position.y - _collider.bounds.extents.y + rayDist,
            transform.position.z
            );

        float dist1 = CastARay(RayStart1, transform.TransformDirection(Vector3.down), rayDist * 2f, platformMask);
        float dist2 = CastARay(RayStart2, transform.TransformDirection(Vector3.down), rayDist * 2f, platformMask);

        if (dist1 > 0 || dist2 > 0)
        {
            onGround = true;
            nbJump = MaxJump;
        }
        else
        {
            onGround = false;
        }

    }

    private void Jump()
    {
        if (liane.isLianeFixed()) // TODO : Reset velocity + add normal jump in dir
        {
            liane.Release();
            //lianeAcceleration = 100000;
            //acceleration = _rb.velocity.x / 1000;
            onGround = true;//WILL ALLOW THE JUMP
            nbJump = 1;
        }

        if (nbJump <= 0) return;

        // Jump
        _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, 0);
        nbJump--;
        GetComponent<AudioSource>().Play();
    }

    void vMovment() {

        _rb.velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !_jumpInput)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Clamp fall velocity
        if (_rb.velocity.y < -maxFallVelocity) _rb.velocity = new Vector3(_rb.velocity.x, -maxFallVelocity, 0);
    }

    void hMovment() {

        // Set last direction
        if (_movementInput.x != 0 && _movementInput.x != lastDir) lastDir = (int)_movementInput.x;


        // On ground movement
        if (onGround) GroundMovement();
        else AirMovement();
    }

    private void GroundMovement()
    {
        float acceleration = horizontalAcceleration * _movementInput.x;

        float newHVelocity;

        if (_movementInput.x == 0) // deceleration
        {
            if (_rb.velocity.x == 0)
            {
                newHVelocity = 0;
            }
            else if (_rb.velocity.x > 0)
            {
                newHVelocity = _rb.velocity.x - horizontalAcceleration * Time.deltaTime;
                if (newHVelocity < 0) newHVelocity = 0;
            }
            else
            {
                newHVelocity = _rb.velocity.x + horizontalAcceleration * Time.deltaTime;
                if (newHVelocity > 0) newHVelocity = 0;
            }
        }
        else // acceleration
            newHVelocity = _rb.velocity.x + acceleration * Time.deltaTime;



        if (newHVelocity > horizontalMaxVelocity) newHVelocity = horizontalMaxVelocity;
        else if (newHVelocity < -horizontalMaxVelocity) newHVelocity = -horizontalMaxVelocity;

        _rb.velocity = new Vector3(newHVelocity, _rb.velocity.y, 0);
    }

    private void AirMovement()
    {

        float acceleration = airHorizontalAcceleration * _movementInput.x;

        float newHVelocity;

        if (_movementInput.x == 0) // deceleration
        {
            if (_rb.velocity.x == 0)
            {
                newHVelocity = 0;
            }
            else if (_rb.velocity.x > 0)
            {
                newHVelocity = _rb.velocity.x - airHorizontalAcceleration * Time.deltaTime;
                if (newHVelocity < 0) newHVelocity = 0;
            }
            else
            {
                newHVelocity = _rb.velocity.x + airHorizontalAcceleration * Time.deltaTime;
                if (newHVelocity > 0) newHVelocity = 0;
            }
        }
        else // acceleration
            newHVelocity = _rb.velocity.x + acceleration * Time.deltaTime;



        if (newHVelocity > horizontalMaxVelocity)
        {
            if (_rb.velocity.x <= horizontalMaxVelocity) newHVelocity = horizontalMaxVelocity;
            else if (newHVelocity > _rb.velocity.x) newHVelocity = _rb.velocity.x;
        }
        else if (newHVelocity < -horizontalMaxVelocity)
        {
            if (_rb.velocity.x >= -horizontalMaxVelocity) newHVelocity = -horizontalMaxVelocity;
            else if (newHVelocity < _rb.velocity.x) newHVelocity = _rb.velocity.x;
        }

        _rb.velocity = new Vector3(newHVelocity, _rb.velocity.y, 0);
        Debug.Log(newHVelocity);
    }





    float CastARay(Vector3 pos,Vector3 dir,float length, LayerMask mask){
        RaycastHit Hit;

        Debug.DrawRay(pos, dir * length);
        bool hitPlateform = Physics.Raycast(pos, transform.TransformDirection( dir), out Hit, length, mask);  
        
        if(hitPlateform) return Hit.distance;
        return -1;
    }


    //orthogonal vector
    public Vector3 PerpendicularClockwise(Vector3 vect)
    {
        return new Vector3(vect.y, -vect.x, 0);
    }
    public Vector3 PerpendicularCounterClockwise(Vector3 vect)
    {
        return new Vector3(-vect.y, vect.x, 0);
    }


    // ============================== Liane

    private void LaunchLiane(){

        if (lastDir == 1)
        {
            liane.Extend(1); // Right
        }
        else
        {
            liane.Extend(3); // Left
        }
    }

    private void SetLianeAcceleration(Vector3 dir, Vector3 lianeDir)
    {
        lianeAcceleration = Vector3.Dot(dir, lianeDir);

        if (lianeAcceleration < minlianeSpeed) { lianeAcceleration = minlianeSpeed; }
        if (!liane.isLeftOfFixed())
        {
            lianeAcceleration *= -1;
        }
    }

    private void lianeMovment()
    {
        Vector3 lianeDir = liane.GetLianeDir();

        if (lianeAcceleration == 100000)
        {
            SetLianeAcceleration(_rb.velocity.normalized, lianeDir);
        }

        _rb.velocity = PerpendicularClockwise(lianeDir) * lianeAcceleration * lianeSpeed;

    }

    private void ReleaseLiane()
    {
        liane.Release();
        lianeAcceleration = 100000;

        //rb.velocity = new Vector3();
        //rb.AddForceAtPosition(PerpendicularCounterClockwise(liane.getLianeDir())*Math.Sign(lianeAcceleration) * minlianeSpeed, transform.position);

        // acceleration = rb.velocity.x / 1000;
    }

    // ============================== Inputs

    public void OnMovement(InputAction.CallbackContext callback)
    {
        _movementInput = callback.ReadValue<Vector2>();

        if (_movementInput.x > 0) _movementInput.x = 1;
        else if (_movementInput.x < 0) _movementInput.x = -1;

        if (_movementInput.y > 0) _movementInput.y = 1;
        else if (_movementInput.y < 0) _movementInput.y = -1;
    }

    public void OnJump(InputAction.CallbackContext callback)
    {
        _jumpInput = callback.ReadValue<float>() > 0f;

        if (!callback.started) return;

        Jump();
    }

    public void OnLiane(InputAction.CallbackContext callback)
    {
        if (!callback.started) return;

        if (!liane.isLianeFixed()) LaunchLiane();
        else ReleaseLiane();
    }

}
