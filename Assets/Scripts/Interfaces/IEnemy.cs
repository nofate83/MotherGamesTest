using System;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IEnemy
    {
        Transform transform { get; }

        string name { get; set; }

        Action<IEnemy> Died { get; set; }

        Action<float> CauseDamage { get; set; }

        float DistanceToPlayer { get; }

        bool IsDead { get; }

        void GetDamage(float damage);

        void Renew();
    }
}
