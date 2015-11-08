using UnityEngine;
using System.Collections;

public class NodeManager{
	
	public Node[,] nodes;
	public int size_x = 0;
	public int size_y = 0;
	private Vector3 left;
	private Vector3 right;
	private int xIndexOffset;
	private int yIndexOffset;

	private ObjectManager objectManager;

	public NodeManager()
	{
		MakeNodes ();
	}

	private void MakeNodes()
	{
		objectManager = ObjectManager.GetInstance ();
		SetGridPosition();
		BuildNodes();
		ConnectNodes();
	}

	private void BuildNodes ()
	{
		float xPos;
		float zPos;
		float txPos;
		float tyPos;
		for (int x=0; x<size_x; x++) {
			for (int y=0; y<size_y; y++) {
				xPos = left.x + (x * objectManager.MapData.NodeSize.x * (objectManager.MapData.IsIsoGrid?.5f:1f));
				zPos = right.y + ((y + 1) * objectManager.MapData.NodeSize.y) + (objectManager.MapData.IsIsoGrid?((x%2==1)?objectManager.MapData.NodeSize.y/2f:0f):0f);
				txPos = (x * objectManager.MapData.NodeSize.x * (objectManager.MapData.IsIsoGrid?.5f:1f));
				tyPos = ((y + 1) * objectManager.MapData.NodeSize.y) + (objectManager.MapData.IsIsoGrid?((x%2==1)?objectManager.MapData.NodeSize.y/2f:0f):0f);
				Vector3 position = new Vector3 (xPos + objectManager.MapData.NodeSize.x / 2f, 0, zPos - objectManager.MapData.NodeSize.y / 2f);
				Vector2 textureCenter = new Vector2(objectManager.MapData.NodeSize.x/2f + txPos,
				                                    tyPos - objectManager.MapData.NodeSize.y / 2f);
				nodes [x, y].listIndexX = x;
				nodes [x, y].listIndexY = y;
				nodes [x, y].UnityPosition = position;
				nodes [x, y].texturePosX = (int)(objectManager.MapData.NodeSize.x/2f + txPos);
				nodes [x, y].texturePosY = (int)(tyPos - objectManager.MapData.NodeSize.y / 2f);
			}
		}
	}

	private void ConnectNodes ()
	{
		if(!objectManager.MapData.IsIsoGrid)
		{
			for (int y=0; y<size_y; y++) {
				for (int x=0; x<size_x; x++) {
					//Debug.Log(x+", "+ y);
					
					if (x - 1 >= 0) {
						nodes [x, y].BorderTiles [(int)Border.downRight] = nodes [x - 1, y];
						if (y - 1 >= 0)
							nodes [x, y].BorderTiles [(int)Border.Down] = nodes [x - 1, y - 1];
						
						if (y + 1 < size_y)
							nodes [x, y].BorderTiles [(int)Border.Right] = nodes [x - 1, y + 1];
					}
					
					if (x + 1 < size_x) {
						nodes [x, y].BorderTiles [(int)Border.upLeft] = nodes [x + 1, y];
						if (y - 1 >= 0)
							nodes [x, y].BorderTiles [(int)Border.Left] = nodes [x + 1, y - 1];
						
						if (y + 1 < size_y)
							nodes [x, y].BorderTiles [(int)Border.Up] = nodes [x + 1, y + 1];
					}
					
					if (y - 1 >= 0)
						nodes [x, y].BorderTiles [(int)Border.downLeft] = nodes [x, y - 1];
					
					if (y + 1 < size_y)
						nodes [x, y].BorderTiles [(int)Border.upRight] = nodes [x, y + 1];
				}
			}
		}
		else
		{
			for (int y=0; y<size_y; y++) {
				for (int x=0; x<size_x; x++) {
					//Debug.Log(x+", "+ y);
					if(x%2 == 0){
						if (x - 1 >= 0) {
							nodes [x, y].BorderTiles [(int)Border.Left] = nodes [x - 1, y];
							if (y - 1 >= 0)
								nodes [x, y].BorderTiles [(int)Border.Down] = nodes [x - 1, y - 1];
						}

						if (x + 1 < size_x) {
							nodes [x, y].BorderTiles [(int)Border.Up] = nodes [x + 1, y];
							if (y - 1 >= 0)
								nodes [x, y].BorderTiles [(int)Border.Right] = nodes [x + 1, y - 1];
						}
					}

					if(x%2 == 1){
						if (x - 1 >= 0) {
							nodes [x, y].BorderTiles [(int)Border.Down] = nodes [x - 1, y];
							
							if (y + 1 < size_y)
								nodes [x, y].BorderTiles [(int)Border.Left] = nodes [x - 1, y + 1];
						}

						if (x + 1 < size_x) {
							nodes [x, y].BorderTiles [(int)Border.Right] = nodes [x + 1, y];
							if (y + 1 < size_y)
								nodes [x, y].BorderTiles [(int)Border.Up] = nodes [x + 1, y + 1];
						}
					}

					if (x - 2 >= 0)
						nodes [x, y].BorderTiles [(int)Border.downLeft] = nodes [x - 2, y];

					if (x + 2 < size_x)
						nodes [x, y].BorderTiles [(int)Border.upRight] = nodes [x + 2, y];

					if (y - 1 >= 0)
						nodes [x, y].BorderTiles [(int)Border.downRight] = nodes [x, y - 1];
				
					if (y + 1 < size_y)
						nodes [x, y].BorderTiles [(int)Border.upLeft] = nodes [x, y + 1];
				}
			}
		}
	}

