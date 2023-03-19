
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerClimbMovement : PlayerState
    {
        private Rigidbody _rb;
        private Collider _collider;
        private float _acceleration;
        private float _maxVelocity;
        public PlayerClimbMovement(PlayerMovement movement, Rigidbody rb, Collider collider, float acceleration, float maxVelocity) : base(movement)
        {
            _rb = rb;
            _collider = collider;
            _acceleration = acceleration;
            _maxVelocity = maxVelocity;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _movement.GroundCheck();
            _movement.WallCheck();
            _movement.CanClimbCheck();

            if (!_movement.CanClimb)
            {
                _movement.HotFixedUpdateSwitchState(PlayerMovementType.Air);
                return;
            }

            // Horizontal movement
            float hAcceleration = _acceleration * _movement.ClampedMovementInput.x;
            float newHVelocity;
            if (_movement.ClampedMovementInput.x == 0) // Deceleration
            {
                if (_rb.velocity.x == 0) // End of decel
                {
                    newHVelocity = 0;
                }
                else if (_rb.velocity.x > 0) // Pos decel
                {
                    newHVelocity = _rb.velocity.x - _acceleration * Time.deltaTime;
                    if (newHVelocity < 0) newHVelocity = 0;
                }
                else // Neg decel
                {
                    newHVelocity = _rb.velocity.x + _acceleration * Time.deltaTime;
                    if (newHVelocity > 0) newHVelocity = 0;
                }
            }
            else // Acceleration
            {
                newHVelocity = _rb.velocity.x + hAcceleration * Time.deltaTime;
            }

            // Clamp Velocity (see horizontalMaxVelocity)
            if (newHVelocity > _maxVelocity) newHVelocity = _maxVelocity;
            else if (newHVelocity < -_maxVelocity) newHVelocity = -_maxVelocity;

            // Vertical movement
            float vAcceleration = _acceleration * _movement.ClampedMovementInput.y;
            float newVVelocity;
            if (_movement.ClampedMovementInput.y == 0) // Deceleration
            {
                if (_rb.velocity.y == 0) // End of decel
                {
                    newVVelocity = 0;
                }
                else if (_rb.velocity.y > 0) // Pos decel
                {
                    newVVelocity = _rb.velocity.y - _acceleration * Time.deltaTime;
                    if (newVVelocity < 0) newVVelocity = 0;
                }
                else // Neg decel
                {
                    newVVelocity = _rb.velocity.y + _acceleration * Time.deltaTime;
                    if (newVVelocity > 0) newVVelocity = 0;
                }
            }
            else // Acceleration
            {
                Debug.Log("Vertical climb");
                newVVelocity = _rb.velocity.y + vAcceleration * Time.deltaTime;
            }

            // Clamp Velocity (see horizontalMaxVelocity)
            if (newVVelocity > _maxVelocity) newVVelocity = _maxVelocity;
            else if (newVVelocity < -_maxVelocity) newVVelocity = -_maxVelocity;

            // Set Velocity
            _rb.velocity = new Vector3(newHVelocity, newVVelocity, 0);

            // Step detection
            //StepCheck();

            _movement.FixVerticalPenetration();
            _movement.FixHorizontalPenetration();
        }
        public override void OnClimbInput()
        {
            base.OnClimbInput();
            _movement.TryClimbing();
        }
    }
}

