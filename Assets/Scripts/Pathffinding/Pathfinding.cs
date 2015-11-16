using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Pathfinding
{
    public bool pathsExist;
    private ObjectManager objectManager;

    public Pathfinding()
    {
        objectManager = ObjectManager.GetInstance();
    }

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
		
		return path;
	}

    /// <summary>
    /// Checks Checks that there is a path and updates it's path and the Enemies on the field.
    /// </summary>
    /// <returns><c>true</c>, if there is a valid <c>false</c> otherwise.</returns>
    public IEnumerator CheckAndUpdatePaths()
	{
        StringBuilder sb = new StringBuilder();

        IDictionary<string, List<Node>> nodePaths = new Dictionary<string, List<Node>>();
		Node destination = objectManager.Map.destinationNode;

		foreach(Node spawn in objectManager.Map.enemySpawnNodes){
            sb.Clear();
            sb.Append(spawn.listIndexX).Append(spawn.listIndexY).Append(destination.listIndexX).Append(destination.listIndexY);

            if (nodePaths.ContainsKey(sb.ToString()))
            {
                continue;
            }

            List<Node> path = Astar(spawn, destination);
            if (path == null)
            {
                pathsExist = false;
                yield break;
            }
            nodePaths.Add(sb.ToString(), path);
		}

        int count = 0;
        int numEnemies = objectManager.enemies.Count;
        for (int index = 0; index < numEnemies; index++) {
            if (index >= objectManager.enemies.Count)
            {
                pathsExist = true;
                yield break;
            }

            EnemyBase agent = objectManager.enemies[index];
            List<Node> path;
            sb.Clear();
            if (agent.movingBackwards > 0)
            {
                sb.Append(agent.onNode.listIndexX).Append(agent.onNode.listIndexY).Append(agent.SpawnNode.listIndexX).Append(agent.SpawnNode.listIndexY);
            }
            else
            {
                sb.Append(agent.onNode.listIndexX).Append(agent.onNode.listIndexY).Append(destination.listIndexX).Append(destination.listIndexY);
            }

            if (nodePaths.ContainsKey(sb.ToString()))
            {
                agent.SetPath(nodePaths[sb.ToString()]);
                continue;
            }

            count++;

            if (agent.movingBackwards > 0)
            {
				path = Astar(agent.onNode, agent.SpawnNode);
            }
			else
            {
                path = Astar(agent.onNode, destination);
            }

            if (path == null)
            {
                pathsExist = false;
                yield break;
            }

			agent.SetPath (path);
            nodePaths.Add(sb.ToString(), path);

            if (count % 10 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
		}

        pathsExist = true;
	}
}