namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player = null;

        public override void Use(AbilityUseParams useParams)
        {
            Heal(useParams);
            PlayAbilitySound();
            PlayParticleEffect();
        }

        private void Heal(AbilityUseParams useParams)
        {
            player.AddHealth((config as SelfHealConfig).GetExtraHealth());
        }

        protected override void Start()
        {
            player = GetComponent<Player>();
            base.Start();
        }
    }
}
