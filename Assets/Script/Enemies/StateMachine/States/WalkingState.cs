using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MonkeyMonk.Enemies.StateMachine
{
    public class WalkingState : State
    {
        [SerializeField] private float speed = 1f;
        
        private int _walkingDirection = -1;

        private Rigidbody _rb;
        private Collider _collider;

        private LayerMask _cliffHitMaskLayer;
        private LayerMask _hitMaskLayer;

        public override void Initialize(EnemyStateMachine stateMachine)
        {
            base.Initialize(stateMachine);

            _rb = Entity.GetComponent<Rigidbody>();
            _collider = Entity.GetComponent<Collider>();

            _cliffHitMaskLayer = LayerMask.GetMask("Block");
            _hitMaskLayer = LayerMask.GetMask("Block", "Enemy");
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void FixedUpdateState()
        {
            // Check if near cliff
            bool IsNearCliff = Physics.Raycast(Entity.transform.position + new Vector3(_walkingDirection * _collider.bounds.extents.x, -_collider.bounds.extents.y, 0) + new Vector3(_walkingDirection * 0.1f, 0.1f, 0), Vector3.down, 0.2f, _cliffHitMaskLayer);

            bool IsNearWall = Physics.Raycast(Entity.transform.position + new Vector3(_walkingDirection * (_collider.bounds.extents.x - 0.05f), 0, 0), Vector3.right * _walkingDirection, 0.1f, _hitMaskLayer);

            // Switch dir if near cliff
            if(!IsNearCliff && Entity.IsGrounded)
            {
                _walkingDirection *= -1;
            }

            // Switch dir if near cliff
            if (IsNearWall && Entity.IsGrounded)
            {
                _walkingDirection *= -1;
            }

            _rb.velocity = new Vector2(_walkingDirection * speed, _rb.velocity.y);

            base.FixedUpdateState();
        }

    }
}