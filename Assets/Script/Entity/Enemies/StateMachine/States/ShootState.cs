using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class ShootState : State
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private float beforeShootDuration = 1.0f;
        [SerializeField] private float cooldown = 1.0f;

        private Coroutine _shootCoroutine;

        public override void EnterState()
        {
            base.EnterState();

            _shootCoroutine = StartCoroutine(Shoot());
        }

        private IEnumerator Shoot()
        {
            yield return new WaitForSeconds(beforeShootDuration);

            // Shoot
            Projectile proj = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(Entity.LookDirection * Vector2.right, Entity.gameObject);

            yield return new WaitForSeconds(cooldown);

            ExitState();
        }

        public override void ExitState()
        {
            base.ExitState();

            if(_shootCoroutine != null)
            {
                StopCoroutine(_shootCoroutine);
                _shootCoroutine = null;
            }
        }
    }
}