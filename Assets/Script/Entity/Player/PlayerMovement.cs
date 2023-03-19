using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonkeyMonk.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        // =============================== Variables
        #region Variables

        [Header("Movement")]
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask climbMask;

        [Header("Ground Movement")]
        [SerializeField] private float maxHorizontalVelocity = 1f;
        [SerializeField] private float horizontalAcceleration = 1f;

        [Header("Air Movement")]
        [SerializeField] private float maxFallVelocity = 20f;
        [SerializeField] private float airHorizontalAcceleration = 1f;

        [Header("Hang Movement")]
        [SerializeField] private float hangSpeed = 20f;
        [SerializeField] private LayerMask hangMask;

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

        [Header("Components")]
        private Rigidbody _rb;
        private Collider _collider;

        [Header("Inputs")]
        private Vector2 _movementInput;
        private Vector2 _clampedMovementInput;
        private bool _jumpInput;
        private bool _hangInput;



        private bool _isOnGround = false;
        private bool _isOnSolidGround;
        private bool _isTouchingWall = false;
        [SerializeField]
        private bool _canClimb = false;

        public Vector2 LastLookingDirection { get; private set; }
        public Vector2 ClampedMovementInput { get => _clampedMovementInput; }
        public bool JumpInput { get => _jumpInput; }
        public bool HangInput { get => _hangInput; }

        public bool IsOnGround { get => _isOnGround; }
        public bool IsOnSolidGround { get => _isOnSolidGround; }
        public bool IsTouchingWall { get => _isTouchingWall; }
        public bool CanClimb { get => _canClimb; }



        #endregion

        // =============================== Movement
        #region Movement

        #region Checks

        public void GroundCheck()
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

            RaycastHit hit1, hit2;

            bool hasHit1 = Physics.Raycast(RayStart1, Vector3.down, out hit1, rayDist * 2f, groundMask);
            bool hasHit2 = Physics.Raycast(RayStart2, Vector3.down, out hit2, rayDist * 2f, groundMask);

            if (_rb.velocity.y <= 0 && (hasHit1 || hasHit2)) // On Ground
            {
                _canJump = true;
                _isOnGround = true;

                _isOnSolidGround = ((hasHit1 && hit1.collider.gameObject.layer == LayerMask.NameToLayer("Block"))
                    || (hasHit2 && hit2.collider.gameObject.layer == LayerMask.NameToLayer("Block")));


                // Jump buffering
                if (_isJumpBufferCall)
                {
                    TryJump();
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
                _isOnSolidGround = false;

                if (_isOnGround && _canJump)
                {
                    // Start Coyote jump coroutine
                    if (_coyoteJumpCoroutine != null)
                    {
                        StopCoroutine(_coyoteJumpCoroutine);
                        _coyoteJumpCoroutine = null;
                    }
                    _coyoteJumpCoroutine = StartCoroutine(CoyoteJumpCoroutine());
                }

                _isOnGround = false;
            }
        }

        public void WallCheck()
        {
            if (_clampedMovementInput.x == 0)
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

        public bool HangCheck()
        {
            if (!_hangInput) return false;

            return TopHangCheck();
        }

        public bool TopHangCheck()
        {
            if(_rb.velocity.y < 0) return false;

            Vector3 RayStart1;
            Vector3 RayStart2;

            float rayDist = 0.1f;

            RayStart1 = new Vector3(
                _collider.bounds.center.x - (_collider.bounds.extents.x - 0.01f),
                _collider.bounds.center.y + _collider.bounds.extents.y - rayDist,
                _collider.bounds.center.z
                );
            RayStart2 = new Vector3(
                _collider.bounds.center.x + (_collider.bounds.extents.x - 0.01f),
                _collider.bounds.center.y + _collider.bounds.extents.y - rayDist,
                _collider.bounds.center.z
                );

            RaycastHit hit1, hit2;

            bool hasHit1 = Physics.Raycast(RayStart1, Vector3.up, out hit1, rayDist * 2f, hangMask);
            bool hasHit2 = Physics.Raycast(RayStart2, Vector3.up, out hit2, rayDist * 2f, hangMask);

            return hasHit1 || hasHit2;
        }
        
        public void CanClimbCheck()
        {
            Collider[] climbColiders = Physics.OverlapBox(_collider.bounds.center, _collider.bounds.extents, Quaternion.identity, climbMask);
            if(climbColiders.Length != 0)
            {
                _canClimb = true;
            } else
            {
                _canClimb = false;
            }
        }

        #endregion

        #region Physics Fix

        public void FixVerticalPenetration()
        {
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

                RaycastHit hit1, hit2;

                bool hasHit1 = Physics.Raycast(LeftRay, Vector3.down, out hit1, -_rb.velocity.y * Time.fixedDeltaTime, groundMask);
                bool hasHit2 = Physics.Raycast(RightRay, Vector3.down, out hit2, -_rb.velocity.y * Time.fixedDeltaTime, groundMask);

                if ((hasHit1 && !Physics.GetIgnoreCollision(_collider, hit1.collider)) || (hasHit2 && !Physics.GetIgnoreCollision(_collider, hit2.collider)))
                {
                    float dist;

                    if (!hasHit1) dist = hit2.distance;
                    else if (!hasHit2) dist = hit1.distance;
                    else dist = Mathf.Min(hit1.distance, hit2.distance);

                    _rb.velocity = new Vector3(_rb.velocity.x, -dist / Time.fixedDeltaTime, _rb.velocity.z);
                }
            }
        }

        public void FixHorizontalPenetration()
        {
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

        #endregion

        #region Better feel

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

        #endregion

        #region Actions

        public void TryJump()
        {
            if (!_canJump)
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

        public void TryLaunchLiane()
        {
            if (_clampedMovementInput.x > 0) liane.Extend(1); // Right
            else if (_clampedMovementInput.x < 0) liane.Extend(3); // Left
            else liane.Extend(2); // Up


            if (liane.isLianeFixed()) SwitchState(PlayerMovementType.Liane);
        }
        public void TryClimbing()
        {
            if (_canClimb && _currentStateType != PlayerMovementType.Climb)
            {
                _rb.useGravity = false;
                _rb.velocity = new Vector3(0, 0, 0);
                SwitchState(PlayerMovementType.Climb);
            } else if(_currentStateType == PlayerMovementType.Climb)
            {
                _rb.useGravity = true;
                SwitchState(PlayerMovementType.Air);
            }
        }

        #endregion

        float CastARay(Vector3 pos, Vector3 dir, float length, LayerMask mask)
        {
            RaycastHit hit;

            bool hitPlateform = Physics.Raycast(pos, dir, out hit, length, mask);
            if (hitPlateform) Debug.DrawRay(pos, dir * length, Color.green);
            else Debug.DrawRay(pos, dir * length, Color.red);

            if (hitPlateform) return hit.distance;
            return -1;
        }

        // Release liane on ground or wall hit
        private void OnCollisionStay(Collision collision)
        {
            if (!(_currentStateType == PlayerMovementType.Liane)) return;

            if (((1 << collision.gameObject.layer) & groundMask) > 0) ReevaluateMovementType();
        }

        #endregion

        // =============================== Inputs
        #region Inputs

        public void OnMovement(InputAction.CallbackContext callback)
        {
            _movementInput = callback.ReadValue<Vector2>();
            Debug.Log(_movementInput);
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

            if (_clampedMovementInput == new Vector2(0, 1)) LastLookingDirection = new Vector2(0, 1);
            else if(_clampedMovementInput.x < 0) LastLookingDirection = new Vector2(-1, 1).normalized;
            else if (_clampedMovementInput.x > 0) LastLookingDirection = new Vector2(1, 1).normalized;
        }

        public void OnJump(InputAction.CallbackContext callback)
        {
            _jumpInput = callback.ReadValue<float>() > 0f;

            if (!callback.started) return;

            _currentState?.OnJumpInput();
        }

        public void OnLiane(InputAction.CallbackContext callback)
        {
            if (!callback.started) return;

            _currentState?.OnLianeInput();
        }
        public void OnClimb(InputAction.CallbackContext callback)
        {
            if (!callback.started) return;
            _currentState?.OnClimbInput();
        }

        public void OnHang(InputAction.CallbackContext callback)
        {
            if(callback.started || callback.canceled) _hangInput = callback.ReadValue<float>() > 0f;
        }

        #endregion

        public void Reset()
        {
            _movementInput = Vector2.zero;
            _clampedMovementInput = Vector2.zero;
            _jumpInput = false;
            _hangInput = false;
        }

        // =============================== State machine
        #region StateMachine

        public PlayerMovementType CurrentMovementType { get => _currentStateType; }

        private Dictionary<PlayerMovementType, PlayerState> _states = new();

        private PlayerMovementType _currentStateType = PlayerMovementType.None;
        private PlayerState _currentState;

        private void Awake()
        {
            LastLookingDirection = new Vector2(1, 1).normalized;

            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            InitStates();
        }

        private void InitStates()
        {
            _states[PlayerMovementType.None] = null;
            _states[PlayerMovementType.Ground] = new PlayerGroundMovement(this, _rb, _collider, horizontalAcceleration, maxHorizontalVelocity, groundMask);
            _states[PlayerMovementType.Air] = new PlayerAirMovement(this, _rb, airHorizontalAcceleration, maxHorizontalVelocity, gravityMultiplier, fallMultiplier, lowJumpMultiplier, maxFallVelocity);
            _states[PlayerMovementType.Liane] = new PlayerLianeMovement(this, _rb, liane, gravityMultiplier, lianeHorizontalSpeed, lianeSpeed, lianeMaxAngle, lianeVerticalSpeed);
            _states[PlayerMovementType.Hang] = new PlayerHangMovement(this, _rb, _collider, hangMask, hangSpeed);
            _states[PlayerMovementType.Climb] = new PlayerClimbMovement(this, _rb, _collider, horizontalAcceleration, maxHorizontalVelocity);


            SwitchState(_currentStateType);

            ReevaluateMovementType();
        }

        public void ReevaluateMovementType()
        {
            if (_isOnGround) SwitchState(PlayerMovementType.Ground);
            else SwitchState(PlayerMovementType.Air);
        }

        private void Update()
        {
            _currentState?.Update();
        }

        private void FixedUpdate()
        {
            // Rotate player
            if (_movementInput.x < 0) transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (_movementInput.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);

            _currentState?.FixedUpdate();
        }

        public void SwitchState(PlayerMovementType stateType)
        {
            _currentState?.ExitState();
            _currentStateType = stateType;
            _currentState = _states[stateType];
            _currentState?.EnterState();
        }

        public void HotFixedUpdateSwitchState(PlayerMovementType stateType)
        {
            _currentState?.ExitState();
            _currentStateType = stateType;
            _currentState = _states[stateType];
            _currentState?.FixedUpdate();
            _currentState?.EnterState();
        }

#if UNITY_EDITOR
        // Editor fix
        private void OnValidate()
        {
            InitStates();
        }
#endif

        #endregion

    }

    public enum PlayerMovementType 
    {
        None,
        Ground,
        Air,
        Liane,
        Hang,
        Climb
    }
}