using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour
{
	public TextMesh enemyDeathInt;

	// Prefabs, GameObjects, and Textures
	public List<GameObject> enemyPrefabs;
	public List<GameObject> bossPrefabs;
	public List<GameObject> obstaclePrefabs;

	public Transform enemySpawnTransform;
	public Transform destinationTransform;
	public Texture healthTexture;
	public Node destinationNode;
	public Node enemySpawnNode;

	// Grid/Node
	private ObjectManager objectManager;
	public NodeGenerator nodeGenerator;

	// Wave control attributes
	public List<Wave> upcomingWaves = new List<Wave> ();
	public List<Wave> currentWaves = new List<Wave> ();
	public float waveSpawnDelay = 25;
	private float nextWaveSpawnEvent;
	public bool playerTriggeredWave;

	public bool isIsoGrid = true;
	private Color[] colors;

	[HideInInspector]
	public GoogleMobileAdsScript ad;

	void Awake()
	{
		objectManager = ObjectManager.GetInstance ();

		NodeGenerator nodeGenerator = new NodeGenerator(objectManager.MapData);
		nodeGenerator.MakeNodes();

		MakeWaves ();
		MakeObstacles ();

		destinationTransform.position = nodeGenerator.nodes[size_x-2, 0].unityPosition;
		destinationTransform.position = new Vector3(destinationTransform.position.x, -.9f, destinationTransform.position.z);

		enemySpawnTransform.position = nodeGenerator.nodes[0, size_z-1].unityPosition;
		enemySpawnTransform.position = new Vector3(enemySpawnTransform.position.x, -.9f, enemySpawnTransform.position.z);
	}
	
	// Use this for initialization
	void Start () {
		ad = GameObject.FindGameObjectWithTag("Ad").GetComponent<GoogleMobileAdsScript>();
		ad.RequestInterstitial ();

		foreach (Node node in nodeGenerator.nodes){
			Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), node.unityPosition, Quaternion.Euler(Vector3.zero));
		}
	}

	/// <summary>
	/// Displays the grid.
	/// </summary>
	void OnGUI ()
	{
		foreach (EnemyBase Gob in objectManager.ThingsWithHealthBars()) {
			//Health Bar
			float healthRatio = (((float)Gob.Health) / ((float)Gob.maxHealth));
			if (healthRatio != 1) {
				Vector2 objSize = Gob.GetPixelSize ();
				float width = Gob.healthBarSize.x;
				float height = Gob.healthBarSize.y;
	
				width = width * healthRatio;
				Vector3 wantedPos = Camera.main.WorldToScreenPoint (Gob.transform.position);
	
				GUI.color = new Color (2 * (1 - healthRatio), 2 * healthRatio, 0);
				GUI.DrawTexture (new Rect (wantedPos.x - width / 2, Screen.height - wantedPos.y - objSize.y / 2, width, height), healthTexture);
			}
		}


	}

	/// <summary>
	/// Initializes the waves and choses random enemies for each wave.
	/// The first wave always consists of basic enemies.
	/// </summary>
	private void MakeWaves ()
	{
		int random;
		Wave wave;

        for (int x = 1; x < objectManager.gameState.numberOfWaves; x++)
        {
			if (x % 10 == 0) {
				random = UnityEngine.Random.Range ((int)BossType.Start+1, (int)BossType.Max);
				wave = new Wave ((BossType)random, 1f, 1);
			} else {
				random = UnityEngine.Random.Range ((int)EnemyType.Start+1, (int)EnemyType.Max);
				wave = new Wave ((EnemyType)random, 1f, 10);
			}
			
			upcomingWaves.Add (wave);
		}
	}

	private void MakeObstacles()
	{
		System.Random random = new System.Random();  // Needs to be instantiated outside the loop. http://stackoverflow.com/questions/5398336/random-number-generator-always-picks-the-same-value-when-run-inside-a-loop
	
		if(objectManager.gameState.MapType == MapType.Obstacles){
			for(int x=0; x<size_x; x++){
				for(int z=0; z<size_z; z++){
					if((x%5==3 || x%5==4) && (z%5==3 || z%5==4)){
						nodes[x,z].isBuildable = false;
						nodes[x,z].isWalkable = false;

						if(x%5==4 &&
						   z%5==4 && 
						   !(Camera.main.WorldToScreenPoint(nodes[x,z].unityPosition).y <= 10 ||
	                         Camera.main.WorldToScreenPoint(nodes[x,z].unityPosition).y >= Screen.height - Screen.height * .20)) {
							Vector3 spawnPosition = new Vector3(nodes[x,z].unityPosition.x - nodeSize.x/2, -3, nodes[x,z].unityPosition.z - nodeSize.y/2);
							Array values = Enum.GetValues(typeof(ObstacleType));
							ObstacleType randomObstacle = (ObstacleType)values.GetValue(random.Next(values.Length));
							GameObject obstaclePrefab = obstaclePrefabs[(int)randomObstacle];
							Instantiate (obstaclePrefab, spawnPosition, obstaclePrefab.transform.rotation);							
						}
						else if(x%5==4 && z%5==4) {

							if(!(Camera.main.WorldToScreenPoint (nodes[x-1,z-1].unityPosition).y <= 10 ||
							      Camera.main.WorldToScreenPoint (nodes[x-1,z-1].unityPosition).y >= Screen.height - Screen.height * .21)){
								nodes[x,z].isBuildable = true;
								nodes[x,z].isWalkable = true;

								nodes[x-1,z].isBuildable = true;
								nodes[x-1,z].isWalkable = true;

								nodes[x,z-1].isBuildable = true;
								nodes[x,z-1].isWalkable = true;

								nodes[x-1,z-1].isBuildable = true;
								nodes[x-1,z-1].isWalkable = true;
							}
						}
					}
				}
			}
		}



	}

	/// <summary>
	/// Starts a wave from the upcomming Waves list.
	/// Ends the game when all the waves have been ran, all the enemies are dead, and the player didn't lose.
	/// </summary>
	public void SpawnWave ()
	{
		if (upcomingWaves.Count > 0) {
			Wave wave = upcomingWaves [0];
			upcomingWaves.RemoveAt (0);
			currentWaves.Add (wave);
			objectManager.gameState.waveCount++;
		}
	}
	
	/// <summary>
	/// Creates enemies.
	/// </summary>
	public void CreateEnemies ()
	{
		if (Time.time >= nextWaveSpawnEvent || playerTriggeredWave) {
			objectManager.WaveWheel.UpdateSpriteImages = true;

			playerTriggeredWave = false;
			nextWaveSpawnEvent = Time.time + waveSpawnDelay;
			SpawnWave ();
		}

		foreach (Wave wave in currentWaves) {

			if(objectManager.gameState.GameSpeed == GameSpeed.Paused)
			{
				wave.nextEnemySpawnEvent += Time.deltaTime;
			}
			else 
			{
				wave.nextEnemySpawnEvent -= (Time.deltaTime * (float)objectManager.gameState.GameSpeed) - Time.deltaTime;
			}

			if (Time.time >= wave.nextEnemySpawnEvent && wave.numberOfEnemies > 0) {
				GameObject enemy;
				if(wave.enemyType == EnemyType.Max){
					enemy = bossPrefabs [(int)wave.bossType];
				}else{
					enemy = enemyPrefabs [(int)wave.enemyType];
				}

				Vector3 moveSpawnOffScreen = enemySpawnTransform.position;
				moveSpawnOffScreen.x-=10;
				moveSpawnOffScreen.y-=10;

				Instantiate (enemy, moveSpawnOffScreen, Quaternion.Euler (new Vector3 (90, 45, 0)));
				wave.nextEnemySpawnEvent = Time.time + wave.spawnDelay;
				wave.numberOfEnemies--;
			}
		}

		currentWaves.RemoveAll (a => a.numberOfEnemies <= 0);
	}

	/// <summary>
	/// Gets the tile from location.
	/// </summary>
	public Node GetNodeFromLocation (Vector3 location)
	{
		
		int xIndex = (int)Mathf.Floor ((location.x - left.x) / nodeSize.x);
		int zIndex = size_z + ((int)Mathf.Floor ((location.z - left.y) / nodeSize.y));

		// out of bounds check
		if (zIndex >= size_z || zIndex < 0 || xIndex >= size_x || xIndex < 0)
			return null;

		
		return nodes [xIndex, zIndex];
	}

	/// <summary>
	/// Gets the tile from location.
	/// </summary>
	public Node GetClosestNode (Vector3 location)
	{
		
		int xIndex = (int)Mathf.Floor ((location.x - left.x) / nodeSize.x);
		int zIndex = size_z + ((int)Mathf.Floor ((location.z - left.y) / nodeSize.y));
		
		if (xIndex >= size_x)
			xIndex = size_x - 1;
		else if (xIndex < 0)
			xIndex = 0;
		
		if (zIndex >= size_z)
			zIndex = size_z - 1;
		else if (zIndex < 0)
			zIndex = 0;
		
		
		return nodes [xIndex, zIndex];
	}

	/// <summary>
	/// Blocks the node at the given position.
	/// returns true if the object can be built 
	/// else false	
	/// </summary>
	public bool BlockNode (Vector3 position)
	{
		Node node = GetNodeFromLocation (position);

		if (node == null || !node.isBuildable)
			return false;

		node.isWalkable = false;
		node.isBuildable = false;

		UpdateGridNode(node);
		return true;
	}

	public void UnBlockNode (Vector3 position)
	{
		Node node = GetNodeFromLocation (position);

		node.isWalkable = true;
		node.isBuildable = true;

		UpdateGridNode(node);
	}
	
	public void Update ()
	{
		if(objectManager.gameState.gameStarted && !objectManager.gameState.gameOver){

			if(objectManager.gameState.GameSpeed == GameSpeed.Paused)
			{
				nextWaveSpawnEvent += Time.deltaTime;
			}
			else 
			{
				nextWaveSpawnEvent -= (Time.deltaTime * (float)objectManager.gameState.GameSpeed) - Time.deltaTime;
			}


			objectManager.gameState.nextWaveCountDown = (int)(nextWaveSpawnEvent - Time.time);
			CreateEnemies ();
		}
	}

	public void UpdateGridNode(Node node)
	{
		float c = node.listIndex.x + node.listIndex.z;
		if (node.isBuildable) 
			gridTexture.SetPixel((int)node.listIndex.x, (int)node.listIndex.z, new Color(c%2, c%2, c%2, .3f));
		else
			gridTexture.SetPixel((int)node.listIndex.x, (int)node.listIndex.z, Color.clear);

		gridTexture.Apply ();
	}
}
