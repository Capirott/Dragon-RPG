using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : AbilityBehaviour
    {
        public override void Use(GameObject target)
        {
            DealRadialDamage();
            PlayAbilitySound();
            PlayAbilityAnimation();
            PlayParticleEffect();
        }
        
        private void DealRadialDamage()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, (config as AreaEffectConfig).GetRadius(), Vector3.up, (config as AreaEffectConfig).GetRadius());
            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.GetComponent<HealthSystem>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerControl>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDeal = (config as AreaEffectConfig).GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }   
        }  
    }
}