﻿using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]
    public class SelfHealConfig : AbilityConfig
    {
        [Header("Self Heal Specific")]
        [SerializeField] float extraHealth = 10f;

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            SelfHealBehaviour behaviourComponent = gameObjectToAttachTo.AddComponent<SelfHealBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetExtraHealth()
        {
            return extraHealth;
        }
    }
}
