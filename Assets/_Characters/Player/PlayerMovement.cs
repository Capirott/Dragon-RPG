using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerMovement : MonoBehaviour
    {
        ThirdPersonCharacter thirdPersonCharacter;
        CameraRaycaster cameraRaycaster;
        GameObject walkTarget = null;
        AICharacterControl aICharacterControl = null;
        //bool isIndirectMode = false;
        [SerializeField] const int walkableLayerNumber = 8;
        [SerializeField] const int enemyLayerNumber = 9;

        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            aICharacterControl = GetComponent<AICharacterControl>();
            cameraRaycaster.notifyMouseLeftClickObservers += ProcessMouseClick;
            walkTarget = new GameObject("walkTarget");

        }

        private void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
        {
            switch (layerHit)
            {
                case enemyLayerNumber:
                    GameObject enemy = raycastHit.collider.gameObject;
                    aICharacterControl.SetTarget(enemy.transform);
                    break;
                case walkableLayerNumber:
                    walkTarget.transform.position = raycastHit.point;
                    aICharacterControl.SetTarget(walkTarget.transform);
                    break;
                default:
                    Debug.LogError("Don't know how to handle mouse click or player movement");
                    return;
            }
        }

        private void ProcessDirectMovement()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 cameraFoward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * cameraFoward + h * Camera.main.transform.right;
            thirdPersonCharacter.Move(movement, false, false);
        }


    }
}
