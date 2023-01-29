using MonkeyMonk.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class EnemyDamageBox : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        [SerializeField] private Vector3 collisionExtends;
        [SerializeField] private LayerMask mask;

        private Collider[] _collisions = new Collider[10];

        private void FixedUpdate()
        {
            if (enemy.IsKnocked) return;

            int n = Physics.OverlapBoxNonAlloc(transform.position, collisionExtends, _collisions, Quaternion.identity, mask);

            for (int i = 0; i < n; i++)
            {
                if (_collisions[i].TryGetComponent(out Player.Player player)) player.Damage(1);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, collisionExtends * 2);
        }
#endif
    }
}