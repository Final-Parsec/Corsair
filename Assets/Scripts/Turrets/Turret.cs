using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Turrets;

public class Turret : MonoBehaviour
{
    public TurretModel turretModel;
    public List<Node> nodesInRange = new List<Node>();

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
	
	// Properties
	private float AttackDelay
	{
		get 
		{
			int inverted = turretModel.rateOfFire;
			if (turretModel.rateOfFire == 0) 
            { 
				return float.MaxValue;
			}
			else if (turretModel.rateOfFire < 5)
            {
				inverted = turretModel.rateOfFire + 2 * (5 - turretModel.rateOfFire);
			}
			else if (turretModel.rateOfFire > 5) 
            {
				inverted = turretModel.rateOfFire - 2 * (turretModel.rateOfFire - 5);
			}
			
			return (((float)inverted - 1f) / (10f - 1f)) * (MaxAttackDelay - MinAttackDelay) + .1f;
		}
	}

    public float AoeRange
    {
        get
        {
			float minRange = Mathf.Min(objectManager.MapData.NodeSize.x, objectManager.MapData.NodeSize.y) * 1.5f;
            float maxRange = minRange * 4f;

            float computedRange = (((float)turretModel.aoeRange - 1f) / (10f - 1f)) * (maxRange - minRange) + minRange;
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

	public bool Upgrade(Upgrade upgrade)
	{
        if (upgrade == null || 
            upgrade.Cost > objectManager.gameState.playerMoney)
        {
            return false;
        }

		objectManager.gameState.playerMoney -= (int)upgrade.Cost;
		Msrp += (int)upgrade.Cost / 2;

		Level++;     

		foreach(Stat stat in upgrade.stats)
		{
			switch(stat.AttribId)
			{
			case Attribute.Range:
                    turretModel.range += (int)stat.Value;
				FillNodesInRange();
				objectManager.TurretRange.ChangeSprite((int)turretModel.range);
				break;
			case Attribute.RateOfFire:
                    turretModel.rateOfFire += (int)stat.Value;
				break;
			case Attribute.AoeDamage:
                    turretModel.aoeDamage += (int)stat.Value;
				break;
			case Attribute.AoeRange:
                    turretModel.aoeRange += (int)stat.Value;
				break;
			case Attribute.Damage:
                    turretModel.damage += (int)stat.Value;
				break;
			case Attribute.DamageOverTime:
                    turretModel.damageOverTime += (int)stat.Value;
				break;
			case Attribute.Slow:
                    turretModel.Slow += stat.Value;
				break;
			case Attribute.SlowDuration:
                    turretModel.SlowDuration += stat.Value;
				break;
			case Attribute.MindControlDuration:
                    turretModel.MindControlDuration += (int)stat.Value;
				break;
			default:
				Debug.Log("Unknown upgrade " + stat.AttribId);
				break;
			}
		}

        return true;
	}


	// Runs when entity is Instantiated
	void Awake()
	{
		objectManager = ObjectManager.GetInstance ();
		objectManager.AddEntity (this);
	}
	
	public void Deselect()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        Level = level;
	}
	
	void Fire (EnemyBase myTarget)
	{
        var targetPosition = myTarget.transform.position;
		nextDamageEvent = Time.time + AttackDelay;
        ////GameObject projectileObject = Instantiate (projectileType, transform.position, Quaternion.LookRotation (targetPosition)) as GameObject;
        ////Projectile projectile = projectileObject.GetComponent<Projectile> ();
        var projectile = projectileType.GetObjectFromPool<Projectile>(
			projectileType.name,
            transform.position,
            Quaternion.LookRotation(targetPosition));
		projectile.gameObject.name = projectileType.name;
		projectile.Damage = turretModel.damage;
        projectile.DamageOverTime = turretModel.damageOverTime;
        projectile.Slow = turretModel.Slow;
        projectile.SlowDuration = turretModel.SlowDuration;
		projectile.target = myTarget;
		projectile.targetPosition = targetPosition;
        projectile.Owner = this;

        // Turret is AoE enabled. Do stuff.
        if (turretModel.aoeDamage > 0 && turretModel.aoeRange > 0)
        {
            projectile.AoeDamage = turretModel.aoeDamage;
            projectile.AoeRange = AoeRange;
        }
	}

	void OnNodeExit ()
	{
		if (target != null || !IsTargetInRange()) 
		{
			RemoveTargetCallbacks();
			target = null;
		}
	}	

	void OnTargetKilled ()
	{
		RemoveTargetCallbacks();
		target = null;
	}	

	private void RemoveTargetCallbacks()
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

		str += StatInfo.statInfo[Attribute.Damage].Acronym + "=" + turretModel.damage + "\t" ;
		str += StatInfo.statInfo[Attribute.Range].Acronym + "=" + turretModel.range + "\t" ;
		str += StatInfo.statInfo[Attribute.RateOfFire].Acronym + "=" + turretModel.rateOfFire + "\t" ;
		str += turretModel.aoeDamage > 0?StatInfo.statInfo[Attribute.AoeDamage].Acronym + "=" + turretModel.aoeDamage + "\t":"";
		str += turretModel.aoeRange > 0?StatInfo.statInfo[Attribute.AoeRange].Acronym + "=" + turretModel.aoeRange + "\t":"";
		str += turretModel.damageOverTime > 0?StatInfo.statInfo[Attribute.DamageOverTime].Acronym + "=" + turretModel.damageOverTime + "\t":"";
		str += turretModel.Slow > 0?StatInfo.statInfo[Attribute.Slow].Acronym + "=" + turretModel.Slow + "\t":"";
		str += turretModel.SlowDuration > 0?StatInfo.statInfo[Attribute.SlowDuration].Acronym + "=" + turretModel.SlowDuration + "\t":"";
		str += turretModel.MindControlDuration > 0?StatInfo.statInfo[Attribute.MindControlDuration].Acronym + "=" + turretModel.MindControlDuration + "\t":"";

		return str;
	}

