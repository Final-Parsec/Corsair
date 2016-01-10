using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Assets.Scripts.Turrets;

public class TurretFactory : MonoBehaviour
{
	public List<GameObject> turretPrefabs;
	public List<int> turretCosts;
	
	// Set or check which type of turret the factory is currently creating.
	public TurretType TurretType { get; set; }
    private TurretModel[] turretModels;
	
	private ObjectManager objectManager;

    public IEnumerator PlaceOrSelectTurret()
    {
        Vector3 mousePosition = Input.mousePosition;

        Node cursorOnNode = objectManager.NodeManager.GetNodeFromLocation(Camera.main.ScreenToWorldPoint(mousePosition));

        if (cursorOnNode == null || objectManager.gameState.optionsOn)
        {
            yield break;
        }
        Debug.Log(cursorOnNode.listIndexX + ", " + cursorOnNode.listIndexY);

        bool canBuild = objectManager.NodeManager.BlockNode(cursorOnNode.UnityPosition);

        if (turretCosts[(int)TurretType] <= objectManager.gameState.playerMoney)
        {
            if (canBuild)
            {

                yield return StartCoroutine(objectManager.Pathfinding.CheckAndUpdatePaths());

                if (objectManager.Pathfinding.pathsExist)
                {
                    Vector3 correctedPosition = cursorOnNode.UnityPosition;
                    correctedPosition.y = -((cursorOnNode.listIndexY / objectManager.NodeManager.size_y) + (cursorOnNode.listIndexX / objectManager.NodeManager.size_x));
                    Turret turret = ((GameObject)Instantiate(turretPrefabs[(int)TurretType], correctedPosition, Quaternion.Euler(new Vector3(90, 0, 0)))).GetComponent<Turret>();
                    turret.Msrp = turretCosts[(int)TurretType];
                    turret.turretModel = (TurretModel)turretModels[(int)TurretType].Clone();
                    turret.FillNodesInRange();
                    cursorOnNode.turret = turret;
                    objectManager.gameState.playerMoney -= turretCosts[(int)TurretType];
                    yield break;
                }
                else
                {
                    AlertText.Alert("Can't Block The Path!");
                    objectManager.AudioManager.PlayBlock();
                }
            }
        }
        else
        {
            AlertText.Alert("You Need More $$$");
            objectManager.AudioManager.PlayMoney();
        }
		
		if (!canBuild)
		{
            if(objectManager.TurretFocusMenu.SelectedTurret == cursorOnNode.turret)
            {
                objectManager.TurretFocusMenu.SelectedTurret = null;
            }
            else
            {
                objectManager.TurretFocusMenu.SelectedTurret = cursorOnNode.turret;
                objectManager.GuiButtonMethods.CloseTurretMenu();
            }
			yield break;
		}
		
		Debug.Log ("Unable to place turret at this location.");
		objectManager.NodeManager.UnBlockNode (cursorOnNode.UnityPosition);
	}

    public IEnumerator SelectTurret()
    {
        Vector3 mousePosition = Input.mousePosition;

        Node cursorOnNode = objectManager.NodeManager.GetNodeFromLocation(Camera.main.ScreenToWorldPoint(mousePosition));

        if (cursorOnNode == null || objectManager.gameState.optionsOn)
        {
            yield break;
        }
        Debug.Log(cursorOnNode.listIndexX + ", " + cursorOnNode.listIndexY);
        
        if (objectManager.TurretFocusMenu.SelectedTurret == cursorOnNode.turret)
        {
            objectManager.TurretFocusMenu.SelectedTurret = null;
        }
        else
        {
            objectManager.TurretFocusMenu.SelectedTurret = cursorOnNode.turret;
            objectManager.GuiButtonMethods.CloseTurretMenu();
        }
        yield break;
        
    }

    void Start ()
	{
		objectManager = ObjectManager.GetInstance ();
	    PopulateTurretModels();
	}

    void PopulateTurretModels()
    {
        turretModels = new TurretModel[Enum.GetNames(typeof(TurretType)).Length];

        turretModels[(int)TurretType.Basic] = new TurretModel
        {
            damage = 1,
            range = 2,
            rateOfFire = 4,
            aoeDamage = 0,
            aoeRange = 0,
            damageOverTime = 0,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Ground | AttackOptionsFlags.Air,
            turretType = TurretType.Basic,
            UpgradeNames = new List<string> {"Range","Damage","Speed"},
            UpgradePaths = new Dictionary<string, int> { { "Range", 0} , { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Strong] = new TurretModel
        {
            damage = 3,
            range = 3,
            rateOfFire = 5,
            aoeDamage = 3,
            aoeRange = 1,
            damageOverTime = 0,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Ground | AttackOptionsFlags.Air,
            turretType = TurretType.Strong,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Deportation] = new TurretModel
        {
            damage = 0,
            range = 3,
            rateOfFire = 3,
            aoeDamage = 0,
            aoeRange = 0,
            // TODO: calculate aoeRange the same way as turret range.
            damageOverTime = 0,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 2,
            attackOptions = AttackOptionsFlags.Ground,
            turretType = TurretType.Deportation,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.RedTape] = new TurretModel
        {
            damage = 2,
            range = 1,
            rateOfFire = 4,
            aoeDamage = 0,
            aoeRange = 0,
            damageOverTime = 0,
            Slow = .3f,
            SlowDuration = 7.2f,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Ground | AttackOptionsFlags.Air,
            turretType = TurretType.RedTape,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };
    }
}