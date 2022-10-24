using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MonkeyMonk.Enemies.StateMachine
{
    public class WalkingState : State
    {
        [SerializeField] private float speed = 1f;

        private int _walkingDirection = -1;

        private Rigidbody2D _rb2d;
        private Collider2D _collider;

        private void Start()
        {
            _rb2d = Entity.GetComponent<Rigidbody2D>();
            _collider = Entity.GetComponent<Collider2D>();
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void UpdateState()
        {
            // Check if near cliff
            RaycastHit2D hit = Physics2D.Raycast(Entity.transform.position + new Vector3(_walkingDirection * _collider.bounds.extents.x, -_collider.bounds.extents.y) + new Vector3(_walkingDirection * 0.1f, 0.1f), Vector2.down, 0.2f);

            bool isGrounded = _collider.Cast(Vector2.down, new RaycastHit2D[1], 0.05f) > 0;

            // Switch dir if near cliff
            if(!hit && isGrounded)
            {
                _walkingDirection *= -1;
            }


            _rb2d.velocity = new Vector2(_walkingDirection * speed, _rb2d.velocity.y);


            base.UpdateState();
        }

    }
}