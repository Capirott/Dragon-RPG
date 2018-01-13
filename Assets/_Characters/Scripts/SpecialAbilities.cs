using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float currentEnergyPoints;
        [SerializeField] float regenPointsPerSecond = 10f;

        private AudioSource audioSource;

        public float energyAsPercentage { get { return currentEnergyPoints / maxEnergyPoints; } }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }
            
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            currentEnergyPoints = maxEnergyPoints;
            AttachInitialAbilities();
            UpdateEnergyBar();
        }

        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoints();
                UpdateEnergyBar();
            }
        }

        private void AddEnergyPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            energyBar.fillAmount = energyAsPercentage;
        }

        private void AttachInitialAbilities()
        {
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility(int index)
        {
            SpecialAbilities energy = GetComponent<SpecialAbilities>();
            float energyCost = abilities[index].GetEnergyCost();
            if (energyCost <= currentEnergyPoints)
            {
                energy.ConsumeEnergy(energyCost);
            }
        }
    }
}