using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class Player : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] Weapon currentWeaponConfig;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [Range(0.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle;

        GameObject weaponObject;

        const string ATTACK_TRIGGER = "Attack";

        Enemy currentEnemy = null;
        Animator animator;
        SpecialAbilities abilities;
        CameraRaycaster cameraRaycaster;
        float lastHitTime = 0f;

        private void Start()
        {
            abilities = GetComponent<SpecialAbilities>();
            RegisterForMouseClick();
            PutWeaponInHand(currentWeaponConfig);
            SetupRuntimeAnimator();
        }

        public void PutWeaponInHand(Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = currentWeaponConfig.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject);
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = currentWeaponConfig.GetAnimClip();
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
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        private void OnMouseOverEnemy(Enemy enemy)
        {
            currentEnemy = enemy;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0);
            }
        }

        private void Update()
        {
            var healthAsPercentage = GetComponent<HealthSystem>().healthAsPercentage;
            if (healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for (int i = 1; i < abilities.GetNumberOfAbilities(); ++i)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    abilities.AttemptSpecialAbility(i);
                }
            }
        }  

        private void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                animator.SetTrigger(ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
            bool isCriticalHit = Random.Range(0.0f, 1.0f) <= criticalHitChance;
            float damage = baseDamage + currentWeaponConfig.GetAdditionalDamage();
            if (isCriticalHit)
            {
                criticalHitParticle.Play();
                damage *= criticalHitMultiplier;
            }
            return damage;
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return (distanceToTarget <= currentWeaponConfig.GetMaxAttackRange());
        }


        private void OnDrawGizmos()
        {
            if (currentWeaponConfig != null)
            {
                Gizmos.color = new Color(255f, 0f, 255f, .5f);
                Gizmos.DrawWireSphere(transform.position, currentWeaponConfig.GetMaxAttackRange());
            }
        }
    }
}