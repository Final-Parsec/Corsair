using UnityEngine;
using System.Collections;

public abstract class IMapData {
	public Tile[,] tiles;
	public string mapName;
	public Vector2 nodeSize;
	public bool isIsoGrid;

	public abstract Vector2 GetDestinationTileIndex();

	public abstract Vector2[] GetEnemySpawnTileIndecies();
}
