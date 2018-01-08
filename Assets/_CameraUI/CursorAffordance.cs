using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{

    [RequireComponent(typeof(CameraRaycaster))]
    public class CursorAffordance : MonoBehaviour
    {

        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D targetCursor = null;
        [SerializeField] Texture2D unknowCursor = null;
        [SerializeField] Vector2 cursorHotSpot = new Vector2(0, 0);
        [SerializeField] const int walkableLayerNumber = 8;
        [SerializeField] const int enemyLayerNumber = 9;
        CameraRaycaster cameraRaycaster;

        void Start()
        {
            cameraRaycaster = GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyLayerChangeObservers += OnLayerChanged;
        }

        void OnLayerChanged(int layerHit)
        {
            switch (layerHit)
            {
                case enemyLayerNumber:
                    Cursor.SetCursor(targetCursor, cursorHotSpot, CursorMode.Auto);
                    break;
                case walkableLayerNumber:
                    Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(unknowCursor, cursorHotSpot, CursorMode.Auto);
                    break;
            }
        }
    }

}