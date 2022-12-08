using MonkeyMonk.Enemies.StateMachine;
using MonkeyMonk.Enemies.StateMachine.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class Enemy : Entity
    {
        [SerializeField] private EnemyStateMachine enemyStateMachine;

        [SerializeField] private bool isJumpable = false;

        public EnemyStateMachine StateMachine { get => enemyStateMachine; }
        public bool IsKnocked { get => _isKnockedLink.Value; private set => _isKnockedLink.Value = value; }
        public bool IsGrounded { get; private set; }
        public bool IsJumpable { get => isJumpable; }
        public int LookDirection { get; private set; } = 1;

        [SerializeField] private LinkedVariable<bool> _isKnockedLink;

        private Collider _collider;

        private LayerMask _groundMask;

        public event Action OnDestroyEvent;



        protected override void Awake()
        {
            base.Awake();

            enemyStateMachine.Initialize(this);

            _isKnockedLink.Init(enemyStateMachine);

            _collider = GetComponent<Collider>();
            _groundMask = LayerMask.GetMask("Block");
        }

        public void Knock()
        {
            enemyStateMachine.Knock();
        }

        public void ChangeDirection(int dir)
        {
            LookDirection = dir;
        }

        private void FixedUpdate()
        {
            IsGrounded = Physics.Raycast(new Ray(transform.position + new Vector3(0, -_collider.bounds.extents.y + 0.05f), Vector3.down), 0.1f, _groundMask)
                    || Physics.Raycast(new Ray(transform.position + new Vector3(-_collider.bounds.extents.x, -_collider.bounds.extents.y + 0.05f), Vector3.down), 0.1f, _groundMask)
                    || Physics.Raycast(new Ray(transform.position + new Vector3(_collider.bounds.extents.x, -_collider.bounds.extents.y + 0.05f), Vector3.down), 0.1f, _groundMask);
        }

        private void OnDestroy()
        {
            OnDestroyEvent?.Invoke();
        }
    }
}
