using MonkeyMonk.Enemies.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine enemyStateMachine;

        [SerializeField] private bool isJumpable = false;

        public EnemyStateMachine StateMachine { get => enemyStateMachine; }
        public bool IsKnocked { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsJumpable { get => isJumpable; }
        public int LookDirection { get; private set; } = 1;

        private MonoBehaviour b;

        private Collider _collider;

        private LayerMask _groundMask;
        private float _knockTimeLeft;

        public event Action OnDestroyEvent;


        private void Awake()
        {

            enemyStateMachine.Initialize(this);

            _collider = GetComponent<Collider>();
            _groundMask = LayerMask.GetMask("Block");
        }

        public void Knock()
        {
            IsKnocked = true;
            _knockTimeLeft = 2.0f;

            enemyStateMachine.Knock();
        }

        public void ChangeDirection(int dir)
        {
            LookDirection = dir;
        }


        private void Update()
        {
            if (IsKnocked && IsGrounded)
            {
                if (_knockTimeLeft > 0) _knockTimeLeft -= Time.deltaTime;
                if (_knockTimeLeft <= 0) IsKnocked = false;
            }
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
