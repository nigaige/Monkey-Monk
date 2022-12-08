using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float horizontalAcceleration = 1f;
    [SerializeField] float horizontalMaxVelocity =1f;

    [Header("Jump")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float jumpForce = 0.4f;

    [SerializeField] float vSpeed = 0f;
    [SerializeField] float maxFallVelocity = -1;



    private int dir;
    private int lastDir = 1;

    [SerializeField] int MaxJump = 2;
    public int nbJump = 1;

    [SerializeField] public bool onGround = false;
    public Vector3 RayCast_Dir;
    
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
            hMovment(_movementInput.x);
            if (!onGround) vMovment();
        }
    }

    void Jump()
    {
        if (liane.isLianeFixed()) // TODO : Reset velocity + add normal jump in dir
        {
            liane.resetLiane();
            lianeAcceleration = 100000;
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

    void hMovment(float hinput) {

        if (hinput > 0) dir = 1;
        else if (hinput < 0) dir = -1;
        else dir = 0;

        // Set last direction
        if (dir != 0 && dir != lastDir) lastDir = dir;


        // On ground movement
        if (!onGround) return;

        float acceleration = horizontalAcceleration * dir;
        float newHVelocity = _rb.velocity.x + acceleration * Time.deltaTime;

        if (newHVelocity > horizontalMaxVelocity) newHVelocity = horizontalMaxVelocity;
        else if (newHVelocity < -horizontalMaxVelocity) newHVelocity = -horizontalMaxVelocity;
        else if (dir == 0) newHVelocity = 0; // TODO : Check this

        _rb.velocity = new Vector3(newHVelocity, _rb.velocity.y, 0);
    }


    //orthogonal vector
    public Vector3 PerpendicularClockwise(Vector3 vect){
        return new Vector3(vect.y, -vect.x,0);
    }
    public Vector3 PerpendicularCounterClockwise(Vector3 vect){
        return new Vector3(-vect.y, vect.x,0);
    }


    private void setLianeAcceleration(Vector3 dir, Vector3 lianeDir){
        lianeAcceleration = Vector3.Dot(dir, lianeDir) ;
        if (lianeAcceleration < minlianeSpeed) {lianeAcceleration = minlianeSpeed;}
        if (!liane.isLeftOfFixed()){
            lianeAcceleration *= -1;
        }
    }

    private void lianeMovment(){
        Vector3 lianeDir = liane.getLianeDir();

        if (lianeAcceleration == 100000) {
            setLianeAcceleration(_rb.velocity, lianeDir);
        }
        
        Debug.Log(lianeAcceleration);
        

        
        _rb.velocity = PerpendicularCounterClockwise(lianeDir)*lianeAcceleration * lianeSpeed;

    }


    float CastARay(Vector3 pos,Vector3 dir,float length, LayerMask mask){
        RaycastHit Hit;

        Debug.DrawRay(pos, dir * length);
        bool hitPlateform = Physics.Raycast(pos, transform.TransformDirection( dir), out Hit, length, mask);  
        
        if(hitPlateform) return Hit.distance;
        return -1;
    }




    void Checkground()
    {
        Vector3 RayStart1;
        Vector3 RayStart2;

        float rayDist = 0.1f;


        float rayDir = (vSpeed > 0) ? 1 : -1;


        RayStart1 = new Vector3(
            transform.position.x - _collider.bounds.extents.x,
            transform.position.y + (_collider.bounds.extents.y - rayDist) * rayDir,
            transform.position.z
            );
        RayStart2 = new Vector3(
            transform.position.x + _collider.bounds.extents.x,
            transform.position.y + (_collider.bounds.extents.y - rayDist) * rayDir,
            transform.position.z
            );

        
        float raySize = (0.1f);

        RayCast_Dir = Vector3.down;
        if(rayDir >0){
            RayCast_Dir=Vector3.up;
            raySize = (0.2f);
        }

        float dist1 = CastARay(RayStart1, transform.TransformDirection( RayCast_Dir),raySize, platformMask);
        float dist2 = CastARay(RayStart2, transform.TransformDirection( RayCast_Dir),raySize, platformMask);

        if (dist1 > 0 || dist2 > 0) {
            float dist = (dist1 + dist2) / 2;

            onGround = true;
            nbJump = MaxJump;
        }else {
            onGround = false;
        }

    }


    void startLiane(){

        // Quit liane
        if (liane.isLianeFixed() || liane.getIsExtending())
        {
            //rb.velocity = new Vector3();
            //rb.AddForceAtPosition(PerpendicularCounterClockwise(liane.getLianeDir())*Math.Sign(lianeAcceleration) * minlianeSpeed, transform.position);
            liane.resetLiane();
            lianeAcceleration = 100000;
           // acceleration = rb.velocity.x / 1000;

            
        }
        else // Start liane
        {
            //TODO 8 DIRECTION
            if (lastDir == 1)
            {//right
                liane.startExtend(1);
            }
            else
            {
                liane.startExtend(3);
            }

        }
    }

    // ============================== Inputs

    public void OnMovement(InputAction.CallbackContext callback)
    {
        _movementInput = callback.ReadValue<Vector2>();
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

        startLiane();
    }

}
