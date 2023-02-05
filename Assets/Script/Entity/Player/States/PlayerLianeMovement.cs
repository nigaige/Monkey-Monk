using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerLianeMovement : PlayerState
    {
        private Rigidbody _rb;
        private Liane _liane;
        private float _gravityMultiplier;
        private float _lianeHorizontalSpeed;
        private float _lianeSpeed;
        private float _lianeMaxAngle;
        private float _lianeVerticalSpeed;

        private float _angle;
        private float _angleAcceleration;
        private float _angleVelocity;

        public PlayerLianeMovement(PlayerMovement movement, Rigidbody rb, Liane liane, float gravityMultiplier, float lianeHorizontalSpeed, float lianeSpeed, float lianeMaxAngle, float lianeVerticalSpeed) : base(movement)
        {
            _rb = rb;
            _liane = liane;

            _gravityMultiplier = gravityMultiplier;
            _lianeHorizontalSpeed = lianeHorizontalSpeed;
            _lianeSpeed = lianeSpeed;
            _lianeMaxAngle = lianeMaxAngle;
            _lianeVerticalSpeed = lianeVerticalSpeed;
        }

        public override void EnterState()
        {
            base.EnterState();

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PassthroughPlatform"), true);

            // TEST : Set default liane angle velocity
            Vector3 playerLianeDir = PerpendicularClockwise(_liane.GetLianeDir().normalized);

            //Debug.DrawLine(transform.position, transform.position + playerLianeDir, Color.red, 1f);
            float vel = Vector3.Dot(_rb.velocity, playerLianeDir);


            // Set default liane angle velocity
            //float vel = _rb.velocity.x;
            _angleVelocity = vel / _liane.GetLianeLength();

            // Reset linear velocity
            _rb.velocity = Vector3.zero;

            // Set default angle
            _angle = Mathf.Deg2Rad * Vector3.SignedAngle(Vector3.down, -_liane.GetLianeDir().normalized, Vector3.forward);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_movement.HangCheck())
            {
                _movement.HotFixedUpdateSwitchState(PlayerMovementType.Hang);
                return;
            }

            // Calculate angle
            _angleAcceleration = Physics.gravity.y * (_gravityMultiplier - 1) * Mathf.Sin(_angle) /*/ liane.GetLianeLength()*/;
            //Debug.DrawRay(transform.position, Vector3.Cross(-liane.GetLianeDir().normalized, -Vector3.forward) * _angleAcceleration, Color.red);

            // Accel / Decel
            if (_movement.ClampedMovementInput.x != 0)
            {
                if (Mathf.Sign(_angleVelocity) == Mathf.Sign(_movement.ClampedMovementInput.x)) // acceleration
                {
                    float newPredAngleAccel = _angleAcceleration + Mathf.Sign(_angleVelocity) * _lianeHorizontalSpeed * Mathf.Abs(Mathf.Cos(_angle));
                    float newPredAngleVel = _angleVelocity + newPredAngleAccel * Time.deltaTime * _lianeSpeed;

                    float higherA = GetPredictedHigherAngle(newPredAngleAccel, newPredAngleVel, _angle + newPredAngleVel * _lianeSpeed * Time.deltaTime);

                    if (higherA < Mathf.Deg2Rad * _lianeMaxAngle) _angleAcceleration = newPredAngleAccel;
                }
                else if (Mathf.Sign(_angleVelocity) != Mathf.Sign(_movement.ClampedMovementInput.x)) // deceleration
                {
                    _angleAcceleration -= Mathf.Sign(_angleVelocity) * _lianeHorizontalSpeed * Mathf.Abs(Mathf.Cos(_angle));
                }
            }

            float newAngleVel = _angleVelocity + _angleAcceleration * Time.deltaTime * _lianeSpeed;

            _angleVelocity = newAngleVel;

            if (_movement.ClampedMovementInput.x == 0) _angleVelocity *= 0.995f; // Loss

            _angle += _angleVelocity * _lianeSpeed * Time.deltaTime;

            // Move to next pos
            Vector3 target = _liane.LianePosition + _liane.GetLianeLength() * new Vector3(Mathf.Sin(_angle), -Mathf.Cos(_angle), 0);
            _rb.velocity = (target - _rb.position) / Time.deltaTime;

            // Vertical Liane Movement
            if (_movement.ClampedMovementInput == Vector2.up || _movement.ClampedMovementInput == Vector2.down)
            {
                if (_liane.GetLianeLength() - _movement.ClampedMovementInput.y * _lianeVerticalSpeed * Time.deltaTime > _liane.MaxLength) // end of the rope
                {
                    _rb.velocity -= (_liane.LianePosition - _movement.transform.position).normalized * (_liane.MaxLength - _liane.GetLianeLength());
                    _liane.SetLianeLength(_liane.MaxLength);
                }
                else
                {
                    _rb.velocity += _movement.ClampedMovementInput.y * (_liane.LianePosition - _movement.transform.position).normalized * _lianeVerticalSpeed;
                    _liane.SetLianeLength(_liane.GetLianeLength() - _movement.ClampedMovementInput.y * _lianeVerticalSpeed * Time.deltaTime);
                }
            }
        }

        public override void ExitState()
        {
            base.ExitState();

            _liane.Release();
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PassthroughPlatform"), false);
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
                float next_angle_accel = Physics.gravity.y * (_gravityMultiplier - 1) * Mathf.Sin(curr_angle) / _liane.GetLianeLength();
                float next_angle_vel = curr_angle_vel + curr_angle_accel * Time.fixedDeltaTime * _lianeSpeed;
                float next_angle = curr_angle + next_angle_vel * _lianeSpeed * Time.fixedDeltaTime;

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

        public Vector3 PerpendicularClockwise(Vector3 vect)
        {
            return new Vector3(vect.y, -vect.x, 0);
        }

        public override void OnLianeInput()
        {
            base.OnLianeInput();

            _movement.SwitchState(PlayerMovementType.Air);
        }

        public override void OnJumpInput()
        {
            base.OnJumpInput();

            _movement.SwitchState(PlayerMovementType.Air);
        }
    }
}