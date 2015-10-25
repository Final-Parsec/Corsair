using UnityEngine;
using System.Collections;
using System.IO;

public class TextureGenerator{

	// Grid/Node
	private int sizeX;
	private int sizeY;
	private int tileSizeX;
	private int tileSizeY;
	private int maxTextures;
	private IMapData mapData;

	public Texture2D[] Generate(IMapData mapData)
	{
		this.mapData = mapData;
		
		sizeX = mapData.tiles.GetLength(0);
		sizeY = mapData.tiles.GetLength(1);
		
		tileSizeX = (int)mapData.tileSize.x;
		tileSizeY = (int)mapData.tileSize.y;

		CalculateTilePositions();

		return MakeTextures();
	}

	private void CalculateTilePositions ()
	{
		float txPos;
		float tyPos;
		for (int x=0; x<sizeX; x++) {
			for (int y=0; y<sizeY; y++) {
				txPos = (x * tileSizeX * (mapData.isIsoGrid?.5f:1f));
				tyPos = ((y + 1) * tileSizeY) + (mapData.isIsoGrid?((x%2==1)?tileSizeY/2f:0f):0f);

				mapData.tiles [x, y].texturePositionX = (int)(tileSizeX/2 + txPos);
				mapData.tiles [x, y].texturePositionY = (int)(tyPos - tileSizeY / 2);

				maxTextures = Mathf.Max(maxTextures, mapData.tiles [x, y].tileTextures.Length);
			}
		}
	}
	
	private Texture2D[] MakeTextures()
	{
		Texture2D[] textureArray = new Texture2D[maxTextures];

		for(int textureNumber = 0; textureNumber < maxTextures; textureNumber++)
		{

			textureArray[textureNumber] = new Texture2D (sizeX * (tileSizeX / (mapData.isIsoGrid?2:1)) + (mapData.isIsoGrid?tileSizeX/2:0),
			                                             sizeY * tileSizeY + (mapData.isIsoGrid?tileSizeY/2:0));

			textureArray[textureNumber].wrapMode = TextureWrapMode.Clamp;
			textureArray[textureNumber].filterMode = FilterMode.Point;
			
			ClearEdges (textureArray[textureNumber]);
			
			for (var y = sizeY - 1; y > -1; y--)
			{
				for (var x = 1; x < sizeX; x += 2)
				{
					var tile = mapData.tiles[x, y];
					WriteTileTexture(tile, tile.tileTextures[(textureNumber >= tile.tileTextures.Length)?tile.tileTextures.Length - 1 : textureNumber], textureArray[textureNumber]);
				}
				
				for (var x = 0; x < sizeX; x += 2)
				{
					var tile = mapData.tiles[x, y];
					WriteTileTexture(tile, tile.tileTextures[(textureNumber >= tile.tileTextures.Length)?tile.tileTextures.Length - 1 : textureNumber], textureArray[textureNumber]);
				}
			}
			textureArray[textureNumber].Apply ();
		}

		SaveTextures(textureArray);
		return textureArray;
	}
	
	public void WriteTileTexture(Tile tile, Texture2D tex, Texture2D masterTexture)
	{
		Color[] colors = tex.GetPixels ();
		
		int xOffset = tile.texturePositionX - tileSizeX / 2;
		int yOffset = tile.texturePositionY - tileSizeY / 2;
		
		for (int x = 0; x < tileSizeX; x++)
		{
			for (int y = 0; y < tileSizeY; y++)
			{
				int index = y * tileSizeX + x;
				if( colors[index].a == 0){
					continue;
				}
				masterTexture.SetPixel(x+xOffset, y+yOffset, colors[index]);
			}
		}
	}
	
	public void ClearEdges(Texture2D masterTexture)
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
		
		for(int x = 0; x < tileSizeX * (sizeX+1); x++){
			for(int y = 0; y < tileSizeY * (sizeY+1); y++){
				masterTexture.SetPixel(x, y, Color.clear);
			}
		}
		
	}

	private void SaveTextures(Texture2D[] textures)
	{
		int count = 0;
		foreach(Texture2D tex in textures){
			
			byte[] bytes = tex.EncodeToPNG();
			string path = Application.dataPath + "/Resources/"+mapData.mapName+"/mapTextures/";
			if(File.Exists(path)){
				File.WriteAllBytes(path+mapData.mapName+"_"+ count++ +".png", bytes);
			}else{
				Directory.CreateDirectory(path);
				File.WriteAllBytes(path+mapData.mapName+"_"+ count++ +".png", bytes);
			}
		}
	}
}
