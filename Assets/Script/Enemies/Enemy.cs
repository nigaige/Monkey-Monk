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


        private Collider2D _collider;

        private ContactFilter2D _groundFilter;
        private float _knockTimeLeft;

        private Action onDestroyEvent;


        private void Awake()
        {
            enemyStateMachine.Initialize(this);

            _collider = GetComponent<Collider2D>();
            _groundFilter.SetLayerMask(LayerMask.GetMask("Block"));
        }

        public void Knock()
        {
            IsKnocked = true;
            _knockTimeLeft = 2.0f;

            enemyStateMachine.Knock();
        }


        private void Update()
        {
            IsGrounded = _collider.Cast(Vector2.down, _groundFilter, new RaycastHit2D[1], 0.05f) > 0;

            if (IsKnocked && IsGrounded)
            {
                if (_knockTimeLeft > 0) _knockTimeLeft -= Time.deltaTime;
                if (_knockTimeLeft <= 0) IsKnocked = false;
            }
        }

        #region === Events

        public void AddOnDestroyListener(Action action)
        {
            onDestroyEvent += action;
        }

        public void RemoveOnDestroyListener(Action action)
        {
            onDestroyEvent -= action;
        }

        #endregion

        private void OnDestroy()
        {
            onDestroyEvent?.Invoke();
        }
    }
}
