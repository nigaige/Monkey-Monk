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
        [SerializeField] private bool isStartDirectionRight;

        public EnemyStateMachine StateMachine { get => enemyStateMachine; }
        public bool IsKnocked { get => _isKnockedLink.Value; private set => _isKnockedLink.Value = value; }
        public bool IsGrounded { get; private set; }
        public bool IsJumpable { get => isJumpable; }
        public int LookDirection { get; private set; } = 1;

        [SerializeField] private LinkedVariable<bool> _isKnockedLink;

        [SerializeField] private LayerMask groundMask;

        private Collider _collider;

        public event Action OnDestroyEvent;



        protected override void Awake()
        {
            base.Awake();

            ChangeDirection(isStartDirectionRight ? 1 : -1);

            enemyStateMachine.Initialize(this);

            _isKnockedLink.Init(enemyStateMachine);

            _collider = GetComponent<Collider>();
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
            float raySize = 0.06f;


            Debug.DrawRay(transform.position + new Vector3(0, -_collider.bounds.extents.y + 0.05f), Vector3.down * raySize);

            IsGrounded = Physics.Raycast(new Ray(transform.position + new Vector3(0, -_collider.bounds.extents.y + 0.05f), Vector3.down), raySize, groundMask)
                    || Physics.Raycast(new Ray(transform.position + new Vector3(-_collider.bounds.extents.x, -_collider.bounds.extents.y + 0.05f), Vector3.down), raySize, groundMask)
                    || Physics.Raycast(new Ray(transform.position + new Vector3(_collider.bounds.extents.x, -_collider.bounds.extents.y + 0.05f), Vector3.down), raySize, groundMask);
        }



        float CastARay(Vector3 pos, Vector3 dir, float length, LayerMask mask)
        {
            RaycastHit Hit;

            Debug.DrawRay(pos, dir * length);
            bool hitPlateform = Physics.Raycast(pos, transform.TransformDirection(dir), out Hit, length, mask);

            if (hitPlateform) return Hit.distance;
            return -1;
        }
        private void OnDestroy()
        {
            OnDestroyEvent?.Invoke();
        }
    }
}
