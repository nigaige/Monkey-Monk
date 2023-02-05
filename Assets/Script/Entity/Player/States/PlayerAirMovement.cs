using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerAirMovement : PlayerState
    {
        private Rigidbody _rb;
        private float _airHorizontalAcceleration;
        private float _maxHorizontalVelocity;

        private float _gravityMultiplier;
        private float _fallMultiplier;
        private float _lowJumpMultiplier;
        private float _maxFallVelocity;

        public PlayerAirMovement(PlayerMovement movement, Rigidbody rb, float airHorizontalAcceleration, float maxHorizontalVelocity, float gravityMultiplier, float fallMultiplier, float lowJumpMultiplier, float maxFallVelocity) : base(movement)
        {
            _rb = rb;
            _airHorizontalAcceleration = airHorizontalAcceleration;
            _maxHorizontalVelocity = maxHorizontalVelocity;
            _gravityMultiplier = gravityMultiplier;
            _fallMultiplier = fallMultiplier;
            _lowJumpMultiplier = lowJumpMultiplier;
            _maxFallVelocity = maxFallVelocity;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _movement.GroundCheck();
            _movement.WallCheck();

            if (_movement.HangCheck())
            {
                _movement.HotFixedUpdateSwitchState(PlayerMovementType.Hang);
                return;
            }

            if (_movement.IsOnGround)
            {
                _movement.HotFixedUpdateSwitchState(PlayerMovementType.Ground);
                return;
            }

            FallMovement();

            float acceleration = _airHorizontalAcceleration * _movement.ClampedMovementInput.x;

            float newHVelocity;
            if (_movement.ClampedMovementInput.x == 0) // Deceleration
            {
                if (_rb.velocity.x == 0) // End of decel
                {
                    newHVelocity = 0;
                }
                else if (_rb.velocity.x > 0) // Pos decel
                {
                    newHVelocity = _rb.velocity.x - _airHorizontalAcceleration * Time.deltaTime;
                    if (newHVelocity < 0) newHVelocity = 0;
                }
                else // Neg decel
                {
                    newHVelocity = _rb.velocity.x + _airHorizontalAcceleration * Time.deltaTime;
                    if (newHVelocity > 0) newHVelocity = 0;
                }
            }
            else // Acceleration
                newHVelocity = _rb.velocity.x + acceleration * Time.deltaTime;


            // Clamp Velocity (see horizontalMaxVelocity) except if already over the threshold (used to keep liane propulsion inertia)
            if (newHVelocity > _maxHorizontalVelocity)
            {
                if (_rb.velocity.x <= _maxHorizontalVelocity) newHVelocity = _maxHorizontalVelocity;
                else if (newHVelocity > _rb.velocity.x) newHVelocity = _rb.velocity.x;
            }
            else if (newHVelocity < -_maxHorizontalVelocity)
            {
                if (_rb.velocity.x >= -_maxHorizontalVelocity) newHVelocity = -_maxHorizontalVelocity;
                else if (newHVelocity < _rb.velocity.x) newHVelocity = _rb.velocity.x;
            }

            // Set Velocity
            _rb.velocity = new Vector3(newHVelocity, _rb.velocity.y, 0);

            _movement.FixVerticalPenetration();
            _movement.FixHorizontalPenetration();
        }

        private void FallMovement()
        {
            // Global Gravity Multiplier
            _rb.velocity += Vector3.up * (Physics.gravity.y * (_gravityMultiplier - 1) * Time.deltaTime);

            if (_rb.velocity.y < 0) // Fall multiplier
            {
                _rb.velocity += Vector3.up * (Physics.gravity.y * (_fallMultiplier - 1) * Time.deltaTime);
            }
            else if (_rb.velocity.y > 0 && !_movement.JumpInput) // LowJump multiplier
            {
                _rb.velocity += Vector3.up * (Physics.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime);
            }

            // Clamp fall velocity
            if (_rb.velocity.y < -_maxFallVelocity) _rb.velocity = new Vector3(_rb.velocity.x, -_maxFallVelocity, 0);
        }

        public override void OnJumpInput()
        {
            base.OnJumpInput();

            _movement.TryJump(); // Can jump if coyote
        }

        public override void OnLianeInput()
        {
            base.OnLianeInput();

            if (_movement.IsTouchingWall) return;

            _movement.TryLaunchLiane();
        }
    }
}