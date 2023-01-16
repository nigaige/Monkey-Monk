using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerGroundMovement : PlayerState
    {
        private Rigidbody _rb;
        private Collider _collider;
        private float _horizontalAcceleration;
        private float _maxHorizontalVelocity;
        private LayerMask _groundMask;

        public PlayerGroundMovement(PlayerMovement movement, Rigidbody rb, Collider collider, float horizontalAcceleration, float maxHorizontalVelocity, LayerMask groundMask) : base(movement)
        {
            _rb = rb;
            _collider = collider;
            _horizontalAcceleration = horizontalAcceleration;
            _maxHorizontalVelocity = maxHorizontalVelocity;
            _groundMask = groundMask;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _movement.GroundCheck();
            _movement.WallCheck();

            if (!_movement.IsOnGround)
            {
                _movement.HotFixedUpdateSwitchState(PlayerMovementType.Air);
                return;
            }
            
            float acceleration = _horizontalAcceleration * _movement.ClampedMovementInput.x;

            float newHVelocity;
            if (_movement.ClampedMovementInput.x == 0) // Deceleration
            {
                if (_rb.velocity.x == 0) // End of decel
                {
                    newHVelocity = 0;
                }
                else if (_rb.velocity.x > 0) // Pos decel
                {
                    newHVelocity = _rb.velocity.x - _horizontalAcceleration * Time.deltaTime;
                    if (newHVelocity < 0) newHVelocity = 0;
                }
                else // Neg decel
                {
                    newHVelocity = _rb.velocity.x + _horizontalAcceleration * Time.deltaTime;
                    if (newHVelocity > 0) newHVelocity = 0;
                }
            }
            else // Acceleration
                newHVelocity = _rb.velocity.x + acceleration * Time.deltaTime;


            // Clamp Velocity (see horizontalMaxVelocity)
            if (newHVelocity > _maxHorizontalVelocity) newHVelocity = _maxHorizontalVelocity;
            else if (newHVelocity < -_maxHorizontalVelocity) newHVelocity = -_maxHorizontalVelocity;

            // Set Velocity
            _rb.velocity = new Vector3(newHVelocity, _rb.velocity.y, 0);

            // Step detection
            StepCheck();

            _movement.FixVerticalPenetration();
            _movement.FixHorizontalPenetration();
        }

        private bool StepCheck()
        {
            if (_movement.ClampedMovementInput.x == 0) return false;

            float halfRayLength = 0.1f;

            Vector3 bottomRay = new Vector3(
                _collider.bounds.center.x + (_collider.bounds.extents.x - halfRayLength) * Mathf.Sign(_movement.ClampedMovementInput.x),
                _collider.bounds.center.y - _collider.bounds.extents.y,
                _collider.bounds.center.z
                );

            Vector3 upRay = new Vector3(
                _collider.bounds.center.x + (_collider.bounds.extents.x - halfRayLength) * Mathf.Sign(_movement.ClampedMovementInput.x),
                _collider.bounds.center.y - (_collider.bounds.extents.y - 0.1f),
                _collider.bounds.center.z
                );

            bool hasHit1 = Physics.Raycast(bottomRay, Vector3.right * Mathf.Sign(_movement.ClampedMovementInput.x), halfRayLength + Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime, _groundMask);

            if (hasHit1)
            {
                bool hasHit2 = Physics.Raycast(upRay, Vector3.right * Mathf.Sign(_movement.ClampedMovementInput.x), halfRayLength + Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime + 0.1f, _groundMask);

                if (!hasHit2)
                {
                    RaycastHit hit;
                    Physics.Raycast(upRay + Vector3.right * Mathf.Sign(_movement.ClampedMovementInput.x) * (halfRayLength + Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime + 0.1f), Vector3.down, out hit, 0.15f, _groundMask);
                    
                    _movement.transform.position += Vector3.up * (0.1f - hit.distance);

                    return true;
                }

            }

            return false;
        }


        private void JumpDown()
        {
            Collider[] colls = Physics.OverlapBox(_collider.bounds.center - Vector3.up * 0.1f, _collider.bounds.extents, Quaternion.identity, LayerMask.GetMask("PassthroughPlatform"));

            foreach (Collider col in colls)
            {
                if (col.TryGetComponent(out PassthroughPlatform platform))
                {
                    platform.IgnorePlayerForSeconds(0.5f);
                }
            }
        }

        public override void OnJumpInput()
        {
            base.OnJumpInput();

            if (_movement.ClampedMovementInput != Vector2.down) _movement.TryJump();
            else JumpDown();
        }

        public override void OnLianeInput()
        {
            base.OnLianeInput();

            if (_movement.IsOnSolidGround) return;

            _movement.TryLaunchLiane();
        }
    }
}