
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerClimbMovement : PlayerState
    {
        private Rigidbody _rb;
        private Collider _collider;
        public PlayerClimbMovement(PlayerMovement movement, Rigidbody rb, Collider collider) : base(movement)
        {
            _rb = rb;
            _collider = collider;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _movement.GroundCheck();
            _movement.WallCheck();
            _movement.CanClimbCheck();

            if (_movement.CanClimb)
            {
                Debug.Log("Climbing");
            }
        }
    }
}

