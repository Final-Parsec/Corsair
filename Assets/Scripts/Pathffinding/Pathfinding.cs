using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
	private ObjectManager objectManager;

	public List<Node> Astar (Node start, Node goal)
	{
		if (start == null || goal == null){
			return null;
		}
		
		if (start == goal)
		{
			return new List<Node> {start};
		}

		foreach(Node node in objectManager.NodeManager.nodes)
		{
			node.Reset();
		}
		
		MinHeap openSet = new MinHeap(start);
		start.IsInOpenSet = true;
		
		start.gScore = 0;
		start.fScore = start.gScore + Heuristic_cost_estimate (goal, start);

		Node current = null;
		while (openSet.Count() > 0) {
			
			current = openSet.GetRoot ();

			current.IsInOpenSet = false;
			current.IsInClosedSet = true;
			
			
			if (current == goal)
			{
				return Reconstruct_path (start, goal);
			}
			
			foreach (Node neighbor in current.BorderTiles) {
				if(neighbor == null || !neighbor.IsWalkable || neighbor.IsInClosedSet)
					continue;
				
				// if the new gscore is lower replace it
				int tentativeGscore = current.gScore + Heuristic_cost_estimate (current, neighbor);
				
				if (!neighbor.IsInOpenSet || tentativeGscore < neighbor.gScore) {
					
					neighbor.parent = current;
					neighbor.gScore = tentativeGscore;
					neighbor.fScore = neighbor.gScore + Heuristic_cost_estimate (goal, neighbor);
					
					if (!neighbor.IsInOpenSet){
						openSet.Add (neighbor);
						neighbor.IsInOpenSet = true;
					}
					else
					{
						openSet.Reevaluate(neighbor);
					}
				}
			}
		}
		// Fail
		return null;
	}

	
	public int Heuristic_cost_estimate (Node goal, Node current)
	{
		
		int dx1 = (int)Math.Abs((current.listIndexX) - (goal.listIndexX));
		int dy1 = (int)Math.Abs((current.listIndexY) - (goal.listIndexY));
		
		if (dx1 > dy1)
			return 14*dy1 + 10*(dx1-dy1);
		else
			return 14*dx1 + 10*(dy1-dx1);
	}
	
	
	/// <summary>
	/// Reconstruct_path the specified start and goal.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="goal">Goal.</param>
	public static List<Node> Reconstruct_path (Node start, Node goal)
	{
		List<Node> path = new List<Node> ();
		path.Add (goal);
		
		Node itr = goal;
		while (itr != null && itr.parent != start) {
			path.Add (itr.parent);
			itr = itr.parent;
		}
		path.Add (start);
		
		return path;
	}

	/// <summary>
	/// Checks Checks that there is a path and updates it's path and the Enemies on the field.
	/// </summary>
	/// <returns><c>true</c>, if there is a valid <c>false</c> otherwise.</returns>
	public bool CheckAndUpdatePaths ()
	{
		Node destination = objectManager.Map.destinationNode;

		foreach(Node spawn in objectManager.Map.enemySpawnNodes){
			if (Astar (spawn, destination) == null)
				return false;

		}
		
		foreach (EnemyBase entity in objectManager.enemies) {
			List<Node> path;

            
			if(entity.mindControlled > 0)
            {
				path = Astar(entity.onNode, entity.spawnNode);
            }
			else
            {
                path = Astar(entity.onNode, destination);
            }

			if (path == null)
				return false;
			entity.SetPath (path);
		}
		return true;
	}

	// Use this for initialization
	void Start ()
	{
		objectManager = ObjectManager.GetInstance ();
	}
}