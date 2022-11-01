using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttack : MonoBehaviour
{
    [SerializeField] private TEST_PlayerMovement player;

    private Collider2D _collider;
    private Rigidbody2D _rb2d;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rb2d = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Enemy") return;

        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy == null || enemy.IsKnocked) return;

        if (_rb2d.velocity.y < 0 && enemy.IsJumpable)
        {
            // Check if player was on top of enemy

            Rigidbody2D otherRb = collision.attachedRigidbody;

            float y1 = _collider.bounds.center.y - _collider.bounds.extents.y;
            float y2 = collision.bounds.center.y + collision.bounds.extents.y;

            float last_y1 = y1 - _rb2d.velocity.y * Time.fixedDeltaTime;
            float last_y2 = y2 - otherRb.velocity.y * Time.fixedDeltaTime;

            if (last_y1 >= last_y2)
            {
                Debug.Log("Rebound");
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * 7;
                collision.GetComponent<Enemy>().Knock();
            }
        }
    }

}
