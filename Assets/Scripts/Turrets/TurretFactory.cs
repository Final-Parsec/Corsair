using UnityEngine;
using System.Collections.Generic;

public class TurretFactory : MonoBehaviour
{
	public List<GameObject> turretPrefabs;
	public List<int> turretCosts;
	
	// Set or check which type of turret the factory is currently creating.
	public TurretType TurretType { get; set; }
	
	private ObjectManager objectManager;
	
	public void PlaceOrSelectTurret (Vector3 mousePosition)
	{
		Node cursorOnNode = objectManager.NodeManager.GetNodeFromLocation (Camera.main.ScreenToWorldPoint (mousePosition));
		if(cursorOnNode == null || objectManager.gameState.optionsOn)
			return;

		bool canBuild = objectManager.NodeManager.BlockNode (cursorOnNode.UnityPosition);

		if (turretCosts [(int)TurretType] <= objectManager.gameState.playerMoney && 
            objectManager.Pathfinding.CheckAndUpdatePaths () && 
            canBuild) 
        {
			Vector3 correctedPosition = cursorOnNode.UnityPosition;
			correctedPosition.y = -((cursorOnNode.listIndexY / objectManager.NodeManager.size_y) + (cursorOnNode.listIndexX / objectManager.NodeManager.size_x));
			Turret turret = ((GameObject) Instantiate (turretPrefabs [(int)TurretType], correctedPosition, Quaternion.Euler (new Vector3 (90, 0, 0)))).GetComponent<Turret>();
			turret.Msrp = turretCosts [(int)TurretType];
            turret.TurretType = TurretType;
			cursorOnNode.turret = turret;
			objectManager.gameState.playerMoney -= turretCosts [(int)TurretType];
			return;
		} 
		
		if (!canBuild)
		{
			objectManager.TurretFocusMenu.SelectedTurret = cursorOnNode.turret;
			return;
		}
		
		Debug.Log ("Unable to place turret at this location.");
		objectManager.NodeManager.UnBlockNode (cursorOnNode.UnityPosition);
		objectManager.Pathfinding.CheckAndUpdatePaths ();
	}

	// Use this for initialization
	void Start ()
	{
		objectManager = ObjectManager.GetInstance ();
	}
}