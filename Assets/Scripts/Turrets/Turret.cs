using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Turret : MonoBehaviour
{
	// Configurable
	public float accuracyError = 2.0f;
    public int aoeDamage = 0;
    public int aoeRange = 0;
	public int damage = 10;
    public int damageOverTime = 0;
	public float range = 5;
	public List<Node> nodesInRange = new List<Node>();
	public int rateOfFire = 5;
	public float Slow { get; set; }
	public float SlowDuration { get; set; }
	public int MindControlDuration { get; set; }

	public float focusMenuBlue = 0;
	public float focusMenuGreen = 175;
	public float focusMenuRed = 0;
	public GameObject projectileType;
	public Sprite selectedSprite;
    public Sprite levelOneSprite;
    public Sprite levelTwoSprite;
    public Sprite levelThreeSprite;
    public Sprite levelOneSpriteSelected;
    public Sprite levelTwoSpriteSelected;
    public Sprite levelThreeSpriteSelected;
	
	// Constants
	private const float MinAttackDelay = 0.1f;
	private const float MaxAttackDelay = 2f;
	
	// Internal
	private Sprite defaultSprite;
	private EnemyBase target = null;
	private float nextDamageEvent;
	private ObjectManager objectManager;	
	private static readonly object syncRoot = new object ();
	
	// Properties
	private float AttackDelay
	{
		get 
		{
			int inverted = rateOfFire;
			if (rateOfFire == 0) 
            { 
				return float.MaxValue;
			}
			else if (rateOfFire < 5)
            {
				inverted = rateOfFire + 2 * (5 - rateOfFire);
			}
			else if (rateOfFire > 5) 
            {
				inverted = rateOfFire - 2 * (rateOfFire - 5);
			}
			
			return (((float)inverted - 1f) / (10f - 1f)) * (MaxAttackDelay - MinAttackDelay) + .1f;
		}
	}

    public float AoeRange
    {
        get
        {
			float minRange = Mathf.Min(objectManager.MapData.nodeSize.x, objectManager.MapData.nodeSize.y) * 1.5f;
            float maxRange = minRange * 4f;

            float computedRange = (((float)aoeRange - 1f) / (10f - 1f)) * (maxRange - minRange) + minRange;
            computedRange = computedRange / transform.localScale.x;

            return computedRange;
        }
    }

    private int level = 0;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            switch (value)
            {
                case 1:
                    defaultSprite = levelOneSprite;
                    selectedSprite = levelOneSpriteSelected;
                    break;
                case 2:
                    defaultSprite = levelTwoSprite;
                    selectedSprite = levelTwoSpriteSelected;
                    break;
                case 3:
                    defaultSprite = levelThreeSprite;
                    selectedSprite = levelThreeSpriteSelected;
                    break;
            }

            if (this == objectManager.TurretFocusMenu.SelectedTurret)
            {
                this.GetComponent<SpriteRenderer>().sprite = selectedSprite;    
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            }
            
            level = value;
        }
    }
	 
    // How much the turret can be sold for. 
    // Based on upgrades, bling, and current market conditions.
    public int Msrp { get; set; }

    

    public TurretType TurretType { get; set; }

    public int UpgradeOneLevel { get; set; }
    public int UpgradeTwoLevel { get; set; }
    public int UpgradeThreeLevel { get; set; }


	public void UpgradeTurret(Upgrade upgrade, int upgradeLevel)
	{
		if (upgrade.Cost > objectManager.gameState.playerMoney)
			return;
		objectManager.gameState.playerMoney -= (int)upgrade.Cost;
		Msrp += (int)upgrade.Cost / 2;

		switch(upgradeLevel)
		{
		case 1:
			UpgradeOneLevel++;
			break;
		case 2:
			UpgradeTwoLevel++;
			break;
		case 3:
			UpgradeThreeLevel++;
			break;
		}

		Level++;     

		foreach(Stat stat in upgrade.stats)
		{
			switch(stat.AttribId)
			{
			case Attribute.Range:
				range += (int)stat.Value;
				FillNodesInRange();
				objectManager.TurretRange.ChangeSprite((int)range);
				break;
			case Attribute.RateOfFire:
				rateOfFire += (int)stat.Value;
				break;
			case Attribute.AoeDamage:
				aoeDamage += (int)stat.Value;
				break;
			case Attribute.AoeRange:
				aoeRange += (int)stat.Value;
				break;
			case Attribute.Damage:
				damage += (int)stat.Value;
				break;
			case Attribute.DamageOverTime:
				damageOverTime += (int)stat.Value;
				break;
			case Attribute.Slow:
				Slow += stat.Value;
				break;
			case Attribute.SlowDuration:
				SlowDuration += stat.Value;
				break;
			case Attribute.MindControlDuration:
				MindControlDuration += (int)stat.Value;
				break;
			default:
				Debug.Log("Unknown upgrade " + stat.AttribId);
				break;
			}
		}
	}


	// Runs when entity is Instantiated
	void Awake()
	{
		objectManager = ObjectManager.GetInstance ();
		objectManager.AddEntity (this);
		FillNodesInRange();
	}
	
	public void Deselect()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        Level = level;
	}
	
	void Fire (EnemyBase myTarget)
	{
        var targetPosition = myTarget.transform.position;
		var aimError = Random.Range (-accuracyError, accuracyError);
		var aimPoint = new Vector3 (targetPosition.x + aimError, targetPosition.y, targetPosition.z + aimError);
		nextDamageEvent = Time.time + AttackDelay;
        ////GameObject projectileObject = Instantiate (projectileType, transform.position, Quaternion.LookRotation (targetPosition)) as GameObject;
        ////Projectile projectile = projectileObject.GetComponent<Projectile> ();
        var projectile = projectileType.GetObjectFromPool<Projectile>(
			projectileType.name,
            transform.position,
            Quaternion.LookRotation(targetPosition));
		projectile.gameObject.name = projectileType.name;
		projectile.Damage = damage;
        projectile.DamageOverTime = damageOverTime;
        projectile.Slow = Slow;
        projectile.SlowDuration = SlowDuration;
		projectile.target = myTarget;
		projectile.targetPosition = aimPoint;
        projectile.Owner = this;

        // Turret is AoE enabled. Do stuff.
        if (aoeDamage > 0 && aoeRange > 0)
        {
            projectile.AoeDamage = aoeDamage;
            projectile.AoeRange = AoeRange;
        }
	}

	void OnNodeExit ()
	{
		if (target != null || !IsTargetInRange()) 
		{
			removeTargetCallbacks();
			target = null;
		}
	}	

	void OnTargetKilled ()
	{
		removeTargetCallbacks();
		target = null;
	}	

	private void removeTargetCallbacks()
	{
		if (target != null)
		{
			target.Killed -= OnTargetKilled;
			target.NodeExit -= OnNodeExit;
		}
	}
	
	public void Select()
	{
        this.GetComponent<SpriteRenderer>().sprite = selectedSprite;
	}

	// Use this for initialization
	void Start ()
	{
		defaultSprite = GetComponent<SpriteRenderer>().sprite;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(objectManager.gameState.GameSpeed == GameSpeed.Paused)
		{
			nextDamageEvent += Time.deltaTime;
			return;
		}

		if (target != null && target.gameObject.activeSelf)
		{
			if (Time.time >= nextDamageEvent)
			{
				Fire(target);
				nextDamageEvent = Time.time + (AttackDelay * 1f/(float)objectManager.gameState.GameSpeed); 
			}
			         
		}
		else
		{

			FindNewTarget();
		}
			
	}

	public string GetStats()
	{
		string str = "";

		str += StatInfo.statInfo[Attribute.Damage].Acronym + "=" + damage + "\t" ;
		str += StatInfo.statInfo[Attribute.Range].Acronym + "=" + range + "\t" ;
		str += StatInfo.statInfo[Attribute.RateOfFire].Acronym + "=" + rateOfFire + "\t" ;
		str += aoeDamage > 0?StatInfo.statInfo[Attribute.AoeDamage].Acronym + "=" + aoeDamage + "\t":"";
		str += aoeRange > 0?StatInfo.statInfo[Attribute.AoeRange].Acronym + "=" + aoeRange + "\t":"";
		str += damageOverTime > 0?StatInfo.statInfo[Attribute.DamageOverTime].Acronym + "=" + damageOverTime + "\t":"";
		str += Slow > 0?StatInfo.statInfo[Attribute.Slow].Acronym + "=" + Slow + "\t":"";
		str += SlowDuration > 0?StatInfo.statInfo[Attribute.SlowDuration].Acronym + "=" + SlowDuration + "\t":"";
		str += MindControlDuration > 0?StatInfo.statInfo[Attribute.MindControlDuration].Acronym + "=" + MindControlDuration + "\t":"";

		return str;
	}

	public void FillNodesInRange()
	{
		nodesInRange.Clear();

		Node onNode = objectManager.NodeManager.GetNodeFromLocation(transform.position);

		Vector3 start = new Vector3 (onNode.unityPosition.x + ((objectManager.MapData.nodeSize.x / 2) * (range - 1)),
		                             0,
		                             onNode.unityPosition.z - ((objectManager.MapData.nodeSize.y / 2) * (range - 1) + objectManager.MapData.nodeSize.y));

		Vector3 currentLoc = start;

		//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

		for(int i = 0; i < range * 2 + 1; i++)
		{
			if(i < range - 1 )
			{

				for(int l = 0; l < 3 + i * 2; l++)
				{
					//Instantiate(cube, currentLoc, Quaternion.Euler(Vector3.zero));

					Node node = objectManager.NodeManager.GetNodeFromLocation(currentLoc);
					if(node != null)
					{
						nodesInRange.Add(node);
					}
					currentLoc.x += objectManager.MapData.nodeSize.x / 2;
					currentLoc.z += objectManager.MapData.nodeSize.y / 2;
				}
				currentLoc.x = start.x - objectManager.MapData.nodeSize.x;
				currentLoc.z = start.z;
				start = currentLoc;
			}
			else if(i < range + 1)
			{
				for(int l = 0; l < range * 2 + 1; l++)
				{
					//Instantiate(cube, currentLoc, Quaternion.Euler(Vector3.zero));

					Node node = objectManager.NodeManager.GetNodeFromLocation(currentLoc);
					if(node != null)
					{
						nodesInRange.Add(node);
					}
					currentLoc.x += objectManager.MapData.nodeSize.x / 2;
					currentLoc.z += objectManager.MapData.nodeSize.y / 2;
				}
				currentLoc.x = start.x - objectManager.MapData.nodeSize.x / 2;
				currentLoc.z = start.z + objectManager.MapData.nodeSize.y / 2;
				start = currentLoc;
			}
			else
			{
				for(int l = 0; l < 3 + ((range * 2 ) - i) * 2; l++)
				{
					//Instantiate(cube, currentLoc, Quaternion.Euler(Vector3.zero));

					Node node = objectManager.NodeManager.GetNodeFromLocation(currentLoc);
					if(node != null)
					{
						nodesInRange.Add(node);
					}
					currentLoc.x += objectManager.MapData.nodeSize.x / 2;
					currentLoc.z += objectManager.MapData.nodeSize.y / 2;
				}
				currentLoc.x = start.x;
				currentLoc.z = start.z + objectManager.MapData.nodeSize.y;
				start = currentLoc;
			}
		}
	}
	 

	private void FindNewTarget()
	{
		foreach (Node node in nodesInRange)
		{
			if(node.enemie != null)
			{
				removeTargetCallbacks();
				target = node.enemie;
				target.Killed += OnTargetKilled;
				target.NodeExit += OnNodeExit;
			}
		}
	}

	private bool IsTargetInRange()
	{
		if (target == null)
		{
			return false;
		}

		return nodesInRange.Contains(target.onNode);
	}
}