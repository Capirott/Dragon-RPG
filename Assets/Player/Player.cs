using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float currentHealthPoints;
    [SerializeField] float damagePerHit = 10f;
    [SerializeField] float maxAttackRange = 4f;
    [SerializeField] float minTimeBetweenHits = 0.5f;
    [SerializeField] GameObject currentTarget;
    CameraRaycaster cameraRaycaster;
    [SerializeField] const int enemyLayerNumber = 9;
    float lastHitTime = 0f;
    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    private void Start()
    {
        currentHealthPoints = maxHealthPoints;
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }

    void OnMouseClick (RaycastHit raycastHit, int layerID)
    {
        switch (layerID)
        {
            case enemyLayerNumber:
                GameObject target = raycastHit.collider.gameObject;
                if ((target.transform.position - transform.position).magnitude > maxAttackRange)
                {
                    return;
                }
                currentTarget = target;
                IDamageable enemy = currentTarget.GetComponent<IDamageable>();
                if (Time.time - lastHitTime > minTimeBetweenHits)
                {
                    enemy.TakeDamage(damagePerHit);
                    lastHitTime = Time.time;
                }
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255f, 0f, 255f, .5f);
        Gizmos.DrawWireSphere(transform.position, maxAttackRange);
    }
}
