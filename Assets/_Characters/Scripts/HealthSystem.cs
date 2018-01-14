using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float floatDeathVanishSeconds = 2.0f;

        Animator animator;
        AudioSource audioSource;
        float currentHealthPoints;
        const string DEATH_TRIGGER = "Death";
        Character characterMovement;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            characterMovement = GetComponent<Character>();
            SetCurrentMaxHealth();
        }

        private void Update()
        {
            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            if (healthBar)
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (damageSounds.Length > 0)
            {
                var clip = damageSounds[Random.Range(0, damageSounds.Length)];
                audioSource.PlayOneShot(clip);
            }
            if (currentHealthPoints == 0)
            {
                StartCoroutine(KillCharacter());
            }
        }

        public void AddHealth(float pointsToAdd)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + pointsToAdd, 0, maxHealthPoints);
        }

        IEnumerator KillCharacter()
        {
            characterMovement.Kill();
            animator.SetTrigger(DEATH_TRIGGER);
            if (deathSounds.Length > 0)
            {
                audioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
                audioSource.Play();
                yield return new WaitForSecondsRealtime(audioSource.clip.length + 1f);
            }
            var playerComponent = GetComponent<PlayerControl>();
            if (playerComponent && playerComponent.isActiveAndEnabled)
            {
                SceneManager.LoadScene(0);
            } 
            else
            {
                Destroy(gameObject, floatDeathVanishSeconds);
            }
        }

        private void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

    }
}