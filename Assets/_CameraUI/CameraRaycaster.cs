using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using RPG.Characters;
namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Vector2 cursorHotSpot = new Vector2(0, 0);

        const int POTENTIALLY_WALKABLE_LAYER = 8;
        float maxRaycastDepth = 100f;

        public delegate void OnMouseOverTerrain(Vector3 destination);
        public event OnMouseOverTerrain onMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy(Enemy enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; 
            }
            else
            {
                PerformRaycast();
            }
        }

        void PerformRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (RaycastForEnemy(ray)) { return; }
            if (RaycastForWalkable(ray)) { return; }
        }

        private bool RaycastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
                onMouseOverPotentiallyWalkable(hitInfo.point);
            }
            return potentiallyWalkableHit;
        }

        bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, maxRaycastDepth))
            {
                GameObject gameObjectHit = hitInfo.collider.gameObject;
                Enemy enemyHit = gameObjectHit.GetComponent<Enemy>();
                if (enemyHit)
                {
                    Cursor.SetCursor(enemyCursor, cursorHotSpot, CursorMode.Auto);
                    onMouseOverEnemy(enemyHit);
                }
                return enemyHit;
            }
            return false;
        }
    }
}