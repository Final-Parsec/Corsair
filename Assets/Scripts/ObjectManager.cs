using UnityEngine;
using System.Collections.Generic;
using FinalParsec.Corsair.Maps;
using FinalParsec.Corsair;
using JetBrains.Annotations;
using System.Collections;
using System.IO;

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

    private WaveManager waveManager;
    public WaveManager WaveManager
    {
        get
        {
            if (waveManager == null)
                waveManager = GameObject.Find("Map").GetComponent<WaveManager>();
            return waveManager;
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
				pathfinding = new Pathfinding ();
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

	private WaveDisplay waveDisplay;
	public WaveDisplay WaveDisplay 
	{ 
		get{
			if(waveDisplay == null)
				waveDisplay = GameObject.Find("WaveDisplay").GetComponent<WaveDisplay>();
			return waveDisplay;
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

    /// <summary>
    ///     Backing field for <see cref="MapData" />.
    /// </summary>
	private IMapData mapData;
	
    /// <summary>
    ///     Gets information about the current map.
    /// </summary>
    public IMapData MapData 
	{
        get
        {
            return this.mapData ?? (this.mapData = new WashingtonDCMapData());
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
	    WaveDisplay.DeReference();
		ObjectManager.instance = null;
	}

    /// <summary>
    /// Loads resources with the given names.
    /// </summary>
    /// <typeparam name="T">The type of the resource to load.</typeparam>
    /// <param name="path">The path into Resources</param>
    /// <param name="textureNames">Names of the resources to load</param>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of resource names to resources.</returns>
    public static IDictionary<string, T> LoadResources<T>(string path, IEnumerable<string> textureNames)
        where T : Object
    {
        var dictionary = new Dictionary<string, T>();
        T def = Resources.Load<T>(path + "Default");

        foreach (var name in textureNames)
        {
            T tex = Resources.Load<T>(path + name);
            if (tex != null)
            {
                dictionary.Add(name, tex);
                Debug.Log("Loaded Asset " + name);
            }
            else if (def != null)
            {
                dictionary.Add(name, def);
                Debug.Log("Loaded Default Asset " + name);
            }
        }

        return dictionary;
    }
}