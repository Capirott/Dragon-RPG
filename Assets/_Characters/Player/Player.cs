using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;
using System.Collections;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float currentHealthPoints;
        [SerializeField] float regenPointsPerSecond = 10f;
        [SerializeField] float baseDamage = 10f;
        [SerializeField] Weapon weaponInUse;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [Range(0.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticle;
        [SerializeField] AbilityConfig[] abilities;
        AudioSource audioSource;

        const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";

        Enemy currentEnemy = null;
        Animator animator;
        CameraRaycaster cameraRaycaster;
        float lastHitTime = 0f;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void TakeDamage(float damage)
        {
            ReduceHealth(damage);
            if (currentHealthPoints == 0)
            {
                StartCoroutine(KillPlayer());
            }           
        }

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            AttachInitialAbilities();
        }

        private void AttachInitialAbilities()
        {
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i].AttachAbilityTo(gameObject);
            }
        }

        IEnumerator KillPlayer()
        {
            regenPointsPerSecond = 0;
            audioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
            audioSource.Play();
            animator.SetTrigger(DEATH_TRIGGER);
            yield return new WaitForSecondsRealtime(audioSource.clip.length + 1f);
            SceneManager.LoadScene(0);
        }

        private void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            audioSource.clip = damageSounds[Random.Range(0, damageSounds.Length)];
            audioSource.Play();
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
                AttemptSpecialAbility(0);
            }
        }

        private void Update()
        {
            if (healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
                if (currentHealthPoints < maxHealthPoints)
                {
                    AddHealthPoints();
                }
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for (int i = 1; i < abilities.Length; ++i)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    AttemptSpecialAbility(i);
                }
            }
        }

        private void AddHealthPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            AddHealth(pointsToAdd);
        }

        public void AddHealth(float pointsToAdd)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + pointsToAdd, 0, maxHealthPoints);
        }

        private void AttemptSpecialAbility(int index)
        {
            Energy energy = GetComponent<Energy>();
            float energyCost = abilities[index].GetEnergyCost();
            if (energy.IsEnergyAvailable(energyCost))
            {
                energy.ConsumeEnergy(energyCost);
                var abilityParams = new AbilityUseParams(currentEnemy, baseDamage);
                abilities[index].Use(abilityParams);
            }
        }

        private void AttackTarget()
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger(ATTACK_TRIGGER);
                currentEnemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
            bool isCriticalHit = Random.Range(0.0f, 1.0f) <= criticalHitChance;
            float damage = baseDamage + weaponInUse.GetAdditionalDamage();
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
            return (distanceToTarget <= weaponInUse.GetMaxAttackRange());
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, weaponInUse.GetMaxAttackRange());
        }
    }
}