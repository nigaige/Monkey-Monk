using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MonkeyMonk.Player
{
    public class Player : Entity
    {
        public PlayerMovement PlayerMovement { get => playerMovement; }

        [SerializeField] private PlayerMovement playerMovement;

        protected override void OnDamage()
        {
            base.OnDamage();
            if (playerMovement.CurrentMovementType == PlayerMovementType.Liane) playerMovement.ReevaluateMovementType();
        }

        protected override void OnDeath()
        {
            gameObject.SetActive(false);
            base.OnDeath();
        }

        public override void Reset()
        {
            base.Reset();
            playerMovement.Reset();
            playerMovement.ReevaluateMovementType();
        }

    }
}