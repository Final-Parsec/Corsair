using UnityEngine;
using System.Collections;

public class Tile {

	public Texture2D[] tileTextures;
	public bool isBuildable;

	public int texturePositionX;
	public int texturePositionY;

	public Tile(Texture2D[] tileTextures, bool isBuildable){
		this.tileTextures = tileTextures;
		this.isBuildable = isBuildable;
	}
}
