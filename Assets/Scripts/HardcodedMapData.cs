using UnityEngine;
using System.Collections;

public class HardcodedMapData : IMapData {
	private Texture2D grid;
	private Texture2D grid2;

	public HardcodedMapData(string mapName, Vector2 tileSize, bool isIsoGrid, Texture2D grid, Texture2D grid2)
	{
		this.mapName = mapName;
		this.tileSize = tileSize;
		this.isIsoGrid = isIsoGrid;
		this.grid = grid;
		this.grid2 = grid2;
		this.nodeSize = new Vector2 (32, 16);
		MakeTiles();
	}

	public override Vector2 GetDestinationTileIndex()
	{
		return new Vector2(50, 29);
	}

	public override Vector2[] GetEnemySpawnTileIndecies()
	{
		Vector2[] arr = {new Vector2(5, 5), new Vector2(5, 15), new Vector2(5, 25)};
		return arr;
	}

	private void MakeTiles()
	{
		int lengthX = 68;
		int lengthY = 36;

		this.tiles = new Tile[lengthX, lengthY];
		
		for(int x = 0; x<lengthX; x++)
		{
			for(int y = 0; y<lengthY; y++)
			{
				Texture2D[] testures = (y==1)?new Texture2D[]{grid, grid2, grid}:new Texture2D[]{grid};
				Tile tile = new Tile(testures, false, false, false);
				tiles[x,y] = tile;

				if(x > 1 && x < lengthX-6 && y > 1 && y < lengthY-6){
					tile.isBuildable = true;
					tile.isWalkable = true;
					tile.isNode = true;
				}
			}
		}
	}
}
