using Assets.Scripts.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] TMP_Text _waves;
        [SerializeField] TMP_Text _casualities;
        [SerializeField] ScrollRect _casScrollView;
        [SerializeField] private GameObject Lose;
        [SerializeField] private GameObject Win;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _doubleAttackButton;
        [SerializeField] private Image _attackProgressBar;
        [SerializeField] private Image _doubleAttackProgressBar;

        void Awake()
        {
            _attackButton.onClick.AddListener(() => GlobalActions.AttackPressed?.Invoke());
            _doubleAttackButton.onClick.AddListener(() => GlobalActions.DoubleAttackPressed?.Invoke());
            _restartButton.onClick.AddListener(() => Restart());
            _casScrollView.onValueChanged.AddListener((Vector2 vec) => _casScrollView.normalizedPosition = new Vector2(0, 0));
            GlobalActions.GameOver += GameOver;
            GlobalActions.NewWave += (newstr) => _waves.text = newstr;
            GlobalActions.EnemyDied += (enemy) => _casualities.text += $"\n {enemy.name.Split('.')[4]}";
            GlobalActions.AttackEst += (val) => _attackProgressBar.fillAmount = val;
            GlobalActions.DoubleAttackEst += (val) => _doubleAttackProgressBar.fillAmount = val;
            GlobalActions.HaveTargets += (res) => _doubleAttackButton.interactable = res;

        }

        public void Restart()
        {
            Lose.SetActive(false);
            Win.SetActive(false);
            GlobalActions.RestartRequest?.Invoke();
        }

        private void GameOver(bool res)
        {
            if (res)
            {
                Win.SetActive(true);
            }
            else
            {
                Lose.SetActive(true);
            }
        }
    }
}