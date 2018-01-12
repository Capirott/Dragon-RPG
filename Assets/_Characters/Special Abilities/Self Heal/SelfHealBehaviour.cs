using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config;
        Player player;

        private void Start()
        {
            player = GetComponent<Player>();
        }

        public void Use(AbilityUseParams useParams)
        {
            Heal(useParams);
            PlayParticleEffect();
        }

        private void Heal(AbilityUseParams useParams)
        {
            player.AddHealth(config.GetExtraHealth());
        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetParticlePrefab(), transform);
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration);
        }

        public void SetPowerAttackConfig(SelfHealConfig config)
        {
            this.config = config;
        }

        public SelfHealConfig GetSelfHealConfig()
        {
            return config;
        }

    }
}
