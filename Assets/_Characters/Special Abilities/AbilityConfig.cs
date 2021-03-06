﻿using UnityEngine;
using RPG.Core;

namespace RPG.Characters {

    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip[] audioClips = null;
        [SerializeField] AnimationClip abilityAnimation;

        protected AbilityBehaviour behaviour;

        public void AttachAbilityTo(GameObject gameObjectToAttachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(gameObjectToAttachTo);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo);

        public AnimationClip GetAnimationClip()
        {
            return abilityAnimation;
        }

        public void Use(GameObject target)
        {
            behaviour.Use(target);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip GetRandomAbilitySound()
        {         
            return audioClips.Length == 0 ? null : audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}