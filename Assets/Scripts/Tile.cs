using UnityEngine;
using System.Collections.Generic;

public class Tile {

	public LinkedList<GameObject> doodads;
	public Texture2D[] tileTextures;
	public bool isBuildable;
	public bool isWalkable;
	public bool isNode;

	public int texturePositionX;
	public int texturePositionY;

	public Tile(Texture2D[] tileTextures, bool isBuildable, bool isWalkable, bool isNode){
		this.tileTextures = tileTextures;
		this.isBuildable = isBuildable;
		this.isWalkable = isWalkable;
		this.isNode = isNode;
		doodads = new LinkedList<GameObject>();
	}
}
