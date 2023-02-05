using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Player
{
    public class PlayerJumpAttack : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private float reboundForce;

        private Collider _collider;
        private Rigidbody _rb;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _rb = player.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Collider[] cs = Physics.OverlapBox(_collider.bounds.center + Vector3.down * _collider.bounds.extents.y + Vector3.up * _rb.velocity.y * Time.fixedDeltaTime / 2, new Vector3(_collider.bounds.extents.x, -1 * _rb.velocity.y * Time.fixedDeltaTime / 2, _collider.bounds.extents.z));
            foreach (var item in cs)
            {
                CheckRebound(item);
            }
        }

        private void CheckRebound(Collider other)
        {
            if (other.tag != "Enemy") return;

            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy == null || enemy.IsKnocked) return;

            if (_rb.velocity.y < 0 && enemy.IsJumpable)
            {
                // Check if player was on top of enemy

                Rigidbody otherRb = other.attachedRigidbody;

                float y1 = _collider.bounds.center.y - _collider.bounds.extents.y;
                float y2 = other.bounds.center.y + other.bounds.extents.y;

                float last_y1 = y1 - _rb.velocity.y * Time.fixedDeltaTime;
                float last_y2 = y2 - otherRb.velocity.y * Time.fixedDeltaTime;

                if (last_y1 >= last_y2)
                {
                    if (player.PlayerMovement.CurrentMovementType == PlayerMovementType.Liane)
                        player.PlayerMovement.ReevaluateMovementType();

                    _rb.velocity = new Vector3(_rb.velocity.x, reboundForce, 0);
                    enemy.Knock();
                }
            }
        }

    }
}