using Assets.Scripts.Abstractions;
using Assets.Scripts.Actions;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Controllers.Characters
{
    public class GoblinDaddy : EnemyBase
    {
       
        [SerializeField] private ParticleSystem _particleSystem;
                
        private Transform _expContainer;
        
        protected override void Awake()
        {
            _hp = _startHp;
            _expContainer = GameObject.Find("ExplosionsContainer").transform;
            CreateExplosion();
            _animatorController = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            GlobalActions.GameOver += (res) => _gameOver = true;
        }
        
        private void CreateExplosion()
        {
            _particleSystem = Instantiate(_particleSystem,
                    new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)),
                    Quaternion.identity
                    ).GetComponent<ParticleSystem>();
            _particleSystem.Stop();
            _particleSystem.transform.parent = _expContainer;
        }

        protected override void Die()
        {
            Debug.Log("Die in Daddy");
            _isDead = true;
            Died?.Invoke(this);
            _particleSystem.transform.position = transform.position;
            _particleSystem.Play();
            gameObject.SetActive(false);
        }

        public void BornKids(IEnemy kid)
        {
            kid.transform.position = transform.position - transform.forward +
                new Vector3(UnityEngine.Random.Range(-10, 10) / 2, 0, UnityEngine.Random.Range(-10, 10) / 2);
        }
    }
}