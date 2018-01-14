using System.Collections;
using UnityEngine;


namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]    
    [RequireComponent(typeof(Character))]    
    [RequireComponent(typeof(HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointTolerance = 2.0f;
        [SerializeField] float waypointDwellTime = 2.0f;

        enum State
        {
            idle,
            patrolling,
            attacking,
            chasing
        }

        State state = State.idle;

        PlayerControl player = null;
        Character character;
        float currentWeaponRange;
        float distanceToPlayer;
        int nextWaypointIndex;

        private void Start()
        {
            player = GameObject.FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
        }


        private void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            bool inWeaponCircle = distanceToPlayer <= currentWeaponRange;
            bool inChaseRing = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
            bool outsideChaseRing = distanceToPlayer > chaseRadius;
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            if (outsideChaseRing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(Patrol());
            }
            if (inChaseRing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(ChasePlayer());
            }
            if (inWeaponCircle)
            {
                StopAllCoroutines();
                weaponSystem.AttackTarget(player.gameObject);
                state = State.attacking;
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;            
            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDesination(nextWaypointPos);
                CycleWaypointWhenClose(nextWaypointPos);
                yield return new WaitForSeconds(waypointDwellTime);
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDesination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 0f, 0f, .5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);
            Gizmos.color = new Color(0f, 0f, 255f, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }

    }
}