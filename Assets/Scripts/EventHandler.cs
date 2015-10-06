using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class EventHandler : MonoBehaviour
{
	private readonly int numberOfTurrets = Enum.GetNames (typeof(TurretType)).Length;
    private ObjectManager objectManager;
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
		if (objectManager.gameState.optionsOn || objectManager.gameState.gameOver || EventSystem.current.IsPointerOverGameObject(-1))
			return;
		
		// Left Click Down & Tuoch Event
		if (!CameraMovement.IsCameraMoving() && Input.GetMouseButtonUp(0)) {

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
}