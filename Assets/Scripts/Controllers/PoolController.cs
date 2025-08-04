using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PoolController
    {
        private List<GameObject> _enemiesPrefabs;
        private List<IEnemy> _enemyPool;
        private List<IEnemy> _enemiesOnField;
        private int _enemyCount = 0;
        public List<IEnemy> EnemiesOnField => _enemiesOnField;

        public int OnField => _enemiesOnField.Count;

        public PoolController(List<GameObject> enemiesPrefabs)
        {
            _enemiesPrefabs = enemiesPrefabs;
            _enemyPool = new List<IEnemy>();
            _enemiesOnField = new List<IEnemy>();
        }

        public IEnemy GetEnemy(Type enemyType)
        {
            IEnemy enemy = _enemyPool.FirstOrDefault(x => x.GetType().Equals(enemyType) && x.IsDead);
            if (enemy != null)
            {
                _enemyPool.Remove(enemy);
                enemy.Renew();
            }

            if (enemy == null)
            {
                var prefab = _enemiesPrefabs.FirstOrDefault(x => x.GetComponent<IEnemy>().GetType().Equals(enemyType));
                enemy = UnityEngine.Object.Instantiate(prefab,
                    new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)),
                    Quaternion.identity
                    ).GetComponent<IEnemy>();
            }
            enemy.name = enemy.GetType().ToString() + _enemyCount++;
            enemy.transform.parent = null;
            _enemiesOnField.Add(enemy);
            return enemy;
        }

        public void PutEnemy(IEnemy enemy)
        {
            enemy.Died = null;
            enemy.CauseDamage = null;
            _enemiesOnField.Remove(enemy);
            _enemyPool.Add(enemy);
        }

        public void ClearAll()
        {
            foreach (var enemy in _enemyPool)
            {
                MonoBehaviour.Destroy(enemy.transform.gameObject);

            }
            _enemyPool.Clear();
            foreach (var enemy in _enemiesOnField)
            {
                MonoBehaviour.Destroy(enemy.transform.gameObject);
            }
            _enemiesOnField.Clear();
        }
    }
}