using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;


namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float currentEnergyPoints;
        CameraRaycaster cameraRaycaster;
        

        public float energyAsPercentage { get { return currentEnergyPoints / maxEnergyPoints; } }

        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
        }

        public bool IsEnergyAvailable(float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            float xValue = -(energyAsPercentage / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }
    }
}