using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(Renderer))]
public class RunTextureGenerator : MonoBehaviour
{
	public Vector2 nodeSize;
	public bool isIsoGrid;
	private Tile[,] tiles;

	public Texture2D grid;
	public Texture2D grid2;

	public void GetTexture()
	{
		IMapData mapData = new HardcodedMapData("Test", nodeSize, isIsoGrid, grid, grid2);
		TextureGenerator textureGenerator = new TextureGenerator(); 
		Texture2D[] gridTextures = textureGenerator.Generate(mapData);
		
//		GetComponent<Renderer>().sharedMaterial.SetTexture(1, gridTextures[1]);
//		GetComponent<Renderer>().sharedMaterial.SetTexture(0, gridTextures[0]);

		GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[1];
	}
}