namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        HealthSystem healthSystem = null;

        public override void Use(AbilityUseParams useParams)
        {
            Heal(useParams);
            PlayAbilitySound();
            PlayParticleEffect();
        }

        private void Heal(AbilityUseParams useParams)
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
