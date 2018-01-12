using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected AbilityConfig config;
        AudioSource audioSource = null;

        const float PARTICLE_CLEAN_UP_DELAY = 20f;

        public abstract void Use(AbilityUseParams useParams);

        protected virtual void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        protected void PlayAbilitySound()
        {
            AudioClip audioClip = config.GetRandomAbilitySound();
            if (audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

        public void SetConfig(AbilityConfig config)
        {
            this.config = config;
        }

        protected void PlayParticleEffect()
        {
            GameObject particlePrefab = config.GetParticlePrefab();
            var particleObject = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
            particleObject.transform.SetParent(transform);
            particleObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyParticleWhenFinished(particleObject));
        }

        IEnumerator DestroyParticleWhenFinished(GameObject particleObject)
        {
            while (particleObject.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            }
            Destroy(particleObject);
            yield return new WaitForEndOfFrame();
        }

    }

    
}
