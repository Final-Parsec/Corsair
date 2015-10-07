using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
	private readonly int numberOfTurrets = Enum.GetNames (typeof(TurretType)).Length;
    private ObjectManager objectManager;
	private Canvas canvas;
    private readonly List<string> turretFocusMenuObjects = new List<string>()
	{
		"UpgradeOne",
		"UpgradeTwo",
		"UpgradeThree",
		"Sell"
	};

	// Initialization
	void Start ()
	{
		objectManager = ObjectManager.GetInstance ();
		canvas = GameObject.Find("Canvas").GetComponent("Canvas") as Canvas;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Escape (Back button on Android)
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}

		//Debug.Log (objectManager.gameState.optionsOn);
		if (objectManager.gameState.optionsOn || objectManager.gameState.gameOver || IsPointerOverUIObject(canvas, new Vector2(Input.mousePosition.x,Input.mousePosition.y))
		    || (Input.touchCount == 1 && IsPointerOverUIObject(canvas, Input.touches[0].position)))
			return;
		
		// Left Click Down & Tuoch Event
		if (!CameraMovement.IsCameraMoving() && Input.GetMouseButtonUp(0)) {
			if(objectManager.TurretFocusMenu.isActive){
				objectManager.GuiButtonMethods.UpgradeMenuBackPressed();
				return;
			}

			Vector3 mousePosition = Input.mousePosition;
			
			if (objectManager.TurretFocusMenu.SelectedTurret == null) {
				objectManager.TurretFactory.PlaceOrSelectTurret(mousePosition);
			}
		}
		
		// Check if a number has been pressed and change turret type we're producing.
		for (int i = 1; i <= numberOfTurrets; i++) {
			TurretType associatedType = (TurretType)(i - 1);  // Subtract one because enum indexes begin start 0.
			if (Input.GetKeyDown ("" + i)) {
				objectManager.TurretFactory.TurretType = associatedType;
				Debug.Log ("Selected " + objectManager.TurretFactory.TurretType);
			}
		}
	}

	/// <summary>
	/// Cast a ray to test if screenPosition is over any UI object in canvas. This is a replacement
	/// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
	/// </summary>
	private static bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition) {
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