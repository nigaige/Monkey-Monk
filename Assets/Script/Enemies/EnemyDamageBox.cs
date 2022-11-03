using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class EnemyDamageBox : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player" || enemy.IsKnocked) return;

            Hit(other);
        }

        private void Hit(Collider other)
        {
            // Damage player
            Debug.Log("Hit Player - " + other.gameObject);
        }
    }
}