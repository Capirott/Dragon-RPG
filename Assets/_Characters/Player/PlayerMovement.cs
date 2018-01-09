using System;
using UnityEngine;
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

        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            aICharacterControl = GetComponent<AICharacterControl>();
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            walkTarget = new GameObject("walkTarget");

        }

        private void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                walkTarget.transform.position = enemy.transform.position;
                aICharacterControl.SetTarget(walkTarget.transform);
            }
        }

        private void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aICharacterControl.SetTarget(walkTarget.transform);
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
