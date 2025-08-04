using Assets.Scripts.Interfaces;
using System;

namespace Assets.Scripts.Actions
{
    public static class GlobalActions
    {
        //Scene Controller
        public static Action<string> NewWave;
        public static Action<IEnemy> EnemyDied;
        public static Action<bool> GameOver;

        //player
        public static Action PlayerDied;
        public static Action<float> AttackEst;
        public static Action<float> DoubleAttackEst;
        public static Action<bool> HaveTargets;

        //UI
        public static Action AttackPressed;
        public static Action DoubleAttackPressed;
        public static Action RestartRequest;
    }
}
