using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class EnemyDamageBox : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        private Collider2D _collider;
        private Rigidbody2D _rb2d;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _rb2d = _collider.attachedRigidbody;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (enemy.IsKnocked) return;

            if (collision.tag != "Player" || enemy.IsKnocked) return;


            Rigidbody2D otherRb = collision.attachedRigidbody;
            if (enemy.IsJumpable && otherRb.velocity.y < 0)
            {
                // Check if player was on top of enemy

                float y1 = collision.bounds.center.y - collision.bounds.extents.y;
                float y2 = _collider.bounds.center.y + _collider.bounds.extents.y;

                float last_y1 = y1 - otherRb.velocity.y * Time.fixedDeltaTime;
                float last_y2 = y2 - _rb2d.velocity.y * Time.fixedDeltaTime;

                if (last_y1 >= last_y2)
                {
                    Jumped(collision);
                    return;
                }
            }

            // Else

            Hit(collision);
        }

        private void Jumped(Collider2D collision)
        {
            Debug.Log("Enemy Death - " + collision.gameObject);
        }

        private void Hit(Collider2D collision)
        {
            // Damage player
            Debug.Log("Hit Player - " + collision.gameObject);
        }
    }
}