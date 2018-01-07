using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [HideInInspector]
    public float damageCaused;
    public float projectileLifeSpan = 1.5f;
    float timeCreated = 0f;

    public float projectileSpeed;

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
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageCaused);
        }
        Destroy(gameObject, 0.01f);
    }

}
