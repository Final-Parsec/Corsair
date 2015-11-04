using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class ObjectManager
{
    private static ObjectManager instance;

	public List<EnemyBase> enemies = new List<EnemyBase> ();
	public List<Turret> turrets = new List<Turret> ();
	
    [NotNull]
    public GameState gameState;

	private Map map;
	public Map Map
	{
		get{
			if(map == null)
				map = GameObject.Find ("Map").GetComponent<Map> ();
			return map;
		}
	}

	private EventHandler eventHandler;
	public EventHandler EventHandler
	{
		get{
			if(eventHandler == null)
				eventHandler = GameObject.Find ("Map").GetComponent<EventHandler> ();
			return eventHandler;
		}
	}

	private Pathfinding pathfinding;
	public Pathfinding Pathfinding
	{
		get{
			if(pathfinding == null)
				pathfinding = GameObject.Find ("Map").GetComponent<Pathfinding> ();
			return pathfinding;
		}
	}

	private TurretFactory turretFactory;
	public TurretFactory TurretFactory
	{
		get{
			if(turretFactory == null)
				turretFactory = GameObject.Find ("Map").GetComponent<TurretFactory> ();
			return turretFactory;
		}
	}
    
	private TurretFocusMenu turretFocusMenu;
    public TurretFocusMenu TurretFocusMenu 
	{ 
		get{
			if(turretFocusMenu == null)
				turretFocusMenu = GameObject.Find("UpgradeMenu").GetComponent<TurretFocusMenu>();
			return turretFocusMenu;
		}
	}

	private WaveWheel waveWheel;
	public WaveWheel WaveWheel 
	{ 
		get{
			if(waveWheel == null)
				waveWheel = GameObject.Find("WaveWheel").GetComponent<WaveWheel>();
			return waveWheel;
		} 
	}

	private GuiButtonMethods guiButtonMethods;
	public GuiButtonMethods GuiButtonMethods 
	{ 
		get{
			if(guiButtonMethods == null)
				guiButtonMethods = GameObject.Find("Main Camera").GetComponent<GuiButtonMethods>();
			return guiButtonMethods;
		} 
	}

	private IMapData mapData;
	public IMapData MapData 
	{ 
		get{
			if(mapData == null)
				mapData = new HardcodedMapData("Baer World", new Vector2(128,64), true, null);
			return mapData;
		} 
	}

	private NodeManager nodeManager;
	public NodeManager NodeManager 
	{ 
		get{
			if(nodeManager == null)
				nodeManager = new NodeManager();
			return nodeManager;
		} 
	}

	private RangeVisual turretRange;
	public RangeVisual TurretRange 
	{ 
		get{
			if(turretRange == null)
				turretRange = GameObject.Find("TurretRange").GetComponent<RangeVisual>();
			return turretRange;
		} 
	}

	public ObjectManager ()
	{
		ObjectManager.instance = this;
		gameState = new GameState(1, 999, MapType.Open);
	}
	
	/// <summary>
	///     Gets the singleton instance of <see cref="ObjectManager"/>.
	/// </summary>
	/// <returns>The singleton instance.</returns>
	[NotNull]
    public static ObjectManager GetInstance ()
	{
	    return ObjectManager.instance ?? new ObjectManager();
	}

    /// <summary>
	/// Adds the enemy.
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	public void AddEntity (EnemyBase entity) {
		enemies.Add (entity);
	}

	public void AddEntity (Turret entity) {
		turrets.Add (entity);
	}

	public void DeReference (EnemyBase entity) {
		if (entity is EnemyBase) {
			enemies.Remove ((EnemyBase)entity);
		}
	}
	
	public void DeReference(Turret turret) {
		turrets.Remove(turret);
	}
	
	public List<EnemyBase> ThingsWithHealthBars ()
	{
		return enemies;
	}

	public void DestroySinglton ()
	{
		ObjectManager.instance = null;
	}
}