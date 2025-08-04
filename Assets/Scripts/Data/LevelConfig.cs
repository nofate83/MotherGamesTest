using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Data/BattleCampInfo")]
    public class LevelConfig : ScriptableObject
    {
        public Wave[] Waves;

    }
}