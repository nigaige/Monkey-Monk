using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerState
    {
        protected PlayerMovement _movement;

        public PlayerState(PlayerMovement movement)
        {
            _movement = movement;
        }

        public virtual void EnterState()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void ExitState()
        {

        }

        public virtual void OnJumpInput()
        {

        }

        public virtual void OnLianeInput()
        {

        }
    }
}