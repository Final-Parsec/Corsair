using UnityEngine;
using System.Collections.Generic;

public class ReverseDirection : DebuffBase
{
    /// <summary>
    /// Tells if the effect has been applied.
    /// </summary>
    private bool appliedEffect;
    
    /// <summary>
    /// Initializes an instance of <see cref="DebuffBase"/>.
    /// </summary>
    /// <param name="owner">The agent under the effect.</param>
    /// <param name="duration">The length of time the buff is active.</param>
	public ReverseDirection(EnemyBase owner, float duration)
	{
        this.objectManager = ObjectManager.GetInstance();
        this.owner = owner;
		this.duration = duration;
	}

    /// <summary>
    /// Applies the Buff.
    /// </summary>
    /// <param name="deltaTime">The time between the last application.</param>
    /// <returns>A boolean value. True if the Buff if finished else false.</returns>
	public override bool Apply (float deltaTime)
	{
	    this.elapsedTime += deltaTime;
        if (this.elapsedTime > this.duration) {
		    this.EndEffect ();
			return true;
		}

	    if (this.appliedEffect)
	    {
	        return false;
        }
        
        this.owner.StopMindControlling = false;
        this.owner.mindControlled++;
	    if (this.owner.mindControlled == 1)
	    {
	        this.MoveOwner(this.owner.spawnNode);
	    }
	    this.appliedEffect = true;

        return false;
	}

    /// <summary>
    /// Ends a Buff's effect.
    /// </summary>
	public override void EndEffect ()
	{
		this.owner.mindControlled--;
		if(this.owner.mindControlled <= 0)
        {
            this.MoveOwner(this.objectManager.Map.destinationNode);
            this.owner.StopMindControlling = true;
        }
	}

    /// <summary>
    /// Set the path of the agent to a new destination.
    /// </summary>
    /// <param name="end">The <see cref="Node"/> the agent is moving towards.</param>
	private void MoveOwner(Node end) {
		List<Node> path = this.objectManager.Pathfinding.Astar (this.owner.onNode, end);
		
		if (path != null)
		    this.owner.SetPath (path);
		else
			Debug.Log("No path availible");
	}
}

