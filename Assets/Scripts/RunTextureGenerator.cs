using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RunTextureGenerator : MonoBehaviour
{
	// interface
	public Vector2 nodeSize;
	public bool isIsoGrid;
	private Tile[,] tiles;

	public Texture2D grid;

	// interface
	public void GetTexture()
	{
		MakeTiles();

		Texture2D gridTexture = TextureGenerator.Generate(tiles, nodeSize, isIsoGrid);
		GetComponent<Renderer>().sharedMaterial.mainTexture = gridTexture;
	}

	private void MakeTiles()
	{
		tiles = new Tile[(int)nodeSize.x, (int)nodeSize.y];

		for(int x = 0; x<nodeSize.x; x++)
		{
			for(int y = 0; y<nodeSize.y; y++)
			{
				Tile tile = new Tile(grid, true);
				tiles[x,y] = tile;
			}
		}
	}
}