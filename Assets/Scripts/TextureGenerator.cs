using UnityEngine;
using System.Collections;

public static class TextureGenerator{

	// Grid/Node
	private static int sizeX;
	private static int sizeY;
	private static int nodeSizeX;
	private static int nodeSizeY;
	private static Tile[,] tiles;
	private static int maxTextures;
	
	private static bool isIsoGrid;

	public static Texture2D[] Generate(Tile[,] tiles, Vector2 nodeSize)
	{
		TextureGenerator.tiles = tiles;

		sizeX = tiles.GetLength(0);
		sizeY = tiles.GetLength(1);
		
		nodeSizeX = (int)nodeSize.x;
		nodeSizeY = (int)nodeSize.y;

		TextureGenerator.isIsoGrid = true;

		CalculateTilePositions();
		
		return MakeTextures();
	}

	public static Texture2D[] Generate(Tile[,] tiles, Vector2 nodeSize, bool isIsoGrid)
	{
		TextureGenerator.tiles = tiles;
		
		sizeX = tiles.GetLength(0);
		sizeY = tiles.GetLength(1);
		
		nodeSizeX = (int)nodeSize.x;
		nodeSizeY = (int)nodeSize.y;
		
		TextureGenerator.isIsoGrid = isIsoGrid;

		CalculateTilePositions();

		return MakeTextures();
	}

	private static void CalculateTilePositions ()
	{
		float txPos;
		float tyPos;
		for (int x=0; x<sizeX; x++) {
			for (int y=0; y<sizeY; y++) {
				txPos = (x * nodeSizeX * (isIsoGrid?.5f:1f));
				tyPos = ((y + 1) * nodeSizeY) + (isIsoGrid?((x%2==1)?nodeSizeY/2f:0f):0f);

				tiles [x, y].texturePositionX = (int)(nodeSizeX/2 + txPos);
				tiles [x, y].texturePositionY = (int)(tyPos - nodeSizeY / 2);

				maxTextures = Mathf.Max(maxTextures, tiles [x, y].tileTextures.Length);
			}
		}
	}
	
	private static Texture2D[] MakeTextures()
	{
		Texture2D[] textureArray = new Texture2D[maxTextures];

		for(int textureNumber = 0; textureNumber < maxTextures; textureNumber++)
		{

			textureArray[textureNumber] = new Texture2D (sizeX * (nodeSizeX / (isIsoGrid?2:1)) + (isIsoGrid?nodeSizeX/2:0),
			                             sizeY * nodeSizeY + (isIsoGrid?nodeSizeY/2:0));

			textureArray[textureNumber].wrapMode = TextureWrapMode.Clamp;
			textureArray[textureNumber].filterMode = FilterMode.Point;
			
			ClearEdges (textureArray[textureNumber]);
			
			for (var y = sizeY - 1; y > -1; y--)
			{
				for (var x = 1; x < sizeX; x += 2)
				{
					var tile = tiles[x, y];
					WriteTileTexture(tile, tile.tileTextures[(textureNumber >= tile.tileTextures.Length)?tile.tileTextures.Length - 1 : textureNumber], textureArray[textureNumber]);
				}
				
				for (var x = 0; x < sizeX; x += 2)
				{
					var tile = tiles[x, y];
					WriteTileTexture(tile, tile.tileTextures[(textureNumber >= tile.tileTextures.Length)?tile.tileTextures.Length - 1 : textureNumber], textureArray[textureNumber]);
				}
			}
			textureArray[textureNumber].Apply ();
		}
		return textureArray;
	}
	
	public static void WriteTileTexture(Tile tile, Texture2D tex, Texture2D masterTexture)
	{
		Color[] colors = tex.GetPixels ();
		
		int xOffset = tile.texturePositionX - nodeSizeX / 2;
		int yOffset = tile.texturePositionY - nodeSizeY / 2;
		
		for (int x = 0; x < nodeSizeX; x++)
		{
			for (int y = 0; y < nodeSizeY; y++)
			{
				int index = y * nodeSizeX + x;
				if( colors[index].a == 0){
					continue;
				}
				masterTexture.SetPixel(x+xOffset, y+yOffset, colors[index]);
			}
		}
	}
	
	public static void ClearEdges(Texture2D masterTexture)
	{
		//		for(int x = 0; x < nodeSize.x * size_x; x++){
		//			for(int y = 0; y < nodeSize.y / 2; y++){
		//				masterTexture.SetPixel(x, y, Color.clear);
		//			}
		//		}
		//		for(int x = 0; x < nodeSize.x * size_x; x++){
		//			for(int y = (int)nodeSize.y * size_z - (int)nodeSize.y / 2; y < (int)nodeSize.y * size_z; y++){
		//				masterTexture.SetPixel(x, y, Color.clear);
		//			}
		//		}
		//		for(int x = 0; x < nodeSize.x / 2; x++){
		//			for(int y = 0; y < nodeSize.y * size_z; y++){
		//				masterTexture.SetPixel(x, y, Color.clear);
		//			}
		//		}
		//		for(int x = (int)nodeSize.x * size_x - (int)nodeSize.x; x < (int)nodeSize.x * size_x; x++){
		//			for(int y = 0; y < nodeSize.y * size_z; y++){
		//				masterTexture.SetPixel(x, y, Color.clear);
		//			}
		//		}
		
		for(int x = 0; x < nodeSizeX * (sizeX+1); x++){
			for(int y = 0; y < nodeSizeY * (sizeY+1); y++){
				masterTexture.SetPixel(x, y, Color.clear);
			}
		}
		
	}


}
