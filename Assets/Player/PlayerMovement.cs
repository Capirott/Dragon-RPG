using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkMoveStopRadius = 0.2f;
    [SerializeField] float attackMoveStopRadius = 5f;
    ThirdPersonCharacter thirdPersonCharacter;   
    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination;
    bool isIndirectMode = false;
    Vector3 clickPoint;

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
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
        //if (Input.GetMouseButton(0))
        //{
        //    clickPoint = cameraRaycaster.hit.point;
        //    switch (cameraRaycaster.currentLayerHit)
        //    {
        //        case 8:
        //            currentDestination = clickPoint;
        //            currentDestination = ShortDestination(clickPoint, walkMoveStopRadius);
        //            break;
        //        case 9:
        //            currentDestination = ShortDestination(clickPoint, attackMoveStopRadius);
        //            break;
        //        default:
        //            print("Unexpected layer found");
        //            break;
        //    }
        //}
        //WalkToDestination();
    }

    private void WalkToDestination()
    {
        var playerToClickPoint = currentDestination - transform.position;
        if (playerToClickPoint.magnitude >= walkMoveStopRadius)
        {
            thirdPersonCharacter.Move(playerToClickPoint, false, false);
        }
        {
            thirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }

    private Vector3 ShortDestination(Vector3 destination, float shortening)
    {
        Vector3 reductionVector = (destination - transform.position).normalized * shortening;
        return destination - reductionVector;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, currentDestination);
        Gizmos.DrawSphere(currentDestination, 0.1f);
        Gizmos.DrawSphere(clickPoint, 0.15f);
        Gizmos.color = new Color(255f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackMoveStopRadius);
    }
}

