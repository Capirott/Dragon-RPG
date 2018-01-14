using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";    

        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;
        float lastHitTime;

        private void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        private void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;

            if (target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                targetIsDead = target.GetComponent<HealthSystem>().healthAsPercentage <= Mathf.Epsilon;
                targetIsOutOfRange = Vector3.Distance(transform.position, target.transform.position) > currentWeaponConfig.GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }


        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantsHands = dominantHands.Length;
            Assert.IsFalse(numberOfDominantsHands <= 0, "No DominantHand found on player, please add one.");
            Assert.IsFalse(numberOfDominantsHands > 1, "Multiple DominantHand scripts on player, please remove one.");
            return dominantHands[0].gameObject;
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = currentWeaponConfig.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject);
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        private void SetAttackAnimation()
        {
            if (!character.GetAnimatorOverrideController())
            {
                Debug.LogAssertion("Please provide " + gameObject.name + "with an animator override controller.");
                Debug.Break();
            }
            animator = GetComponent<Animator>();
            var animatorOverrideController    = character.GetAnimatorOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip();
        }

        public void AttackTarget(GameObject target)
        {
            this.target = target;
            StartCoroutine(AttackTargetRepeatedly());
        }

        void AttackTargetOnce()
        {
            transform.LookAt(target.transform.position);
            SetAttackAnimation();
            animator.SetTrigger(ATTACK_TRIGGER);
            StartCoroutine(DamageAfterDelay(currentWeaponConfig.GetDamageDelay()));
        }

        IEnumerator DamageAfterDelay(float damageDelay)
        {
            yield return new WaitForSeconds(damageDelay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }


        IEnumerator AttackTargetRepeatedly()
        {
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            float timeToWait = currentWeaponConfig.GetMinTimeBetweenHits() * character.GetAnimationSpeedMultiplier();
            while (attackerStillAlive && targetStillAlive)
            {
                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;
                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
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
            float damage = baseDamage + currentWeaponConfig.GetAdditionalDamage();
            return damage;
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