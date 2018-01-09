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
