using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonkeyMonk.Enemies.StateMachine.Variables;


namespace MonkeyMonk.Enemies.StateMachine
{
    public class WalkingState : State
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private LinkedVariable<EntityWalkableZone> walkableZone;

        private int _walkingDirection = -1;

        private Rigidbody _rb;
        private Collider _collider;

        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask wallLayer;

        public override void Initialize(EnemyStateMachine stateMachine)
        {
            base.Initialize(stateMachine);

            _rb = Entity.GetComponent<Rigidbody>();
            _collider = Entity.GetComponent<Collider>();

            walkableZone.Init(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void FixedUpdateState()
        {
            if (walkableZone.Value != null) CheckZone();
            CheckLedge();

            _rb.velocity = new Vector2(_walkingDirection * speed, _rb.velocity.y);

            base.FixedUpdateState();
        }

        void CheckZone()
        {
            if (_walkingDirection < 0 && Entity.transform.position.x < walkableZone.Value.GetZones().x) _walkingDirection *= -1;
            if (_walkingDirection > 0 && Entity.transform.position.x > walkableZone.Value.GetZones().y) _walkingDirection *= -1;
        }

        void CheckLedge()
        {
            // Check if near cliff
            bool IsNearCliff = Physics.Raycast(Entity.transform.position + new Vector3(_walkingDirection * _collider.bounds.extents.x, -_collider.bounds.extents.y, 0) + new Vector3(_walkingDirection * 0.1f, 0.1f, 0), Vector3.down, 0.2f, groundLayer);

            bool IsNearWall = Physics.Raycast(Entity.transform.position + new Vector3(_walkingDirection * (_collider.bounds.extents.x - 0.05f), 0, 0), Vector3.right * _walkingDirection, 0.1f, wallLayer);

            // Switch dir if near cliff
            if (!IsNearCliff && Entity.IsGrounded)
            {
                _walkingDirection *= -1;
            }

            // Switch dir if near cliff
            if (IsNearWall && Entity.IsGrounded)
            {
                _walkingDirection *= -1;
            }
        }

    }
}