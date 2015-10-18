using UnityEngine;
using System.Collections;

public class NodeManager{
	
	public Node[,] nodes;
	public int size_x = 0;
	public int size_y = 0;
	private Vector3 left;
	private Vector3 right;

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
				xPos = left.x + (x * objectManager.MapData.nodeSize.x * (objectManager.MapData.isIsoGrid?.5f:1f));
				zPos = right.y + ((y + 1) * objectManager.MapData.nodeSize.y) + (objectManager.MapData.isIsoGrid?((x%2==1)?objectManager.MapData.nodeSize.y/2f:0f):0f);
				txPos = (x * objectManager.MapData.nodeSize.x * (objectManager.MapData.isIsoGrid?.5f:1f));
				tyPos = ((y + 1) * objectManager.MapData.nodeSize.y) + (objectManager.MapData.isIsoGrid?((x%2==1)?objectManager.MapData.nodeSize.y/2f:0f):0f);
				Vector3 position = new Vector3 (xPos + objectManager.MapData.nodeSize.x / 2f, 0, zPos - objectManager.MapData.nodeSize.y / 2f);
				Vector3 listIndex = new Vector3 (x, 0, y);
				Vector2 textureCenter = new Vector2(objectManager.MapData.nodeSize.x/2f + txPos,
				                                    tyPos - objectManager.MapData.nodeSize.y / 2f);
				nodes [x, y].listIndex = listIndex;
				nodes [x, y].setUnityPosition(position);
				nodes [x, y].texturePosX = (int)(objectManager.MapData.nodeSize.x/2f + txPos);
				nodes [x, y].texturePosY = (int)(tyPos - objectManager.MapData.nodeSize.y / 2f);
			}
		}
	}

	private void ConnectNodes ()
	{
		if(!objectManager.MapData.isIsoGrid)
		{
			for (int y=0; y<size_y; y++) {
				for (int x=0; x<size_x; x++) {
					//Debug.Log(x+", "+ y);
					
					if (x - 1 >= 0) {
						nodes [x, y].borderTiles [(int)Border.downRight] = nodes [x - 1, y];
						if (y - 1 >= 0)
							nodes [x, y].borderTiles [(int)Border.Down] = nodes [x - 1, y - 1];
						
						if (y + 1 < size_y)
							nodes [x, y].borderTiles [(int)Border.Right] = nodes [x - 1, y + 1];
					}
					
					if (x + 1 < size_x) {
						nodes [x, y].borderTiles [(int)Border.upLeft] = nodes [x + 1, y];
						if (y - 1 >= 0)
							nodes [x, y].borderTiles [(int)Border.Left] = nodes [x + 1, y - 1];
						
						if (y + 1 < size_y)
							nodes [x, y].borderTiles [(int)Border.Up] = nodes [x + 1, y + 1];
					}
					
					if (y - 1 >= 0)
						nodes [x, y].borderTiles [(int)Border.downLeft] = nodes [x, y - 1];
					
					if (y + 1 < size_y)
						nodes [x, y].borderTiles [(int)Border.upRight] = nodes [x, y + 1];
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
							nodes [x, y].borderTiles [(int)Border.Left] = nodes [x - 1, y];
							if (y - 1 >= 0)
								nodes [x, y].borderTiles [(int)Border.Down] = nodes [x - 1, y - 1];
						}

						if (x + 1 < size_x) {
							nodes [x, y].borderTiles [(int)Border.Up] = nodes [x + 1, y];
							if (y - 1 >= 0)
								nodes [x, y].borderTiles [(int)Border.Right] = nodes [x + 1, y - 1];
						}
					}

					if(x%2 == 1){
						if (x - 1 >= 0) {
							nodes [x, y].borderTiles [(int)Border.Down] = nodes [x - 1, y];
							
							if (y + 1 < size_y)
								nodes [x, y].borderTiles [(int)Border.Left] = nodes [x - 1, y + 1];
						}

						if (x + 1 < size_x) {
							nodes [x, y].borderTiles [(int)Border.Right] = nodes [x + 1, y];
							if (y + 1 < size_y)
								nodes [x, y].borderTiles [(int)Border.Up] = nodes [x + 1, y + 1];
						}
					}

					if (x - 2 >= 0)
						nodes [x, y].borderTiles [(int)Border.downLeft] = nodes [x - 2, y];

					if (x + 2 < size_x)
						nodes [x, y].borderTiles [(int)Border.upRight] = nodes [x + 2, y];

					if (y - 1 >= 0)
						nodes [x, y].borderTiles [(int)Border.downRight] = nodes [x, y - 1];
				
					if (y + 1 < size_y)
						nodes [x, y].borderTiles [(int)Border.upLeft] = nodes [x, y + 1];
				}
			}
		}
	}

	private void SetGridSize()
	{
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
		
		for(int x = 0; x<objectManager.MapData.tiles.GetLength(0); x++)
		{
			if(objectManager.MapData.tiles[x,yIndex].isNode){
				size_x++;
			}
		}
		
		for(int y = 0; y<objectManager.MapData.tiles.GetLength(1); y++)
		{
			if(objectManager.MapData.tiles[xIndex,y].isNode){
				size_y++;
			}
		}

		nodes = new Node[size_x, size_y];

		for(int x = 0; x<objectManager.MapData.tiles.GetLength(0); x++)
		{
			for(int y = 0; y<objectManager.MapData.tiles.GetLength(1); y++)
			{
				if(objectManager.MapData.tiles[x,y].isNode){
					nodes [x - xIndex, y - yIndex] = new Node (objectManager.MapData.tiles[x,y].isBuildable, objectManager.MapData.tiles[x,y].isWalkable);
				}
			}
		}
	}

	private void SetGridPosition()
	{
		SetGridSize();

		left = new Vector2(-(size_x * objectManager.MapData.nodeSize.x) / 2f, (size_y * objectManager.MapData.nodeSize.y) / 2f);
		right = new Vector2((size_x * objectManager.MapData.nodeSize.x * (objectManager.MapData.isIsoGrid?.5f:1f)) / 2f, -(size_y * objectManager.MapData.nodeSize.y) / 2f);
	}

	/// <summary>
	/// Gets the tile from location.
	/// </summary>
	public Node GetNodeFromLocation (Vector3 location)
	{
		int xIndex = (int)Mathf.Floor ((location.x - left.x) / (objectManager.MapData.nodeSize.x));
		int zIndex = size_y + ((int)Mathf.Floor ((location.z - left.y) / objectManager.MapData.nodeSize.y));

		if(objectManager.MapData.isIsoGrid)
		{
			xIndex = (int)Mathf.Round((2f*location.x + objectManager.MapData.nodeSize.x - 2f * left.x) / objectManager.MapData.nodeSize.x) - 2;
			zIndex = (int)Mathf.Round((location.z - (objectManager.MapData.nodeSize.y / 2f) - right.y - ((xIndex%2==1)?objectManager.MapData.nodeSize.y/2f:0f)) / objectManager.MapData.nodeSize.y);
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
		
		int xIndex = (int)Mathf.Floor ((location.x - left.x) / objectManager.MapData.nodeSize.x);
		int zIndex = size_y + ((int)Mathf.Floor ((location.z - left.y) / objectManager.MapData.nodeSize.y));

		if(objectManager.MapData.isIsoGrid)
		{
			xIndex = (int)Mathf.Round((2f*location.x + objectManager.MapData.nodeSize.x - 2f * left.x) / objectManager.MapData.nodeSize.x) - 2;
			zIndex = (int)Mathf.Round((location.z - (objectManager.MapData.nodeSize.y / 2f) - right.y - ((xIndex%2==1)?objectManager.MapData.nodeSize.y/2f:0f)) / objectManager.MapData.nodeSize.y);
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
		
		if (node == null || !node.isBuildable)
			return false;
		
		node.isWalkable = false;
		node.isBuildable = false;
		
		return true;
	}
	
	public void UnBlockNode (Vector3 position)
	{
		Node node = GetNodeFromLocation (position);
		
		node.isWalkable = true;
		node.isBuildable = true;
	}
}
