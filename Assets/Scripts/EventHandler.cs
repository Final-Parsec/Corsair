using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    /// <summary>
    /// Manages and maps class instances.
    /// </summary>
    private ObjectManager objectManager;

    /// <summary>
    /// Number of turrets the user can select.
    /// </summary>
	private readonly int numberOfTurrets = Enum.GetNames (typeof(TurretType)).Length;

    /// <summary>
    /// A reference to the UI canvas in the Scene.
    /// </summary>
	private Canvas canvas;
    
	void Start ()
	{
		objectManager = ObjectManager.GetInstance ();
		canvas = GameObject.Find("Canvas").GetComponent("Canvas") as Canvas;
	}
	
	void Update ()
	{
		// Escape (Back button on Android)
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
        
        // Options are displayed or
        // Game is over or
        // Mouse is over UI or
        // Single touch is over UI.
		if (objectManager.gameState.optionsOn ||
			objectManager.gameState.gameOver ||
			IsPointerOverUiObject (canvas, new Vector2 (Input.mousePosition.x, Input.mousePosition.y)) ||
			(Input.touchCount == 1 && IsPointerOverUiObject (canvas, Input.touches [0].position))) 
		{
			return;
		}
        
        // Camera is not moving and there is a Mouse Up event
		if (!CameraMovement.IsCameraMoving() && Input.GetMouseButtonUp(0)) {

            // Swipe away Turret Upgrade Menu if active
            if (objectManager.TurretFocusMenu.isActive)
            {
                objectManager.TurretFocusMenu.SelectedTurret = null;
                return;
            }

            StartCoroutine(objectManager.TurretFactory.PlaceOrSelectTurret());
		}
	}

	/// <summary>
	/// Cast a ray to test if screenPosition is over any UI object in canvas. This is a replacement
	/// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
	/// </summary>
	private static bool IsPointerOverUiObject(Canvas canvas, Vector2 screenPosition) {
		// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
		// the ray cast appears to require only eventData.position.
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = screenPosition;
		
		GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
		List<RaycastResult> results = new List<RaycastResult>();
		uiRaycaster.Raycast(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}