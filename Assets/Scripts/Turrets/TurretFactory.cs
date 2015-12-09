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
                turret.turretModel = (TurretModel)turretModels[(int)TurretType].Clone();
                turret.FillNodesInRange();
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
    
	void Start ()
	{
		objectManager = ObjectManager.GetInstance ();
	    PopulateTurretModels();
	}

    void PopulateTurretModels()
    {
        turretModels = new TurretModel[Enum.GetNames(typeof(TurretType)).Length];

        turretModels[(int)TurretType.Pistolman] = new TurretModel
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
            turretType = TurretType.Pistolman,
            UpgradeNames = new List<string> {"Range","Damage","Speed"},
            UpgradePaths = new Dictionary<string, int> { { "Range", 0} , { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Rifleman] = new TurretModel
        {
            damage = 3,
            range = 3,
            rateOfFire = 5,
            aoeDamage = 0,
            aoeRange = 0,
            damageOverTime = 0,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Ground | AttackOptionsFlags.Air,
            turretType = TurretType.Rifleman,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Cannon] = new TurretModel
        {
            damage = 0,
            range = 3,
            rateOfFire = 3,
            aoeDamage = 3,
            aoeRange = 1,
            // TODO: calculate aoeRange the same way as turret range.
            damageOverTime = 0,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Ground,
            turretType = TurretType.Cannon,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Netter] = new TurretModel
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
            turretType = TurretType.Netter,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Buckshot] = new TurretModel
        {
            damage = 8,
            range = 3,
            rateOfFire = 6,
            aoeDamage = 4,
            aoeRange = 1,
            damageOverTime = 0,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Air,
            turretType = TurretType.Buckshot,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Molotov] = new TurretModel
        {
            damage = 0,
            range = 2,
            rateOfFire = 4,
            aoeDamage = 0,
            aoeRange = 0,
            damageOverTime = 4,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Ground | AttackOptionsFlags.Air,
            turretType = TurretType.Molotov,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.Sirens] = new TurretModel
        {
            damage = 0,
            range = 2,
            rateOfFire = 2,
            aoeDamage = 0,
            aoeRange = 0,
            damageOverTime = 0,
            Slow = 0,
            SlowDuration = 0,
            MindControlDuration = 5,
            attackOptions = AttackOptionsFlags.Ground | AttackOptionsFlags.Air,
            turretType = TurretType.Sirens,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };

        turretModels[(int)TurretType.PiroThePirateHero] = new TurretModel
        {
            damage = 7,
            range = 4,
            rateOfFire = 6,
            aoeDamage = 3,
            aoeRange = 1,
            damageOverTime = 0,
            Slow = .2f,
            SlowDuration = 1.2f,
            MindControlDuration = 0,
            attackOptions = AttackOptionsFlags.Ground | AttackOptionsFlags.Air,
            turretType = TurretType.PiroThePirateHero,
            UpgradeNames = new List<string> { "Range", "Damage", "Speed" },
            UpgradePaths = new Dictionary<string, int> { { "Range", 0 }, { "Damage", 0 }, { "Speed", 0 } }
        };
    }
}