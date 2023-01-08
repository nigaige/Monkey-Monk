using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Player : Entity
{
    private bool _onGround = false;
    private bool _isTouchingWall = false;

    [Header("Movement")]
    [SerializeField] float horizontalMaxVelocity = 1f;
    [SerializeField] private float maxFallVelocity = 20f;
    [SerializeField] private float gravityMultiplier;

    [SerializeField] private LayerMask groundMask;

    [Header("Ground Movement")]
    [SerializeField] private float horizontalAcceleration = 1f;

    [Header("Air Movement")]
    [SerializeField] private float airHorizontalAcceleration = 1f;

    [Header("Jump")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private float jumpForce = 0.4f;
    [SerializeField] private float coyoteJumpDelay = 0.1f;
    [SerializeField] private float jumpBufferingDelay = 0.1f;

    private Coroutine _coyoteJumpCoroutine;
    private bool _canJump = false;
    private Coroutine _jumpBufferCoroutine;
    private bool _isJumpBufferCall = false;

    [Header("Liane")]
    [SerializeField] private Liane liane;
    [SerializeField] private float lianeSpeed;
    [SerializeField] private float lianeHorizontalSpeed = 0.2f;
    [SerializeField] private float lianeVerticalSpeed = 1;
    [SerializeField] private float lianeMaxAngle = 90;
    [SerializeField] private float maxLianeLength = 10;

    private float _angleAcceleration;
    private float _angleVelocity;
    private float _angle = 0;

    [Header("Others")]
    private int lastDir = 1;


    [Header("Components")]
    private Rigidbody _rb;
    private Collider _collider;

    [Header("Inputs")]
    private Vector2 _movementInput;
    private Vector2 _clampedMovementInput;
    private bool _jumpInput;


    protected override void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        // Rotate player
        if (_movementInput.x < 0) transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (_movementInput.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);

        // Set OnGround
        if (!liane.isLianeFixed()) GroundCheck();
        WallCheck();

        // Liane / Basic Movement switcher
        if (liane.isLianeFixed()) LianeMovement();
        else Movement();
    }

    // ==================================== CHECK ===========================================

    private void GroundCheck()
    {
        Vector3 RayStart1;
        Vector3 RayStart2;

        float rayDist = 0.1f;

        RayStart1 = new Vector3(
            _collider.bounds.center.x - (_collider.bounds.extents.x - 0.01f),
            _collider.bounds.center.y - _collider.bounds.extents.y + rayDist,
            _collider.bounds.center.z
            );
        RayStart2 = new Vector3(
            _collider.bounds.center.x + (_collider.bounds.extents.x - 0.01f),
            _collider.bounds.center.y - _collider.bounds.extents.y + rayDist,
            _collider.bounds.center.z
            );

        float dist1 = CastARay(RayStart1, transform.TransformDirection(Vector3.down), rayDist * 2f, groundMask);
        float dist2 = CastARay(RayStart2, transform.TransformDirection(Vector3.down), rayDist * 2f, groundMask);

        if (_rb.velocity.y <= 0 && (dist1 > 0 || dist2 > 0)) // On Ground
        {
            _canJump = true;
            _onGround = true;

            // Jump buffering
            if (_isJumpBufferCall)
            {
                Jump();
                _isJumpBufferCall = false;
            }

            // Reset Coyote coroutine
            if (_coyoteJumpCoroutine != null)
            {
                StopCoroutine(_coyoteJumpCoroutine);
                _coyoteJumpCoroutine = null;
            }
        }
        else // On Air
        {
            if (_onGround && _canJump)
            {
                // Start Coyote jump coroutine
                if (_coyoteJumpCoroutine != null) 
                {
                    StopCoroutine(_coyoteJumpCoroutine);
                    _coyoteJumpCoroutine = null;
                }
                _coyoteJumpCoroutine = StartCoroutine(CoyoteJumpCoroutine());
            }

            _onGround = false;
        }
    }

    private void WallCheck()
    {
        if(_clampedMovementInput.x == 0)
        {
            _isTouchingWall = false;
            return;
        }

        float halfRayLength = 0.1f;

        Vector3 UpRay = new Vector3(
            _collider.bounds.center.x + (_collider.bounds.extents.x - halfRayLength) * Mathf.Sign(_clampedMovementInput.x),
            _collider.bounds.center.y + _collider.bounds.extents.y,
            _collider.bounds.center.z
            );
        Vector3 BottomRay = new Vector3(
            _collider.bounds.center.x + (_collider.bounds.extents.x - halfRayLength) * Mathf.Sign(_clampedMovementInput.x),
            _collider.bounds.center.y - (_collider.bounds.extents.y - 0.1f),
            _collider.bounds.center.z
            );

        float dist1 = CastARay(UpRay, Vector3.right * Mathf.Sign(_clampedMovementInput.x), halfRayLength * 2f, groundMask);
        float dist2 = CastARay(BottomRay, Vector3.right * Mathf.Sign(_clampedMovementInput.x), halfRayLength * 2f, groundMask);

        _isTouchingWall = (dist1 > 0) || (dist2 > 0);
    }

    private bool StepCheck()
    {
        if (_clampedMovementInput.x == 0) return false;

        float halfRayLength = 0.1f;

        Vector3 bottomRay = new Vector3(
            _collider.bounds.center.x + (_collider.bounds.extents.x - halfRayLength) * Mathf.Sign(_clampedMovementInput.x),
            _collider.bounds.center.y - _collider.bounds.extents.y,
            _collider.bounds.center.z
            );

        Vector3 upRay = new Vector3(
            _collider.bounds.center.x + (_collider.bounds.extents.x - halfRayLength) * Mathf.Sign(_clampedMovementInput.x),
            _collider.bounds.center.y - (_collider.bounds.extents.y - 0.1f),
            _collider.bounds.center.z
            );
        
        float dist1 = CastARay(bottomRay, Vector3.right * Mathf.Sign(_clampedMovementInput.x), halfRayLength + _rb.velocity.x * Time.fixedDeltaTime, groundMask);

        if(dist1 != -1)
        {
            float dist2 = CastARay(upRay, Vector3.right * Mathf.Sign(_clampedMovementInput.x), halfRayLength + _rb.velocity.x * Time.fixedDeltaTime + 0.1f, groundMask);


            if(dist2 == -1)
            {

                float dist3 = 0.1f - CastARay(upRay + Vector3.right * Mathf.Sign(_clampedMovementInput.x) * (halfRayLength + _rb.velocity.x * Time.fixedDeltaTime + 0.1f), Vector3.down, 0.15f, groundMask);

                transform.position += Vector3.up * dist3;

                return true;
            }

        }

        return false;
    }

    // ==================================== MOVEMENT ===========================================
#region Movement

    private void Movement()
    {
        // Fall movement
        if (!_onGround) FallMovement();

        // Set last direction
        if (_clampedMovementInput.x != 0 && _clampedMovementInput.x != lastDir) lastDir = (int)_clampedMovementInput.x;

        // Ground / Air Movement switcher
        if (_onGround) GroundMovement();
        else AirMovement();


        // Fix Y wall stick
        if (_rb.velocity.y < 0)
        {
            Vector3 LeftRay = new Vector3(
                _collider.bounds.center.x - (_collider.bounds.extents.x - 0.01f),
                _collider.bounds.center.y - _collider.bounds.extents.y,
                _collider.bounds.center.z
                );
            Vector3 RightRay = new Vector3(
                _collider.bounds.center.x + (_collider.bounds.extents.x - 0.01f),
                _collider.bounds.center.y - _collider.bounds.extents.y,
                _collider.bounds.center.z
                );

            float dist1 = CastARay(LeftRay, Vector3.down, -_rb.velocity.y * Time.fixedDeltaTime, groundMask);
            float dist2 = CastARay(RightRay, Vector3.down, -_rb.velocity.y * Time.fixedDeltaTime, groundMask);

            if (dist1 != -1 || dist2 != -1)
            {
                float dist;

                if (dist1 == -1) dist = dist2;
                else if (dist2 == -1) dist = dist1;
                else dist = Mathf.Min(dist1, dist2);

                _rb.velocity = new Vector3(_rb.velocity.x, -dist / Time.fixedDeltaTime, _rb.velocity.z);
            }
        }

        // Fix X wall stick
        if (_rb.velocity.x != 0)
        {
            Vector3 UpRay = new Vector3(
                _collider.bounds.center.x + (_collider.bounds.extents.x) * Mathf.Sign(_rb.velocity.x),
                _collider.bounds.center.y + _collider.bounds.extents.y,
                _collider.bounds.center.z
            );
            Vector3 BottomRay = new Vector3(
                _collider.bounds.center.x + (_collider.bounds.extents.x) * Mathf.Sign(_rb.velocity.x),
                _collider.bounds.center.y - (_collider.bounds.extents.y - 0.1f/*step size*/),
                _collider.bounds.center.z
            );

            RaycastHit hit1, hit2;

            bool hasHit1 = Physics.Raycast(UpRay, Vector3.right * Mathf.Sign(_rb.velocity.x), out hit1, Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime, groundMask);
            bool hasHit2 = Physics.Raycast(BottomRay, Vector3.right * Mathf.Sign(_rb.velocity.x), out hit2, Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime, groundMask);

            if ((hasHit1 && !Physics.GetIgnoreCollision(_collider, hit1.collider)) || (hasHit2 && !Physics.GetIgnoreCollision(_collider, hit2.collider)))
            {
                float dist;

                if (!hasHit1) dist = hit2.distance;
                else if (!hasHit2) dist = hit1.distance;
                else dist = Mathf.Min(hit1.distance, hit2.distance);

                _rb.velocity = new Vector3(dist * Mathf.Sign(_rb.velocity.x) / Time.fixedDeltaTime, _rb.velocity.y, _rb.velocity.z);
            }
        }
    }

    private void FallMovement()
    {
        // Global Gravity Multiplier
        _rb.velocity += Vector3.up * (Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime);

        if (_rb.velocity.y < 0) // Fall multiplier
        {
            _rb.velocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        }
        else if (_rb.velocity.y > 0 && !_jumpInput) // LowJump multiplier
        {
            _rb.velocity += Vector3.up * (Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
        }

        // Clamp fall velocity
        if (_rb.velocity.y < -maxFallVelocity) _rb.velocity = new Vector3(_rb.velocity.x, -maxFallVelocity, 0);

        
    }

    private void GroundMovement()
    {
        float acceleration = horizontalAcceleration * _clampedMovementInput.x;

        float newHVelocity;
        if (_clampedMovementInput.x == 0) // Deceleration
        {
            if (_rb.velocity.x == 0) // End of decel
            {
                newHVelocity = 0;
            }
            else if (_rb.velocity.x > 0) // Pos decel
            {
                newHVelocity = _rb.velocity.x - horizontalAcceleration * Time.deltaTime;
                if (newHVelocity < 0) newHVelocity = 0;
            }
            else // Neg decel
            {
                newHVelocity = _rb.velocity.x + horizontalAcceleration * Time.deltaTime;
                if (newHVelocity > 0) newHVelocity = 0;
            }
        }
        else // Acceleration
            newHVelocity = _rb.velocity.x + acceleration * Time.deltaTime;


        // Clamp Velocity (see horizontalMaxVelocity)
        if (newHVelocity > horizontalMaxVelocity) newHVelocity = horizontalMaxVelocity;
        else if (newHVelocity < -horizontalMaxVelocity) newHVelocity = -horizontalMaxVelocity;

        // Set Velocity
        _rb.velocity = new Vector3(newHVelocity, _rb.velocity.y, 0);

        // Step detection
        StepCheck();
    }

    private void AirMovement()
    {
        float acceleration = airHorizontalAcceleration * _clampedMovementInput.x;

        float newHVelocity;
        if (_clampedMovementInput.x == 0) // Deceleration
        {
            if (_rb.velocity.x == 0) // End of decel
            {
                newHVelocity = 0;
            }
            else if (_rb.velocity.x > 0) // Pos decel
            {
                newHVelocity = _rb.velocity.x - airHorizontalAcceleration * Time.deltaTime;
                if (newHVelocity < 0) newHVelocity = 0;
            }
            else // Neg decel
            {
                newHVelocity = _rb.velocity.x + airHorizontalAcceleration * Time.deltaTime;
                if (newHVelocity > 0) newHVelocity = 0;
            }
        }
        else // Acceleration
            newHVelocity = _rb.velocity.x + acceleration * Time.deltaTime;


        // Clamp Velocity (see horizontalMaxVelocity) execpt if already over the threshold (used to keep liane propulsion inertia)
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

        // Set Velocity
        _rb.velocity = new Vector3(newHVelocity, _rb.velocity.y, 0);
    }

    private void LianeMovement()
    {
        // Calculate angle
        _angleAcceleration = Physics.gravity.y * (gravityMultiplier - 1) * Mathf.Sin(_angle) /*/ liane.GetLianeLength()*/;
        //Debug.DrawRay(transform.position, Vector3.Cross(-liane.GetLianeDir().normalized, -Vector3.forward) * _angleAcceleration, Color.red);

        // Accel / Decel
        if (_clampedMovementInput.x != 0)
        {
            if (Mathf.Sign(_angleVelocity) == Mathf.Sign(_clampedMovementInput.x)) // acceleration
            {
                float newPredAngleAccel = _angleAcceleration + Mathf.Sign(_angleVelocity) * lianeHorizontalSpeed * Mathf.Abs(Mathf.Cos(_angle));
                float newPredAngleVel = _angleVelocity + newPredAngleAccel * Time.deltaTime * lianeSpeed;

                float higherA = GetPredictedHigherAngle(newPredAngleAccel, newPredAngleVel, _angle + newPredAngleVel * lianeSpeed * Time.deltaTime);

                if (higherA < Mathf.Deg2Rad * lianeMaxAngle) _angleAcceleration = newPredAngleAccel;
            }
            else if (Mathf.Sign(_angleVelocity) != Mathf.Sign(_clampedMovementInput.x)) // deceleration
            {
                _angleAcceleration -= Mathf.Sign(_angleVelocity) * lianeHorizontalSpeed * Mathf.Abs(Mathf.Cos(_angle));
            }
        }

        float newAngleVel = _angleVelocity + _angleAcceleration * Time.deltaTime * lianeSpeed;

        _angleVelocity = newAngleVel;

        if (_clampedMovementInput.x == 0) _angleVelocity *= 0.995f; // Loss

        _angle += _angleVelocity * lianeSpeed * Time.deltaTime;

        // Move to next pos
        Vector3 target = liane.LianePosition + liane.GetLianeLength() * new Vector3(Mathf.Sin(_angle), -Mathf.Cos(_angle), 0);
        _rb.velocity = (target - _rb.position) / Time.deltaTime;

        // Vertical Liane Movement
        if (_clampedMovementInput == Vector2.up || _clampedMovementInput == Vector2.down)
        {
            if (liane.GetLianeLength() - _clampedMovementInput.y * lianeVerticalSpeed * Time.deltaTime > liane.MaxLength) // end of the rope
            {
                _rb.velocity -= (liane.LianePosition - transform.position).normalized * (liane.MaxLength - liane.GetLianeLength());
                liane.SetLianeLength(liane.MaxLength);
            }
            else
            {
                _rb.velocity += _clampedMovementInput.y * (liane.LianePosition - transform.position).normalized * lianeVerticalSpeed;
                liane.SetLianeLength(liane.GetLianeLength() - _clampedMovementInput.y * lianeVerticalSpeed * Time.deltaTime);
            }
        }
    }


    #endregion

    // ==================================== UTILITES ===========================================

    private IEnumerator JumpBufferCoroutine()
    {
        _isJumpBufferCall = true;
        yield return new WaitForSeconds(jumpBufferingDelay);
        _isJumpBufferCall = false;
    }

    IEnumerator CoyoteJumpCoroutine()
    {
        yield return new WaitForSeconds(coyoteJumpDelay);
        _canJump = false;
        _coyoteJumpCoroutine = null;
    }


    float CastARay(Vector3 pos, Vector3 dir, float length, LayerMask mask){
        RaycastHit hit;

        bool hitPlateform = Physics.Raycast(pos, dir, out hit, length, mask);
        if (hitPlateform) Debug.DrawRay(pos, dir * length, Color.green);
        else Debug.DrawRay(pos, dir * length, Color.red);

        if (hitPlateform) return hit.distance;
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

    /// <summary>
    /// Calculate the higher angle of the pendulum depending on the current angle, velocity and acceleration
    /// </summary>
    private float GetPredictedHigherAngle(float angleAcceleration, float angleVelocity, float angle) // brute force higher pendulum angle
    {
        // init
        float curr_angle_accel = angleAcceleration;
        float curr_angle_vel = angleVelocity;
        float curr_angle = angle;

        // if the angle has already increased in the past (starting from this call)
        bool has_increased = false;

        // loop
        while (true)
        {
            // Compute new pendulum values
            float next_angle_accel = Physics.gravity.y * (gravityMultiplier - 1) * Mathf.Sin(curr_angle) / liane.GetLianeLength();
            float next_angle_vel = curr_angle_vel + curr_angle_accel * Time.fixedDeltaTime * lianeSpeed;
            float next_angle = curr_angle + next_angle_vel * lianeSpeed * Time.fixedDeltaTime;

            if (Mathf.Abs(curr_angle) < Mathf.Abs(next_angle)) has_increased = true;
            
            // Break if Higher point found
            if (has_increased && Mathf.Abs(curr_angle) >= Mathf.Abs(next_angle)) break;

            // Break if no higher point (ex: loop)
            if (Mathf.Abs(curr_angle) > Mathf.PI * 2) break;

            // Set new values to currents
            curr_angle_accel = next_angle_accel;
            curr_angle_vel = next_angle_vel;
            curr_angle = next_angle;
        }

        return Mathf.Abs(curr_angle);
    }

    

    // Release liane on ground hit
    private void OnCollisionStay(Collision collision)
    {
        if (!liane.isLianeFixed()) return;

        if (((1 << collision.gameObject.layer) & groundMask) > 0) ReleaseLiane();
    }


    // ==================================== INPUT METHODS ===========================================

    private void Jump()
    {
        if (liane.isLianeFixed()) // TODO : Boost end velocity
        {
            ReleaseLiane();

            return;
        }
        else if (!_canJump)
        {
            // Reset Jump buffering coroutine
            if (_jumpBufferCoroutine != null)
            {
                StopCoroutine(_jumpBufferCoroutine);
                _jumpBufferCoroutine = null;
            }
            _jumpBufferCoroutine = StartCoroutine(JumpBufferCoroutine());

            return;
        }

        // Jump
        _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, 0);
        GetComponent<AudioSource>().Play();

        _canJump = false;
    }

    private void LaunchLiane()
    {
        if (_clampedMovementInput.x > 0) liane.Extend(1); // Right
        else if (_clampedMovementInput.x < 0) liane.Extend(3); // Left
        else liane.Extend(2); // Up


        if (!liane.isLianeFixed()) return;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PassthroughPlatform"), true);


        
        // TEST : Set default liane angle velocity
        Vector3 playerLianeDir = PerpendicularClockwise(liane.GetLianeDir().normalized);

        //Debug.DrawLine(transform.position, transform.position + playerLianeDir, Color.red, 1f);
        float vel = Vector3.Dot(_rb.velocity, playerLianeDir);
        

        // Set default liane angle velocity
        //float vel = _rb.velocity.x;
        _angleVelocity = vel / liane.GetLianeLength();

        // Reset linear velocity
        _rb.velocity = Vector3.zero;

        // Set default angle
        _angle = Mathf.Deg2Rad * Vector3.SignedAngle(Vector3.down, -liane.GetLianeDir().normalized, Vector3.forward);
    }

    private void ReleaseLiane()
    {
        liane.Release();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PassthroughPlatform"), false);
    }

    // ==================================== INPUTS ===========================================

    public void OnMovement(InputAction.CallbackContext callback)
    {
        _movementInput = callback.ReadValue<Vector2>();

        _clampedMovementInput = Vector2.zero;
        if (_movementInput == Vector2.zero) return;

        float movementAngleDeg = Mathf.Atan2(_movementInput.y, _movementInput.x) * Mathf.Rad2Deg;

        if (movementAngleDeg >= 157.5 || movementAngleDeg <= -157.5)
        {
            _clampedMovementInput = new Vector2(-1, 0);
        }
        else if (movementAngleDeg > 112.5)
        {
            _clampedMovementInput = new Vector2(-1, 1);
        }
        else if (movementAngleDeg > 67.5)
        {
            _clampedMovementInput = new Vector2(0, 1);
        }
        else if (movementAngleDeg > 22.5)
        {
            _clampedMovementInput = new Vector2(1, 1);
        }
        else if (movementAngleDeg < -112.5)
        {
            _clampedMovementInput = new Vector2(-1, -1);
        }
        else if (movementAngleDeg < -67.5)
        {
            _clampedMovementInput = new Vector2(0, -1);
        }
        else if (movementAngleDeg < -22.5)
        {
            _clampedMovementInput = new Vector2(1, -1);
        }
        else
        {
            _clampedMovementInput = new Vector2(1, 0);
        }
        /*
        if (_clampedMovementInput.x > 0) _clampedMovementInput.x = 1;
        else if (_clampedMovementInput.x < 0) _clampedMovementInput.x = -1;

        if (_clampedMovementInput.y > 0) _clampedMovementInput.y = 1;
        else if (_clampedMovementInput.y < 0) _clampedMovementInput.y = -1;*/
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

        if (_onGround || _isTouchingWall) return; // TODO: Check this (passthrough platform)

        if (!liane.isLianeFixed()) LaunchLiane();
        else ReleaseLiane();
    }
}
