using MonkeyMonk.Enemies.StateMachine.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class KnockedState : State
    {
        [SerializeField] LinkedVariable<bool> isKnockedLinked;

        private Rigidbody _rb;

        public override void Initialize(EnemyStateMachine stateMachine)
        {
            base.Initialize(stateMachine);

            _rb = Entity.GetComponent<Rigidbody>();
            isKnockedLinked.Init(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();

            isKnockedLinked.Value = true;
            StartCoroutine(KnockedCooldownCoroutine());
        }

        public IEnumerator KnockedCooldownCoroutine()
        {
            float _knockTimeLeft = 2.0f;

            while (_knockTimeLeft > 0)
            {
                if (Entity.IsGrounded) _knockTimeLeft -= Time.deltaTime;
                yield return null;
            }
            
            ExitState();
        }

        public override void UpdateState()
        {
            if (Entity.IsGrounded) _rb.velocity = Vector2.zero;

            base.UpdateState();
        }

        public override void ExitState()
        {
            isKnockedLinked.Value = false;

            base.ExitState();
        }

    }
}