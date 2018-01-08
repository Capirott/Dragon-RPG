using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {

        public float projectileSpeed;
        [SerializeField] GameObject shooter;

        public float projectileLifeSpan = 1.5f;
        float timeCreated = 0f;
        float damageCaused;
        const float DESTROY_DELAY = 0.01f;
        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }

        private void Start()
        {
            timeCreated = Time.time;
        }

        private void Update()
        {
            if (Time.time - timeCreated >= projectileLifeSpan)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (shooter && collision.gameObject.layer != shooter.layer)
            {
                DamageIfDamageable(collision);
            }
        }

        private void DamageIfDamageable(Collision collision)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageCaused);
            }
            Destroy(gameObject, DESTROY_DELAY);
        }

        internal void SetDamage(float damagePerShot)
        {
            this.damageCaused = damagePerShot;
        }
    }
}