using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonkeyMonk.Enemies.StateMachine.Variables;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class FlyingKnockedState : State
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
            yield return new WaitForSeconds(2f);

            ExitState();
        }

        public override void UpdateState()
        {
            _rb.velocity = Vector2.zero;

            base.UpdateState();
        }

        public override void ExitState()
        {
            isKnockedLinked.Value = false;

            base.ExitState();
        }

    }
}