using MonkeyMonk.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerHangMovement : PlayerState
    {
        private Rigidbody _rb;
        private Collider _collider;

        private LayerMask _hangMask;
        private float _hangSpeed;

        public PlayerHangMovement(PlayerMovement movement, Rigidbody rb, Collider collider, LayerMask hangMask, float hangSpeed) : base(movement)
        {
            _rb = rb;
            _collider = collider;

            _hangMask = hangMask;
            _hangSpeed = hangSpeed;
        }

        public override void EnterState()
        {
            base.EnterState();

            _rb.velocity = Vector3.zero;
            _rb.useGravity = false;
            Debug.Log("Hang");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_movement.HangInput || !_movement.HangCheck())
            {
                _movement.ReevaluateMovementType();
                return;
            }

            if (_movement.TopHangCheck())
            {
                TopHangMovement();
                FixTopHangPosition();
            }

            _movement.FixVerticalPenetration();
            _movement.FixHorizontalPenetration();
        }

        private void TopHangMovement()
        {
            _rb.velocity = new Vector3(_movement.ClampedMovementInput.x * _hangSpeed, 0, 0);
        }

        private void FixTopHangPosition()
        {
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

            bool hasHit1 = Physics.Raycast(RayStart1, Vector3.up, out hit1, rayDist * 2f, _hangMask);
            bool hasHit2 = Physics.Raycast(RayStart2, Vector3.up, out hit2, rayDist * 2f, _hangMask);

            float dist;
            if (hasHit1 && hasHit2) dist = Mathf.Min(hit1.distance, hit2.distance);
            else if (hasHit1) dist = hit1.distance;
            else dist = hit2.distance;

            _movement.transform.position += Vector3.up * (dist - rayDist);
        }



        public override void ExitState()
        {
            base.ExitState();

            Debug.Log("Bop Hang");
            _rb.useGravity = true;
        }
    }
}