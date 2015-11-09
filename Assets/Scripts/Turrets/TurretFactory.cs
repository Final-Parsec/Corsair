using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TurretFactory : MonoBehaviour
{
	public List<GameObject> turretPrefabs;
	public List<int> turretCosts;
	
	// Set or check which type of turret the factory is currently creating.
	public TurretType TurretType { get; set; }
	
	private ObjectManager objectManager;
	
	public IEnumerator PlaceOrSelectTurret ()
	{
        Vector3 mousePosition = Input.mousePosition;

        Node cursorOnNode = objectManager.NodeManager.GetNodeFromLocation (Camera.main.ScreenToWorldPoint (mousePosition));
        if (cursorOnNode == null || objectManager.gameState.optionsOn)
        {
            yield break;
        }

		bool canBuild = objectManager.NodeManager.BlockNode (cursorOnNode.UnityPosition);

        if (canBuild &&
            turretCosts[(int)TurretType] <= objectManager.gameState.playerMoney)
        {
            
            yield return StartCoroutine(objectManager.Pathfinding.CheckAndUpdatePaths());

            if(objectManager.Pathfinding.pathsExist)
            {
                Vector3 correctedPosition = cursorOnNode.UnityPosition;
                correctedPosition.y = -((cursorOnNode.listIndexY / objectManager.NodeManager.size_y) + (cursorOnNode.listIndexX / objectManager.NodeManager.size_x));
                Turret turret = ((GameObject)Instantiate(turretPrefabs[(int)TurretType], correctedPosition, Quaternion.Euler(new Vector3(90, 0, 0)))).GetComponent<Turret>();
                turret.Msrp = turretCosts[(int)TurretType];
                turret.TurretType = TurretType;
                cursorOnNode.turret = turret;
                objectManager.gameState.playerMoney -= turretCosts[(int)TurretType];
                yield break;
            }

        }
		
		if (!canBuild)
		{
			objectManager.TurretFocusMenu.SelectedTurret = cursorOnNode.turret;
			yield break;
		}
		
		Debug.Log ("Unable to place turret at this location.");
		objectManager.NodeManager.UnBlockNode (cursorOnNode.UnityPosition);
	}

	// Use this for initialization
	void Start ()
	{
		objectManager = ObjectManager.GetInstance ();
	}
}