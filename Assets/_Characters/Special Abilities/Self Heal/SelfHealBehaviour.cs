using UnityEngine;
namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        HealthSystem healthSystem = null;

        public override void Use(GameObject target)
        {
            Heal(target);
            PlayAbilitySound();
            PlayParticleEffect();
        }

        private void Heal(GameObject target)
        {
            healthSystem.AddHealth((config as SelfHealConfig).GetExtraHealth());
        }

        protected override void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
            base.Start();
        }
    }
}
