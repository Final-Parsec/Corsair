using UnityEngine;
using System.Collections;

public abstract class IMapData {
	public Tile[,] tiles;
	public string mapName;
	public Vector2 nodeSize;
	public Vector2 tileSize;
	public bool isIsoGrid;
	public float animationSpeed;

	public abstract Vector2 GetDestinationTileIndex();

	public abstract Vector2[] GetEnemySpawnTileIndecies();
}
