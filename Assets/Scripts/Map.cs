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

	public Transform destinationTransform;
	public Texture healthTexture;
	public Node destinationNode;
	public Node[] enemySpawnNodes;

	// Grid/Node
	private ObjectManager objectManager;

	// Wave control attributes
	public List<Wave> upcomingWaves = new List<Wave> ();
	public List<Wave> currentWaves = new List<Wave> ();
	public float waveSpawnDelay = 25;
	private float nextWaveSpawnEvent;
	public bool playerTriggeredWave;

	[HideInInspector]
	public GoogleMobileAdsScript ad;

	void Awake()
	{
		objectManager = ObjectManager.GetInstance ();
		ScaleAndPlaceMap ();
		
		MakeWaves ();
		MakeObstacles ();

		destinationNode = objectManager.NodeManager.GetDestinationNode();
		destinationTransform.position = destinationNode.unityPosition;
		destinationTransform.position = new Vector3(destinationTransform.position.x, -.9f, destinationTransform.position.z);

		enemySpawnNodes = objectManager.NodeManager.GetSpawnNodes ();
	}
	
	// Use this for initialization
	void Start () {
		ad = GameObject.FindGameObjectWithTag("Ad").GetComponent<GoogleMobileAdsScript>();
		ad.RequestInterstitial ();

//		foreach (Node node in objectManager.NodeManager.nodes){
//			Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), node.unityPosition, Quaternion.Euler(Vector3.zero));
//		}

		LoadMapTexture ();
		PlaceDoodads ();
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
			for(int x=0; x<objectManager.NodeManager.size_x; x++){
				for(int z=0; z<objectManager.NodeManager.size_y; z++){
					if((x%5==3 || x%5==4) && (z%5==3 || z%5==4)){
						objectManager.NodeManager.nodes[x,z].isBuildable = false;
						objectManager.NodeManager.nodes[x,z].isWalkable = false;

						if(x%5==4 &&
						   z%5==4 && 
						   !(Camera.main.WorldToScreenPoint(objectManager.NodeManager.nodes[x,z].unityPosition).y <= 10 ||
						  Camera.main.WorldToScreenPoint(objectManager.NodeManager.nodes[x,z].unityPosition).y >= Screen.height - Screen.height * .20)) {
							Vector3 spawnPosition = new Vector3(objectManager.NodeManager.nodes[x,z].unityPosition.x - objectManager.MapData.nodeSize.x/2,
							                                    -3,
							                                    objectManager.NodeManager.nodes[x,z].unityPosition.z - objectManager.MapData.nodeSize.y/2);
							Array values = Enum.GetValues(typeof(ObstacleType));
							ObstacleType randomObstacle = (ObstacleType)values.GetValue(random.Next(values.Length));
							GameObject obstaclePrefab = obstaclePrefabs[(int)randomObstacle];
							Instantiate (obstaclePrefab, spawnPosition, obstaclePrefab.transform.rotation);							
						}
						else if(x%5==4 && z%5==4) {

							if(!(Camera.main.WorldToScreenPoint (objectManager.NodeManager.nodes[x-1,z-1].unityPosition).y <= 10 ||
							     Camera.main.WorldToScreenPoint (objectManager.NodeManager.nodes[x-1,z-1].unityPosition).y >= Screen.height - Screen.height * .21)){
								objectManager.NodeManager.nodes[x,z].isBuildable = true;
								objectManager.NodeManager.nodes[x,z].isWalkable = true;

								objectManager.NodeManager.nodes[x-1,z].isBuildable = true;
								objectManager.NodeManager.nodes[x-1,z].isWalkable = true;

								objectManager.NodeManager.nodes[x,z-1].isBuildable = true;
								objectManager.NodeManager.nodes[x,z-1].isWalkable = true;

								objectManager.NodeManager.nodes[x-1,z-1].isBuildable = true;
								objectManager.NodeManager.nodes[x-1,z-1].isWalkable = true;
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

				int spawnIndex = UnityEngine.Random.Range(0, enemySpawnNodes.Length);

				Instantiate (enemy, enemySpawnNodes[spawnIndex].unityPosition, Quaternion.Euler (new Vector3 (90, 45, 0)));
				wave.nextEnemySpawnEvent = Time.time + wave.spawnDelay;
				wave.numberOfEnemies--;
			}
		}

		currentWaves.RemoveAll (a => a.numberOfEnemies <= 0);
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

	public void ScaleAndPlaceMap()
	{
		Node firstNode = objectManager.NodeManager.nodes [0, 0];
		Node lastNode = objectManager.NodeManager.nodes [objectManager.NodeManager.size_x - 1,
		                                                 objectManager.NodeManager.size_y - 1];
		
		Vector3 center = new Vector3 ((lastNode.unityPosition.x + firstNode.unityPosition.x) / 2f,
		                              firstNode.unityPosition.y - 10,
		                              (lastNode.unityPosition.z + firstNode.unityPosition.z) / 2f);

		int xIndex = -1;
		int yIndex = -1;
		for(int x = 0; x<objectManager.MapData.tiles.GetLength(0); x++)
		{
			for(int y = 0; y<objectManager.MapData.tiles.GetLength(1); y++)
			{
				if(xIndex == -1 && objectManager.MapData.tiles[x,y].isNode){
					xIndex = x;
					yIndex = y;
					
					x = objectManager.MapData.tiles.GetLength(0);
					y = objectManager.MapData.tiles.GetLength(1);
				}
			}
		}

		float textureSizeX = objectManager.MapData.tiles.GetLength (0);
		float textureSizeY = objectManager.MapData.tiles.GetLength (1);

		center.z = center.z + (((textureSizeX - objectManager.NodeManager.size_x) / 2f) - xIndex) * objectManager.MapData.nodeSize.y;
		center.x = center.x + (((textureSizeY - objectManager.NodeManager.size_y) / 2f) - yIndex) * (objectManager.MapData.nodeSize.x / (objectManager.MapData.isIsoGrid?2:1));

		transform.position = center;
		transform.localScale = new Vector3 (((objectManager.MapData.tiles.GetLength(0) + 1) * objectManager.MapData.nodeSize.x) / (objectManager.MapData.isIsoGrid?2:1),
		                                    (objectManager.MapData.tiles.GetLength(1)+.5f) * objectManager.MapData.nodeSize.y,
		                                    1);
	}

	public void PlaceDoodads()
	{
		for(int x = 0; x<objectManager.MapData.tiles.GetLength(0); x++)
		{
			for(int y = 0; y<objectManager.MapData.tiles.GetLength(1); y++)
			{
				Tile tile = objectManager.MapData.tiles[x, y];

				if(tile.doodads.Count > 0)
				{
					foreach(GameObject doodad in tile.doodads)
					{
						Instantiate(doodad,
						            objectManager.NodeManager.GetNodeFromTileIndex(x, y).unityPosition,
						            Quaternion.Euler(Vector3.zero));
					}
				}
			}
		}
	}

	public void LoadMapTexture()
	{
		Texture2D[] textures = Resources.LoadAll<Texture2D>(objectManager.MapData.mapName+"/mapTextures");
		StartCoroutine (Animate(textures));
	}

	IEnumerator Animate(Texture2D[] gridTextures) {
		int itr = 0;
		while(true)
		{
			GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[itr%gridTextures.Length];
			itr++;
			yield return new WaitForSeconds(objectManager.MapData.animationSpeed);
		}
	}
}
