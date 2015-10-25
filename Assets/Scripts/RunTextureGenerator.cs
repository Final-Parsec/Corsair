using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(Renderer))]
public class RunTextureGenerator : MonoBehaviour
{
	public string mapName;
	public Vector2 tileSize;
	public bool isIsoGrid;
	private Tile[,] tiles;

	public Texture2D[] grid;

	public void GetTexture()
	{
		IMapData mapData = new HardcodedMapData(mapName, tileSize, isIsoGrid, grid);
		TextureGenerator textureGenerator = new TextureGenerator(); 
		Texture2D[] gridTextures = textureGenerator.Generate(mapData);
		
//		GetComponent<Renderer>().sharedMaterial.SetTexture(1, gridTextures[1]);
//		GetComponent<Renderer>().sharedMaterial.SetTexture(0, gridTextures[0]);

		GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[0];
	}
}