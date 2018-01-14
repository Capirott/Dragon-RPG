using UnityEngine;
using RPG.CameraUI;
using System.Collections;

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        EnemyAI currentEnemy = null;
        Character character;
        WeaponSystem weaponSystem;
        SpecialAbilities abilities;

        private void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        private void RegisterForMouseEvents()
        {
            var cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        public void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                currentEnemy = null;
                character.SetDesination(destination);
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return (distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange());
        }

        private void OnMouseOverEnemy(EnemyAI enemy)
        {
            currentEnemy = enemy;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndAttack(enemy.gameObject));
            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemy.gameObject))
            {
                abilities.AttemptSpecialAbility(0, enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndPowerAttack(enemy.gameObject));
            }
        }

        IEnumerator MoveToTarget(GameObject enemy)
        {
            while (!IsTargetInRange(enemy.gameObject))
            {
                character.SetDesination(enemy.transform.position);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack(GameObject enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy));
            weaponSystem.AttackTarget(enemy.gameObject);
        }

        IEnumerator MoveAndPowerAttack(GameObject enemy)
        {
            yield return StartCoroutine(MoveToTarget(enemy));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);
        }


        private void Update()
        {
            ScanForAbilityKeyDown();
        }

        private void ScanForAbilityKeyDown()
        {
            for (int i = 1; i < abilities.GetNumberOfAbilities(); ++i)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    abilities.AttemptSpecialAbility(i, currentEnemy ? currentEnemy.gameObject : null);
                }
            }
        }


    }
}