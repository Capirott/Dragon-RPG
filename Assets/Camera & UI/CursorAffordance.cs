using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour {

    [SerializeField] Texture2D walkCursor = null;
    [SerializeField] Texture2D targetCursor = null;
    [SerializeField] Texture2D unknowCursor = null;
    [SerializeField] Vector2 cursorHotSpot = new Vector2(0, 0);
    CameraRaycaster cameraRaycaster;

    void Start () {
        cameraRaycaster = GetComponent<CameraRaycaster>();		
	}
	
	void LateUpdate () {
        switch (cameraRaycaster.currentLayerHit)
        {
            case Layer.Enemy:
                Cursor.SetCursor(targetCursor, cursorHotSpot, CursorMode.Auto);
                break;
            case Layer.Walkable:
                Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
                break;
            case Layer.RaycastEndStop:
                Cursor.SetCursor(unknowCursor, cursorHotSpot, CursorMode.Auto);
                break;
            default:
                Debug.LogError("Don't know what cursor to show!");
                break;
        }
    }
}
