using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MonkeyMonk.Player
{
    public class Player : Entity
    {
        protected override void OnDeath()
        {
            base.OnDeath();
            SceneMaster.Instance.LoadHubWorld();
        }

    }
}