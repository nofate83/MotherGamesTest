using Assets.Scripts.Actions;
using Assets.Scripts.Interfaces;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Controllers
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _startHp;
        private float _hp;
        [SerializeField] private float _attackDamage;
        [SerializeField] private float _attackSpeed;
        [SerializeField] private float _attackRange = 2;
        [SerializeField] private float _doubleAttackDamage;
        [SerializeField] private float _doubleAttackSpeed = 2;

        private Animator _animatorController;
        private PlayerInput _input;
        private Vector3 _moveDirection;
        private float _movementSpeed = 0.1f;
        private IEnemy[] _targets;
        private bool _inAttack = false;  //this shows real state of attack animations, controlled by event in animation
        private float _lastAttackTime = 0;
        private float _lastDoubleAttackTime = 0;
        private bool _dead => _hp <= 0;
        private bool _targetsAvailabel => _targets.Length > 0;
        public static Transform Transform { get; private set; }

        private void Awake()
        {
            _hp = _startHp;
            _input = new PlayerInput();
            _input.Enable();
            Transform = transform;
            GlobalActions.EnemyDied += EnemyKilled;
            _animatorController = GetComponent<Animator>();
            _input.PlayerActionMap.Moving.performed += (direction) =>
            {
                _moveDirection = new Vector3(direction.ReadValue<Vector2>().x, 0, direction.ReadValue<Vector2>().y);
                transform.LookAt(transform.position + _moveDirection, Vector3.up);
                _animatorController.SetBool("Walk", true);

            };
            _input.PlayerActionMap.Moving.canceled += (_) =>
            {
                _moveDirection = Vector2.zero;
                _animatorController.SetBool("Walk", false);
            };
            _input.PlayerActionMap.Attack.performed += (cb) => Attack(cb);

            _input.PlayerActionMap.SuperAttack.performed += (cb) => DoubleAttack(cb);
            GlobalActions.AttackPressed += () => Attack(new InputAction.CallbackContext());
            GlobalActions.DoubleAttackPressed += () => DoubleAttack(new InputAction.CallbackContext());
            GlobalActions.RestartRequest += () => Restart();
            GlobalActions.GameOver += (bool res) => _input.Disable();
        }

        private void Attack(InputAction.CallbackContext _)
        {
            if (!_inAttack && Time.time - _lastAttackTime > _attackSpeed && !_dead)
            {
                _inAttack = true;
                if (_targets.Length > 0)
                {
                    transform.LookAt(_targets[0].transform.position);
                    _targets[0].GetDamage(_attackDamage);

                }
                _lastAttackTime = Time.time;
                _animatorController.SetTrigger("Attack");
            }
        }

        private void DoubleAttack(InputAction.CallbackContext _)
        {
            if (!_inAttack && Time.time - _lastDoubleAttackTime > _doubleAttackSpeed && _targetsAvailabel && !_dead)
            {
                _inAttack = true;
                if (_targets.Length > 0)
                {
                    Vector3 centroid = Vector3.zero;
                    foreach (var target in _targets)
                    {
                        centroid += target.transform.position;
                        target.GetDamage(_doubleAttackDamage);
                    }

                    transform.LookAt(centroid / _targets.Length);
                }

                _lastDoubleAttackTime = Time.time;
                _animatorController.SetTrigger("DoubleAttack");
            }
        }

        private void OnEnable() => _input.Enable();

        private void OnDisable() => _input.Disable();

        private void AttackEnds() => _inAttack = false;

        public void EnemyKilled(IEnemy enemy) => _hp += 10;

        public void GetDamage(float damage)
        {
            if(_hp > 0)   //animation issue
            {
                _hp -= damage;
            }
            
            if (_hp <= 0)
            {
                _animatorController.SetTrigger("Die");
            }
        }

        private void CompletelyDied() => GlobalActions.PlayerDied?.Invoke();
       
        private void Restart()
        { 
            _hp = _startHp;
            _input.Enable();
            _animatorController.SetTrigger("Idle");
            transform.position = Vector3.zero;
            Camera.main.transform.position = transform.position + new Vector3(0, 16, -16);
        }
        
        private void FixedUpdate()
        {
            _targets = SceneController.EnemiesOnfield.Where(x => x.DistanceToPlayer < _attackRange && !x.IsDead).ToArray();
            GlobalActions.HaveTargets?.Invoke(_targets.Length > 0);

            if (Time.time - _lastDoubleAttackTime < _doubleAttackSpeed)
            {
                GlobalActions.DoubleAttackEst?.Invoke((Time.time - _lastDoubleAttackTime) / _doubleAttackSpeed);
            }

            if (Time.time - _lastAttackTime < _attackSpeed)
            {
                GlobalActions.AttackEst?.Invoke((Time.time - _lastAttackTime) / _attackSpeed);
            }

            //have a sense to add rigidbody to player object for more complicated and flexible decisions
            if (_moveDirection != Vector3.zero)
            {
                transform.position += _moveDirection * _movementSpeed;
                Camera.main.transform.position = transform.position + new Vector3(0, 16, -16);
            }

        }
    }
}