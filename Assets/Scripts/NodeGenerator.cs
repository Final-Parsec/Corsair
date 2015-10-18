using UnityEngine;
using System.Collections;

public class NodeGenerator{
	
	public Node[,] nodes;
	public int size_x = 0;
	public int size_y = 0;
	private Vector3 left;
	private Vector3 right;
	private IMapData mapData;

	public NodeGenerator(IMapData mapData)
	{
		this.mapData = mapData;
	}

	public void MakeNodes()
	{
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
				xPos = left.x + (x * mapData.nodeSize.x * (mapData.isIsoGrid?.5f:1f));
				zPos = right.y + ((y + 1) * mapData.nodeSize.y) + (mapData.isIsoGrid?((x%2==1)?mapData.nodeSize.y/2f:0f):0f);
				txPos = (x * mapData.nodeSize.x * (mapData.isIsoGrid?.5f:1f));
				tyPos = ((y + 1) * mapData.nodeSize.y) + (mapData.isIsoGrid?((x%2==1)?mapData.nodeSize.y/2f:0f):0f);
				Vector3 position = new Vector3 (xPos + mapData.nodeSize.x / 2f, 0, zPos - mapData.nodeSize.y / 2f);
				Vector3 listIndex = new Vector3 (x, 0, y);
				Vector2 textureCenter = new Vector2(mapData.nodeSize.x/2f + txPos,
				                                    tyPos - mapData.nodeSize.y / 2f);
				nodes [x, y].setUnityPosition(position);
				nodes [x, y].texturePosX = (int)(mapData.nodeSize.x/2f + txPos);
				nodes [x, y].texturePosY = (int)(tyPos - mapData.nodeSize.y / 2f);
			}
		}
	}

	private void ConnectNodes ()
	{
		if(mapData.isIsoGrid)
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

					if (y - 2 >= 0)
						nodes [x, y].borderTiles [(int)Border.Down] = nodes [x, y - 2];

					if (y + 2 < size_y)
						nodes [x, y].borderTiles [(int)Border.Up] = nodes [x, y + 2];
					
					if (x - 1 >= 0) {
						nodes [x, y].borderTiles [(int)Border.downRight] = nodes [x - 1, y];
						if (y + 1 < size_y)
							nodes [x, y].borderTiles [(int)Border.Right] = nodes [x - 1, y + 1];
					}
					
					if (x + 1 < size_x) {
						nodes [x, y].borderTiles [(int)Border.upLeft] = nodes [x + 1, y];
						if (y - 1 >= 0)
							nodes [x, y].borderTiles [(int)Border.Left] = nodes [x + 1, y - 1];
					}
					
					if (y - 1 >= 0)
						nodes [x, y].borderTiles [(int)Border.downLeft] = nodes [x, y - 1];
					
					if (y + 1 < size_y)
						nodes [x, y].borderTiles [(int)Border.upRight] = nodes [x, y + 1];
				}
			}
		}
	}

	private void SetGridSize()
	{
		int xIndex = -1;
		int yIndex = -1;
		for(int x = 0; x<mapData.tiles.GetLength(0); x++)
		{
			for(int y = 0; y<mapData.tiles.GetLength(1); y++)
			{
				if(xIndex == -1 && mapData.tiles[x,y].isNode){
					xIndex = x;
					yIndex = y;
				}
			}
		}
		
		for(int x = xIndex; x<mapData.tiles.GetLength(0); x++)
		{
			if(mapData.tiles[x,yIndex].isNode){
				size_y++;
			}
		}
		
		for(int y = yIndex; y<mapData.tiles.GetLength(1); y++)
		{
			if(mapData.tiles[xIndex,y].isNode){
				size_y++;
			}
		}

		nodes = new Node[size_x, size_y];

		for(int x = 0; x<mapData.tiles.GetLength(0); x++)
		{
			for(int y = 0; y<mapData.tiles.GetLength(1); y++)
			{
				if(mapData.tiles[x,y].isNode){
					nodes [x - xIndex, y - yIndex] = new Node (mapData.tiles[x,y].isBuildable, mapData.tiles[x,y].isWalkable);
				}
			}
		}
	}

	private void SetGridPosition()
	{
		SetGridSize();

		left = new Vector2(-(size_x * mapData.nodeSize.x) / 2f, (size_y * mapData.nodeSize.y) / 2f);
		right = new Vector2((size_x * mapData.nodeSize.x * (mapData.isIsoGrid?.5f:1f)) / 2f, -(size_y * mapData.nodeSize.y) / 2f);
	}
}
