using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public LinkedList<GameObject> doodads;
    public bool isBuildable;
    public bool isNode;
    public bool isWalkable;

    public int texturePositionX;
    public int texturePositionY;
    public Texture2D[] tileTextures;

    public Tile(Texture2D[] tileTextures, bool isBuildable, bool isWalkable, bool isNode)
    {
        this.tileTextures = tileTextures;
        this.isBuildable = isBuildable;
        this.isWalkable = isWalkable;
        this.isNode = isNode;
        doodads = new LinkedList<GameObject>();
    }
}