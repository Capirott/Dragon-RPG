using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void Use()
        {
            print("Power attack use");
        }

        public void SetPowerAttackConfig(PowerAttackConfig config)
        {
            this.config = config;
        }

        public PowerAttackConfig GetPowerAttackConfig()
        {
            return config;
        }

    }
}
