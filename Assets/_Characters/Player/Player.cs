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

        [SerializeField] SpecialAbility[] abilities;

        Animator animator;
        CameraRaycaster cameraRaycaster;
        float lastHitTime = 0f;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void TakeDamage(float damage)
        {
            ReduceHealth(damage);
            if(currentHealthPoints == 0)
            {
                StartCoroutine(KillPlayer());
            }
        }

        IEnumerator KillPlayer()
        {
            print("You're dead!");
            yield return new WaitForSecondsRealtime(2f);
            SceneManager.LoadScene(0);            
        }

        private void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        }

        private void Start()
        {
            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            abilities[0].AttachComponentTo(gameObject);
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
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0, enemy);
            }
        }

        private void Update()
        {
            if (currentHealthPoints < maxHealthPoints)
            {
                AddEnergyPoints();
            }
        }

        private void AddEnergyPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + pointsToAdd, 0, maxHealthPoints);
        }

        private void AttemptSpecialAbility(int index, Enemy enemy)
        {
            Energy energy = GetComponent<Energy>();
            float energyCost = abilities[index].GetEnergyCost();
            if (energy.IsEnergyAvailable(energyCost))
            {
                energy.ConsumeEnergy(energyCost);
                var abilityParams = new AbilityUseParams(enemy, baseDamage);
                abilities[index].Use(abilityParams);
            }
        }

        private void AttackTarget(Enemy target)
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger("Attack");
                target.TakeDamage(baseDamage);
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