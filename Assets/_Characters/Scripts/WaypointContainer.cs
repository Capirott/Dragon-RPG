using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class WaypointContainer : MonoBehaviour
    {
        [SerializeField] float sphereRadius = 0.2f;
        private void OnDrawGizmos()
        {
            Vector3 firstPositition = transform.GetChild(0).position;
            Vector3 previousPositition = firstPositition;
            foreach (Transform child in transform)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(child.position, sphereRadius);
                Gizmos.DrawLine(previousPositition, child.position);
                previousPositition = child.position;
            }
            Gizmos.DrawLine(previousPositition, firstPositition);
        }
    }
}