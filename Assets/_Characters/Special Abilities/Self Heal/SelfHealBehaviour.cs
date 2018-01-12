using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config = null;
        Player player = null;
        AudioSource audioSource = null;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            player = GetComponent<Player>();
        }

        public void Use(AbilityUseParams useParams)
        {
            PlayParticleEffect();
            PlaySound();
            Heal(useParams);
        }

        private void PlaySound()
        {
            AudioClip audioClip = config.GetAudioClip();
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
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
