using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
namespace RPG.Characters
{
    public class AreaEffectBehaviour : MonoBehaviour, ISpecialAbility
    {
        AreaEffectConfig config;

        public void Use(AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());
            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(config.GetDamageToEachTarget());
            }
        }

        public void SetPowerAttackConfig(AreaEffectConfig config)
        {
            this.config = config;
        }

        public AreaEffectConfig GetPowerAttackConfig()
        {
            return config;
        }
    }
}