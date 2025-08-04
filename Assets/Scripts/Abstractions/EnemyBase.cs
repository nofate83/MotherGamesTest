using Assets.Scripts.Actions;
using Assets.Scripts.Controllers;
using Assets.Scripts.Interfaces;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Abstractions
{
    public abstract class EnemyBase : MonoBehaviour, IEnemy
    {
        [SerializeField] protected float _startHp;
        [SerializeField] protected float _hp;
        [SerializeField] protected float _damage;
        [SerializeField] protected float _attackSpeed;
        [SerializeField] protected float _attackRange = 2;

        protected Animator _animatorController;
        protected NavMeshAgent _agent;

        protected float lastAttackTime = 0;
        protected bool _gameOver = false;
        protected bool _isDead;
        public bool IsDead => _isDead;

        public float DistanceToPlayer => Vector3.Distance(transform.position, Player.Transform.position);
        public Action<IEnemy> Died { get; set; }
        public Action<float> CauseDamage { get; set; }

        protected virtual void Awake()
        {
            _animatorController = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _hp = _startHp;
            GlobalActions.GameOver += (res) => _gameOver = true;
        }

        protected void Start()
        {
            _agent.SetDestination(Player.Transform.position);
        }

        public void GetDamage(float damage)
        {
            _hp -= damage;
            if (_hp <= 0) Die();
        }

        protected virtual void Die()
        {
            _isDead = true;
            _animatorController.SetTrigger("Die");
        }

        private void CompletelyDied()
        {
            gameObject.SetActive(false);
            Died?.Invoke(this);
        }

        public void Renew()
        {
            _isDead = false;
            transform.position = new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));
            _hp = _startHp;
            gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _agent.isStopped = true;
        }

        private void OnEnable()
        {
            if (_agent != null)
            {
                _agent.isStopped = false;
            }
        }

        private void AttackEnds() { } //common handler for goblin animations, used in Player

        private void FixedUpdate()
        {
            if (_isDead)
            {
                return;
            }

            if (DistanceToPlayer <= _attackRange)
            {
                _agent.isStopped = true;
                if (Time.time - lastAttackTime > _attackSpeed && !_gameOver)
                {
                    lastAttackTime = Time.time;
                    CauseDamage.Invoke(_damage);
                    _animatorController.SetBool("Walk", false);
                    _animatorController.SetTrigger("Attack");
                }
            }
            else
            {
                if (_agent.isStopped)
                {
                    _agent.isStopped = false;
                }
                _agent.SetDestination(Player.Transform.position);
                _animatorController.SetBool("Walk", true);
            }
        }
    }
}
