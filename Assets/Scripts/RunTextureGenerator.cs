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
		MakeTiles();

		Texture2D[] gridTextures = TextureGenerator.Generate(tiles, nodeSize, isIsoGrid);
		
//		GetComponent<Renderer>().sharedMaterial.SetTexture(1, gridTextures[1]);
//		GetComponent<Renderer>().sharedMaterial.SetTexture(0, gridTextures[0]);

		GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[1];

		SaveTextures(gridTextures);
	}

//	IEnumerator Animate(Texture2D[] gridTextures) {
//		int max = 30;
//		for (int x = 0; x < max; x++)
//		{
//			GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[x%gridTextures.Length];
//			yield return new WaitForSeconds(.05f);
//			Debug.Log("here");
//		}
//
//	}

	private void MakeTiles()
	{
		tiles = new Tile[(int)nodeSize.x, (int)nodeSize.y];

		for(int x = 0; x<nodeSize.x; x++)
		{
			for(int y = 0; y<nodeSize.y; y++)
			{
				Texture2D[] testures = (y==1)?new Texture2D[]{grid, grid2, grid}:new Texture2D[]{grid};
				Tile tile = new Tile(testures, true);
				tiles[x,y] = tile;
			}
		}
	}

	private void SaveTextures(Texture2D[] textures)
	{
		int count = 0;
		foreach(Texture2D tex in textures){

			byte[] bytes = tex.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/Texture"+ count++ +".png", bytes);
		}
	}
}