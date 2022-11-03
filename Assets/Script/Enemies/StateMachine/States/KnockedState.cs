using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class KnockedState : State
    {
        private Rigidbody _rb;

        public override void Initialize(EnemyStateMachine stateMachine)
        {
            base.Initialize(stateMachine);

            _rb = Entity.GetComponent<Rigidbody>();
        }

        public override void UpdateState()
        {
            if (Entity.IsGrounded)
            {
                if (Entity.IsKnocked) 
                    _rb.velocity = Vector2.zero;
                else 
                    ExitState();
            }

            base.UpdateState();
        }

    }
}