using UnityEngine;
using System.Collections;

public class HardcodedMapData : IMapData {
	private Texture2D grid;
	private Texture2D grid2;

	public HardcodedMapData(string mapName, Vector2 nodeSize, bool isIsoGrid, Texture2D grid, Texture2D grid2)
	{
		this.mapName = mapName;
		this.nodeSize = nodeSize;
		this.isIsoGrid = isIsoGrid;
		this.grid = grid;
		this.grid2 = grid2;
		MakeTiles();
	}

	private void MakeTiles()
	{
		this.tiles = new Tile[(int)nodeSize.x, (int)nodeSize.y];
		
		for(int x = 0; x<nodeSize.x; x++)
		{
			for(int y = 0; y<nodeSize.y; y++)
			{
				Texture2D[] testures = (y==1)?new Texture2D[]{grid, grid2, grid}:new Texture2D[]{grid};
				Tile tile = new Tile(testures, false, false, false);
				tiles[x,y] = tile;

				if(x > 2 && x < nodeSize.x-2 && y > 2 && y < nodeSize.y-2){
					tile.isBuildable = true;
					tile.isWalkable = true;
				}
			}
		}
	}
}
