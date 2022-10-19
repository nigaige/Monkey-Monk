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
    }
}

