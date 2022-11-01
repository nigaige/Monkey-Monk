using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class KnockedState : State
    {
        private Rigidbody2D _rb2d;

        private void Start()
        {
            _rb2d = Entity.GetComponent<Rigidbody2D>();
        }

        public override void UpdateState()
        {
            if (Entity.IsGrounded)
            {
                if (Entity.IsKnocked) 
                    _rb2d.velocity = Vector2.zero;
                else 
                    ExitState();
            }

            base.UpdateState();
        }

    }
}