	public void FillNodesInRange()
	{
		nodesInRange.Clear();

		Node onNode = objectManager.NodeManager.GetNodeFromLocation(transform.position);

		Vector3 start = new Vector3 (onNode.UnityPosition.x + ((objectManager.MapData.NodeSize.x / 2) * (turretModel.range - 1)),
		                             0,
		                             onNode.UnityPosition.z - ((objectManager.MapData.NodeSize.y / 2) * (turretModel.range - 1) + objectManager.MapData.NodeSize.y));

		Vector3 currentLoc = start;

		//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

		for(int i = 0; i < turretModel.range * 2 + 1; i++)
		{
			if(i < turretModel.range - 1 )
			{

				for(int l = 0; l < 3 + i * 2; l++)
				{
					//Instantiate(cube, currentLoc, Quaternion.Euler(Vector3.zero));

					Node node = objectManager.NodeManager.GetNodeFromLocation(currentLoc);
					if(node != null)
					{
						nodesInRange.Add(node);
					}
					currentLoc.x += objectManager.MapData.NodeSize.x / 2;
					currentLoc.z += objectManager.MapData.NodeSize.y / 2;
				}
				currentLoc.x = start.x - objectManager.MapData.NodeSize.x;
				currentLoc.z = start.z;
				start = currentLoc;
			}
			else if(i < turretModel.range + 1)
			{
				for(int l = 0; l < turretModel.range * 2 + 1; l++)
				{
					//Instantiate(cube, currentLoc, Quaternion.Euler(Vector3.zero));

					Node node = objectManager.NodeManager.GetNodeFromLocation(currentLoc);
					if(node != null)
					{
						nodesInRange.Add(node);
					}
					currentLoc.x += objectManager.MapData.NodeSize.x / 2;
					currentLoc.z += objectManager.MapData.NodeSize.y / 2;
				}
				currentLoc.x = start.x - objectManager.MapData.NodeSize.x / 2;
				currentLoc.z = start.z + objectManager.MapData.NodeSize.y / 2;
				start = currentLoc;
			}
			else
			{
				for(int l = 0; l < 3 + ((turretModel.range * 2 ) - i) * 2; l++)
				{
					//Instantiate(cube, currentLoc, Quaternion.Euler(Vector3.zero));

					Node node = objectManager.NodeManager.GetNodeFromLocation(currentLoc);
					if(node != null)
					{
						nodesInRange.Add(node);
					}
					currentLoc.x += objectManager.MapData.NodeSize.x / 2;
					currentLoc.z += objectManager.MapData.NodeSize.y / 2;
				}
				currentLoc.x = start.x;
				currentLoc.z = start.z + objectManager.MapData.NodeSize.y;
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
				RemoveTargetCallbacks();
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