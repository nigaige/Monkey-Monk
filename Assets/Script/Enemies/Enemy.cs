using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class Enemy : MonoBehaviour
    {
        private Action onDestroyEvent;







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
