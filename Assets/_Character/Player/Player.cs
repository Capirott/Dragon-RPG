using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float currentHealthPoints;
        [SerializeField] float damagePerHit = 10f;
        [SerializeField] Weapon weaponInUse;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;
        CameraRaycaster cameraRaycaster;
        [SerializeField] const int enemyLayerNumber = 9;
        float lastHitTime = 0f;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        }

        private void Start()
        {
            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
        }

        private void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAnimClip();
        }

        private void PutWeaponInHand()
        {
            if (weaponInUse == null)
                return;
            var weaponPrefab = weaponInUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            var weapon = Instantiate(weaponPrefab, dominantHand.transform);
            weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantsHands = dominantHands.Length;
            Assert.IsFalse(numberOfDominantsHands <= 0, "No DominantHand found on player, please add one.");
            Assert.IsFalse(numberOfDominantsHands > 1, "Multiple DominantHand scripts on player, please remove one.");
            return dominantHands[0].gameObject;
        }

        private void RegisterForMouseClick()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
        }

        void OnMouseClick(RaycastHit raycastHit, int layerID)
        {
            switch (layerID)
            {
                case enemyLayerNumber:
                    GameObject target = raycastHit.collider.gameObject;
                    if (IsTargetInRange(target))
                    {
                        AttackTarget(target);
                    }                    
                    break;
            }
        }

        private void AttackTarget(GameObject target)
        {
            IDamageable enemyDamageable = target.GetComponent<IDamageable>();
            if (enemyDamageable != null && (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits()))
            {
                animator.SetTrigger("Attack");
                enemyDamageable.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return (distanceToTarget <= weaponInUse.GetMaxAttackRange());
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, weaponInUse.GetMaxAttackRange());
        }
    }
}