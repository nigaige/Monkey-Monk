using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float respawnDelay = 5.0f;

        private Enemy _spawnEnemy;

        void Start()
        {
            RespawnEnemy();
        }

        private void Update()
        {
            
        }

        void RespawnEvent()
        {
            Invoke(nameof(RespawnEnemy), respawnDelay);
        }

        void RespawnEnemy()
        {
            _spawnEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity).GetComponent<Enemy>();
            _spawnEnemy.AddOnDestroyListener(RespawnEvent);
        }



        private void OnDestroy()
        {
            if (_spawnEnemy != null) _spawnEnemy.RemoveOnDestroyListener(RespawnEvent);
        }

        // Preview in editor
        private void OnDrawGizmosSelected()
        {
            if (enemyPrefab == null) return;

            if (enemyPrefab.TryGetComponent(out MeshFilter filter)) {
                Gizmos.DrawMesh(filter.sharedMesh, transform.position, transform.rotation, enemyPrefab.transform.localScale);
            }
        }
    }
}