	private void SetGridSize()
	{
		for(int x = 0; x<objectManager.MapData.Tiles.GetLength(0); x++)
		{
			for(int y = 0; y<objectManager.MapData.Tiles.GetLength(1); y++)
			{
				if(objectManager.MapData.Tiles[x,y].isNode){
					xIndexOffset = x;
					yIndexOffset = y;

					x = objectManager.MapData.Tiles.GetLength(0);
					y = objectManager.MapData.Tiles.GetLength(1);
				}
			}
		}
		
		for(int x = 0; x<objectManager.MapData.Tiles.GetLength(0); x++)
		{
			if(objectManager.MapData.Tiles[x,yIndexOffset].isNode){
				size_x++;
			}
		}
		
		for(int y = 0; y<objectManager.MapData.Tiles.GetLength(1); y++)
		{
			if(objectManager.MapData.Tiles[xIndexOffset,y].isNode){
				size_y++;
			}
		}

		nodes = new Node[size_x, size_y];

		for(int x = 0; x<objectManager.MapData.Tiles.GetLength(0); x++)
		{
			for(int y = 0; y<objectManager.MapData.Tiles.GetLength(1); y++)
			{
				if(objectManager.MapData.Tiles[x,y].isNode){
					nodes [x - xIndexOffset, y - yIndexOffset] = new Node (objectManager.MapData.Tiles[x,y].isBuildable, objectManager.MapData.Tiles[x,y].isWalkable);
				}
			}
		}
	}

	private void SetGridPosition()
	{
		SetGridSize();

		left = new Vector2(-(size_x * objectManager.MapData.NodeSize.x) / 2f, (size_y * objectManager.MapData.NodeSize.y) / 2f);
		right = new Vector2((size_x * objectManager.MapData.NodeSize.x * (objectManager.MapData.IsIsoGrid?.5f:1f)) / 2f, -(size_y * objectManager.MapData.NodeSize.y) / 2f);
	}

	/// <summary>
	/// Gets the tile from location.
	/// </summary>
	public Node GetNodeFromLocation (Vector3 location)
	{
		int xIndex = (int)Mathf.Floor ((location.x - left.x) / (objectManager.MapData.NodeSize.x));
		int zIndex = size_y + ((int)Mathf.Floor ((location.z - left.y) / objectManager.MapData.NodeSize.y));

		if(objectManager.MapData.IsIsoGrid)
		{
			xIndex = (int)Mathf.Round((2f*location.x + objectManager.MapData.NodeSize.x - 2f * left.x) / objectManager.MapData.NodeSize.x) - 2;
			zIndex = (int)Mathf.Round((location.z - (objectManager.MapData.NodeSize.y / 2f) - right.y - ((xIndex%2==1)?objectManager.MapData.NodeSize.y/2f:0f)) / objectManager.MapData.NodeSize.y);
		}

		// out of bounds check
		if (zIndex >= size_y || zIndex < 0 || xIndex >= size_x || xIndex < 0)
			return null;
		
		return nodes [xIndex, zIndex];
	}
	
	/// <summary>
	/// Gets the tile from location.
	/// </summary>
	public Node GetClosestNode (Vector3 location)
	{
		
		int xIndex = (int)Mathf.Floor ((location.x - left.x) / objectManager.MapData.NodeSize.x);
		int zIndex = size_y + ((int)Mathf.Floor ((location.z - left.y) / objectManager.MapData.NodeSize.y));

		if(objectManager.MapData.IsIsoGrid)
		{
			xIndex = (int)Mathf.Round((2f*location.x + objectManager.MapData.NodeSize.x - 2f * left.x) / objectManager.MapData.NodeSize.x) - 2;
			zIndex = (int)Mathf.Round((location.z - (objectManager.MapData.NodeSize.y / 2f) - right.y - ((xIndex%2==1)?objectManager.MapData.NodeSize.y/2f:0f)) / objectManager.MapData.NodeSize.y);
		}
		
		if (xIndex >= size_x)
			xIndex = size_x - 1;
		else if (xIndex < 0)
			xIndex = 0;
		
		if (zIndex >= size_y)
			zIndex = size_y - 1;
		else if (zIndex < 0)
			zIndex = 0;
		
		
		return nodes [xIndex, zIndex];
	}

	public bool BlockNode (Vector3 position)
	{
		Node node = GetNodeFromLocation (position);
		
		if (node == null || !node.IsBuildable)
			return false;
		
		node.IsWalkable = false;
		node.IsBuildable = false;
		
		return true;
	}
	
	public void UnBlockNode (Vector3 position)
	{
		Node node = GetNodeFromLocation (position);
		
		node.IsWalkable = true;
		node.IsBuildable = true;
	}

	public Node GetDestinationNode()
	{
		Vector2 destIndex = objectManager.MapData.DestinationNode;
		return nodes[(int)destIndex.x - xIndexOffset, (int)destIndex.y - yIndexOffset];
	}

	public Node[] GetSpawnNodes()
	{
		Vector2[] spawnInecies = objectManager.MapData.EnemySpawnTileIndicies;
		Node[] spawnNodes = new Node[spawnInecies.Length];

		for(int x = 0; x< spawnInecies.Length; x++){
			spawnNodes[x] = nodes[(int)spawnInecies[x].x - xIndexOffset, (int)spawnInecies[x].y - yIndexOffset];
		}

		return spawnNodes;
	}

	public Node GetNodeFromTileIndex(int x, int y)
	{
		return nodes[x + xIndexOffset, y + yIndexOffset];
	}
}
