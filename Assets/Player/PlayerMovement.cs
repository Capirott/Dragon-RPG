using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkMoveStopRadius = 0.2f;
    ThirdPersonCharacter thirdPersonCharacter;   
    CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;
    bool isIndirectMode = false;

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isIndirectMode = !isIndirectMode;
            currentClickTarget = transform.position;

        }
        if (isIndirectMode)
        {
            ProcessDirectMovement();
        }
        else
        {
            ProcessMouseMovement();
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

    private void ProcessMouseMovement()
    {
        if (Input.GetMouseButton(0))
        {
            print("Cursor raycast hit" + cameraRaycaster.hit.collider.gameObject.name.ToString());
            switch (cameraRaycaster.currentLayerHit)
            {
                case Layer.Walkable:
                    currentClickTarget = cameraRaycaster.hit.point;  // So not set in default case
                    break;
                case Layer.Enemy:
                    print("Not moving to enemy");
                    break;
                default:
                    print("Unexpected layer found");
                    break;
            }
        }
        var playerToClickPoint = currentClickTarget - transform.position;
        if (playerToClickPoint.magnitude >= walkMoveStopRadius)
        {
            thirdPersonCharacter.Move(playerToClickPoint, false, false);
        }
        {
            thirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }
}

