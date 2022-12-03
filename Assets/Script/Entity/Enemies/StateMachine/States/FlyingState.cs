using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class FlyingState : State
    {
        [SerializeField] private List<Vector2> path;
        [SerializeField] private bool looping;
        [SerializeField] private float speed;

        private bool _direction = true;
        private int _currentIndex;
        private Vector2 _startingPosition;
        private Rigidbody _rb;

        private void Start()
        {
            _startingPosition = transform.position;
            _rb = Entity.GetComponent<Rigidbody>();
        }

        public override void UpdateState()
        {
            // Move
            Vector2 dir = (_startingPosition + path[_currentIndex]) - (Vector2)Entity.transform.position;
            _rb.velocity = dir.normalized * speed;

            // Check next target
            if (Vector2.Distance(_startingPosition + path[_currentIndex], (Vector2)Entity.transform.position) < 0.01f)
            {
                GetNextPosition();
            }

            base.UpdateState();
        }

        // Change the target to the next one
        private Vector2 GetNextPosition()
        {
            if (looping)
            {
                if (++_currentIndex >= path.Count) _currentIndex = 0;
            }
            else
            {
                if (_direction)
                {
                    if (_currentIndex + 1 >= path.Count)
                    {
                        _direction = false;
                        _currentIndex--;
                    }
                    else _currentIndex++;
                }
                else
                {
                    if (_currentIndex <= 0)
                    {
                        _direction = true;
                        _currentIndex++;
                    }
                    else _currentIndex--;
                }
            }
            
            return _startingPosition + path[_currentIndex];
        }


        // Path Editor Preview
        private void OnDrawGizmosSelected()
        {
            Vector2 startPos;
            if (Application.isPlaying) startPos = _startingPosition;
            else startPos = transform.position;

            if (path == null || path.Count <= 1) return;

            for (int i = 1; i < path.Count; i++)
            {
                Gizmos.DrawLine(startPos + path[i - 1], startPos + path[i]);
            }
            if (looping) Gizmos.DrawLine(startPos + path[0], startPos + path[path.Count - 1]);
        }
    }
}