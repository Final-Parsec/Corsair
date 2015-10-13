using UnityEngine;
using System.Collections;

public class Tile {

	public Texture2D tileTexture;
	public bool isBuildable;

	public int texturePositionX;
	public int texturePositionY;

	public Tile(Texture2D tileTexture, bool isBuildable){
		this.tileTexture = tileTexture;
		this.isBuildable = isBuildable;
	}
}
