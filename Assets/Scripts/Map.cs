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
	private Vector3 left;
	private Vector3 right;
	public Transform enemySpawnTransform;
	public Transform destinationTransform;
	public Texture healthTexture;
	public Node destinationNode;
	public Node enemySpawnNode;

	// Grid/Node
	public int size_x;
	public int size_z;
	public Vector2 nodeSize;
	public Node[,] nodes;
	public Texture2D grid;
	private Texture2D gridTexture;
	private ObjectManager _ObjectManager;

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
		_ObjectManager = ObjectManager.GetInstance ();
		nodes = new Node[size_x, size_z];

		SetPositions ();
		BuildNodes ();
		ConnectNodes ();
		MakeWaves ();
		MakeObstacles ();

		destinationTransform.position = nodes[size_x-2, 0].unityPosition;
		destinationTransform.position = new Vector3(destinationTransform.position.x, -.9f, destinationTransform.position.z);

		enemySpawnTransform.position = nodes[0, size_z-1].unityPosition;
		enemySpawnTransform.position = new Vector3(enemySpawnTransform.position.x, -.9f, enemySpawnTransform.position.z);
	}
	
	// Use this for initialization
	void Start () {
		ad = GameObject.FindGameObjectWithTag("Ad").GetComponent<GoogleMobileAdsScript>();
		ad.RequestInterstitial ();

		destinationNode = nodes[size_x - 1, 0];
		destinationNode.isBuildable = false;
		enemySpawnNode = nodes[0, size_z - 1];
		enemySpawnNode.isBuildable = false;

		MakeGrid ();

		foreach (Node node in nodes){
			Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), node.unityPosition, Quaternion.Euler(Vector3.zero));
		}
	}

	/// <summary>
	/// Displays the grid.
	/// </summary>
	void OnGUI ()
	{
		foreach (EnemyBase Gob in _ObjectManager.ThingsWithHealthBars()) {
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

        for (int x = 1; x < _ObjectManager.gameState.numberOfWaves; x++)
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
	
		if(_ObjectManager.gameState.MapType == MapType.Obstacles){
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
			_ObjectManager.gameState.waveCount++;
		}
	}
	
	/// <summary>
	/// Creates enemies.
	/// </summary>
	public void CreateEnemies ()
	{
		if (Time.time >= nextWaveSpawnEvent || playerTriggeredWave) {
			_ObjectManager.WaveWheel.UpdateSpriteImages = true;

			playerTriggeredWave = false;
			nextWaveSpawnEvent = Time.time + waveSpawnDelay;
			SpawnWave ();
		}

		foreach (Wave wave in currentWaves) {

			if(_ObjectManager.gameState.GameSpeed == GameSpeed.Paused)
			{
				wave.nextEnemySpawnEvent += Time.deltaTime;
			}
			else 
			{
				wave.nextEnemySpawnEvent -= (Time.deltaTime * (float)_ObjectManager.gameState.GameSpeed) - Time.deltaTime;
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
		if(_ObjectManager.gameState.gameStarted && !_ObjectManager.gameState.gameOver){

			if(_ObjectManager.gameState.GameSpeed == GameSpeed.Paused)
			{
				nextWaveSpawnEvent += Time.deltaTime;
			}
			else 
			{
				nextWaveSpawnEvent -= (Time.deltaTime * (float)_ObjectManager.gameState.GameSpeed) - Time.deltaTime;
			}


			_ObjectManager.gameState.nextWaveCountDown = (int)(nextWaveSpawnEvent - Time.time);
			CreateEnemies ();
		}
	}

	private void SetPositions ()
	{
		left = new Vector2(-(size_x * nodeSize.x) / 2f, (size_z * nodeSize.y) / 2f);
		right = new Vector2((size_x * nodeSize.x * (isIsoGrid?.5f:1f)) / 2f, -(size_z * nodeSize.y) / 2f);
	}
	
	/// <summary>
	/// Initializes nodes that make up the map.
	/// </summary>
	private void BuildNodes ()
	{
		
		float mapSizeX = (right.x - left.x) - (isIsoGrid?(size_x * nodeSize.x * .25f) - nodeSize.x/2: 0);
		float mapSizwZ = (left.y - right.y) + (isIsoGrid?nodeSize.y/2f:0f);

		transform.localScale = new Vector3 (mapSizeX, mapSizwZ, 1);
		
		//nodeSize = new Vector2 (mapSizeX / size_x, mapSizwZ / size_z);
		float xPos;
		float zPos;
		float txPos;
		float tyPos;
		for (int x=0; x<size_x; x++) {
			for (int z=0; z<size_z; z++) {
				xPos = left.x + (x * nodeSize.x * (isIsoGrid?.5f:1f));
				zPos = right.y + ((z + 1) * nodeSize.y) + (isIsoGrid?((x%2==1)?nodeSize.y/2f:0f):0f);
				txPos = (x * nodeSize.x * (isIsoGrid?.5f:1f));
				tyPos = ((z + 1) * nodeSize.y) + (isIsoGrid?((x%2==1)?nodeSize.y/2f:0f):0f);
				Vector3 position = new Vector3 (xPos + nodeSize.x / 2f, 0, zPos - nodeSize.y / 2f);
				Vector3 listIndex = new Vector3 (x, 0, z);
				Vector2 textureCenter = new Vector2(nodeSize.x/2f + txPos,
				                                    tyPos - nodeSize.y / 2f);
				nodes [x, z] = new Node (true, true, position, listIndex, textureCenter);
				
				//if (Camera.main.WorldToScreenPoint (nodes [x, z].unityPosition).y <= 10 ||
				//    Camera.main.WorldToScreenPoint (nodes [x, z].unityPosition).y >= Screen.height - Screen.height * .21) {
				//	nodes [x, z].isWalkable = false;
				//	nodes [x, z].isBuildable = false;
				//}
			}
		}
	}
	
	/// <summary>
	/// Connects the nodes.
	/// </summary>
	private void ConnectNodes ()
	{
		for (int z=0; z<size_z; z++) {
			for (int x=0; x<size_x; x++) {
				//Debug.Log(x+", "+ y);
				
				if (x - 1 >= 0) {
					nodes [x, z].borderTiles [(int)Border.downRight] = nodes [x - 1, z];
					if (z - 1 >= 0)
						nodes [x, z].borderTiles [(int)Border.Down] = nodes [x - 1, z - 1];
					
					if (z + 1 < nodes.GetLength (1))
						nodes [x, z].borderTiles [(int)Border.Right] = nodes [x - 1, z + 1];
				}
				
				if (x + 1 < nodes.GetLength (0)) {
					nodes [x, z].borderTiles [(int)Border.upLeft] = nodes [x + 1, z];
					if (z - 1 >= 0)
						nodes [x, z].borderTiles [(int)Border.Left] = nodes [x + 1, z - 1];
					
					if (z + 1 < nodes.GetLength (1))
						nodes [x, z].borderTiles [(int)Border.Up] = nodes [x + 1, z + 1];
				}
				
				if (z - 1 >= 0)
					nodes [x, z].borderTiles [(int)Border.downLeft] = nodes [x, z - 1];
				
				if (z + 1 < nodes.GetLength (1))
					nodes [x, z].borderTiles [(int)Border.upRight] = nodes [x, z + 1];
			}
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

	private void MakeGrid()
	{
		Vector3 center = new Vector3(right.x + left.x + (isIsoGrid?nodeSize.x/4f:0f), 0, left.y + right.y + (isIsoGrid?nodeSize.y/4f:0f));
		center.y = transform.position.y;
		transform.position = center;
		center.y = Camera.main.transform.position.y;
		Camera.main.transform.position = center;

		gridTexture = new Texture2D (size_x * (int)(nodeSize.x / (isIsoGrid?2f:1f)) + (isIsoGrid?(int)nodeSize.x/2:0),
		                             size_z * (int)nodeSize.y + (isIsoGrid?(int)nodeSize.y/2:0));
		gridTexture.wrapMode = TextureWrapMode.Clamp;
		gridTexture.filterMode = FilterMode.Point;
		GetComponent<Renderer>().material.mainTexture = gridTexture;

		ClearEdges (gridTexture);

        for (var y = size_z - 1; y > -1; y--)
        {
            for (var x = 1; x < size_x; x += 2)
            {
                var node = nodes[x, y];
                if (node.isBuildable)
                {
                    WriteTexture(node, grid, gridTexture);
                }
                else
                {
                    WriteTexture(node, grid, gridTexture);
                }
            }

            for (var x = 0; x < size_x; x += 2)
            {
                var node = nodes[x, y];
                if (node.isBuildable)
                {
                    WriteTexture(node, grid, gridTexture);
                }
                else
                {
                    WriteTexture(node, grid, gridTexture);
                }
            }
        }

        
		gridTexture.Apply ();

	}

    public void WriteTexture(Node node, Texture2D tex, Texture2D masterTexture)
	{
		Color[] colors = tex.GetPixels ();

		int xOffset = node.texturePosX - (int)nodeSize.x / 2;
		int yOffset = node.texturePosY - (int)nodeSize.y / 2;

	    for (int x = 0; x < nodeSize.x; x++)
	    {
            for (int y = 0; y < nodeSize.y; y++)
        {
		
				int index = y * (int)nodeSize.x + x;
				if( colors[index].a == 0){
					continue;
				}
				masterTexture.SetPixel(x+xOffset, y+yOffset, colors[index]);
			}
		}

	}

	public void ClearEdges(Texture2D masterTexture)
	{
//		for(int x = 0; x < nodeSize.x * size_x; x++){
//			for(int y = 0; y < nodeSize.y / 2; y++){
//				masterTexture.SetPixel(x, y, Color.clear);
//			}
//		}
//		for(int x = 0; x < nodeSize.x * size_x; x++){
//			for(int y = (int)nodeSize.y * size_z - (int)nodeSize.y / 2; y < (int)nodeSize.y * size_z; y++){
//				masterTexture.SetPixel(x, y, Color.clear);
//			}
//		}
//		for(int x = 0; x < nodeSize.x / 2; x++){
//			for(int y = 0; y < nodeSize.y * size_z; y++){
//				masterTexture.SetPixel(x, y, Color.clear);
//			}
//		}
//		for(int x = (int)nodeSize.x * size_x - (int)nodeSize.x; x < (int)nodeSize.x * size_x; x++){
//			for(int y = 0; y < nodeSize.y * size_z; y++){
//				masterTexture.SetPixel(x, y, Color.clear);
//			}
//		}

		for(int x = 0; x < (int)nodeSize.x * (size_x+1); x++){
			for(int y = 0; y < nodeSize.y * (size_z+1); y++){
				masterTexture.SetPixel(x, y, Color.clear);
			}
		}

	}

	public void SetGrid(bool flag)
	{
		GetComponent<Renderer> ().material.color = new Color (1f, 1f, 1f, 1f * (flag?1f:0f));
	}
}
