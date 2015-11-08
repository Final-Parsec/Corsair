using UnityEngine;
using System;
using System.Collections.Generic;

public class Node : IComparable
{
	public Turret turret = null;
	public EnemyBase enemie;

	public bool IsWalkable { get; set; }
	public bool IsBuildable { get; set; }

	// Use border enum to access tiles.
	public Node[] BorderTiles = new Node[8];
	public Vector3 UnityPosition { get; set; }
	public int listIndexX;
	public int listIndexY;
	public int texturePosX;
	public int texturePosY;

	// A* variables
	public int gScore = int.MaxValue; // undefined 
	public int fScore = int.MaxValue; // undefined 
	public bool IsInOpenSet { get; set; }
	public bool IsInClosedSet { get; set; }
	public Node parent = null;

	public int CompareTo(System.Object obj) {
		if (obj != null) 
		{
			Node node = obj as Node;
			
			if(this.fScore.CompareTo(node.fScore) == 0)
				return (this.fScore - this.gScore).CompareTo(node.fScore - node.gScore);
			return this.fScore.CompareTo(node.fScore);
		}
		else 
		{
			throw new ArgumentException("Object is not a Node");
		}
	}

	public Node ()
	{
	}

	public Node (bool isWalkable, bool isBuildable, Vector3 unityPosition, Vector3 listIndex, Vector2 textureCenter)
	{
		this.IsWalkable = isWalkable;
		this.IsBuildable = isBuildable;
		this.UnityPosition = unityPosition;
		this.listIndexX = (int)listIndex.x;
		this.listIndexY = (int)listIndex.z;
		this.texturePosX = (int)textureCenter.x;
		this.texturePosY = (int)textureCenter.y;
	}

	public Node (bool isWalkable, bool isBuildable)
	{
		this.IsWalkable = isWalkable;
		this.IsBuildable = isBuildable;
	}
	
	public Node clone ()
	{
		return new Node (IsWalkable, IsBuildable, UnityPosition, new Vector2(listIndexX, listIndexY), new Vector2(texturePosX, texturePosY));
	}

	public void setUnityPosition (Vector3 unityPosition)
	{
		this.UnityPosition = unityPosition;
	}

	public void MakeWalkable ()
	{
		IsWalkable = true;
	}

	public void MakeBuildable ()
	{
		IsBuildable = true;
	}
	
	public Node[] getDiagnalNeighbors ()
	{
		return new Node[4] {BorderTiles [(int)Border.downLeft],
								BorderTiles [(int)Border.downRight],
								BorderTiles [(int)Border.upLeft],
								BorderTiles [(int)Border.upRight]};
	}

	public List<Node> getDiagnalWalkableNeighbors ()
	{
		List<Node> returnList = new List<Node>();
		
		if(BorderTiles [(int)Border.Left]!=null && BorderTiles [(int)Border.Left].IsWalkable){
			if(BorderTiles [(int)Border.Up]!=null &&BorderTiles [(int)Border.Up].IsWalkable)
				returnList.Add(BorderTiles [(int)Border.upLeft]);
			if(BorderTiles [(int)Border.Down]!=null &&BorderTiles [(int)Border.Down].IsWalkable)
				returnList.Add(BorderTiles [(int)Border.downLeft]);
		}

		if(BorderTiles [(int)Border.Right]!=null &&BorderTiles [(int)Border.Right].IsWalkable){
			if(BorderTiles [(int)Border.Up]!=null &&BorderTiles [(int)Border.Up].IsWalkable)
				returnList.Add(BorderTiles [(int)Border.upRight]);
			if(BorderTiles [(int)Border.Down]!=null &&BorderTiles [(int)Border.Down].IsWalkable)
				returnList.Add(BorderTiles [(int)Border.downRight]);
		}
		
		return returnList;
	}

	public Node[] getCloseNeighbors ()
	{
		return new Node[4] {BorderTiles [(int)Border.Left],
								BorderTiles [(int)Border.Right],
								BorderTiles [(int)Border.Down],
								BorderTiles [(int)Border.Up]};
	}

	public int GetDirection(Node node)
	{
        if (node == this)
        {
            return (int)Border.Right;
        }

		for(int index = 0; index < BorderTiles.Length; index++){
			if(node == BorderTiles[index])
				return index;
		}
		return (int)Border.Center;
	}

	public void Reset()
	{
		gScore = int.MaxValue; 
		fScore = int.MaxValue;
		IsInOpenSet = false;
		IsInClosedSet = false;
		parent = null;
	}
}

