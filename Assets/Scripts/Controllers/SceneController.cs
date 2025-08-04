using Assets.Scripts.Actions;
using Assets.Scripts.Controllers.Characters;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Scripts.Controllers
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _enemiesPrefabs;
        [SerializeField] private Transform _poolTransform;
        [SerializeField] private Player _player;
        [SerializeField] private LevelConfig _config;
        private PoolController _enemyPool;
        private int _currWave = 0;

        public static List<IEnemy> EnemiesOnfield { get; private set; }

        private void Awake()
        {
            _enemyPool = new PoolController(_enemiesPrefabs);
            GlobalActions.PlayerDied += () => GlobalActions.GameOver?.Invoke(false);
            GlobalActions.RestartRequest += () => Restart();
            EnemiesOnfield = _enemyPool.EnemiesOnField;
        }

        private void Start()
        {
            SpawnWave();
        }

        private IEnemy GetEnemyToField(Type enemyType)
        {
            IEnemy enemy = _enemyPool.GetEnemy(enemyType);
            enemy.Died += EnemyDied;
            enemy.CauseDamage += (damage) => _player.GetDamage(damage);
            return enemy;
        }

        private void EnemyDied(IEnemy enemy)
        {
            GlobalActions.EnemyDied?.Invoke(enemy);
            enemy.transform.parent = _poolTransform;
            _enemyPool.PutEnemy(enemy);
            if (enemy.GetType().Equals(typeof(GoblinDaddy)))
            {
                for (int i = 0; i < 2; i++)
                {
                    ((GoblinDaddy)enemy).BornKids(GetEnemyToField(typeof(GoblinKid)));
                }
            }

            if (_enemyPool.OnField == 0)
            {
                SpawnWave();
            }
        }

        private void SpawnWave()
        {
            if (_currWave >= _config.Waves.Length)
            {
                GlobalActions.GameOver?.Invoke(true);
                return;
            }
            GlobalActions.NewWave?.Invoke($"{_currWave + 1} / {_config.Waves.Length}");
            var wave = _config.Waves[_currWave];
            foreach (var character in wave.Characters)
            {
                _ = GetEnemyToField(character.GetComponent<IEnemy>().GetType());
            }
            _currWave++;
        }

        private void Restart()
        {
            _currWave = 0;
            _enemyPool.ClearAll();
            SpawnWave();
        }
    }
}