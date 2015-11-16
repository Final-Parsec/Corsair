using UnityEngine;
using System.Collections.Generic;

public abstract class Agent : MonoBehaviour
{
    /// <summary>
    /// Executes event when the <see cref="Agent"/> exits a node.
    /// </summary>
    public delegate void NodeExitAction();
    public event NodeExitAction NodeExit;

    /// <summary>
    /// Manages and maps class instances.
    /// </summary>
    public ObjectManager objectManager;

    /// <summary>
    /// The <see cref="Node"/> the <see cref="Agent"/> was spawned on.
    /// </summary>
    public Node SpawnNode;

    /// <summary>
    /// The Node the agent is over.
    /// </summary>
    public Node onNode;

    /// <summary>
    /// The current path the <see cref="Agent"/> is walking.
    /// </summary>
    protected List<Node> path = null;

    /// <summary>
    /// An index into <paramref name="path"/> List.
    /// The index of the <see cref="Node"/> the <see cref="Agent"/> is traveling to.
    /// </summary>
    protected int currentPathIndex = 0;

    /// <summary>
    /// The <see cref="Agent"/> will move to the next <see cref="Node"/> in it's <paramref name="path"/>
    /// when it is within <paramref name="minWaypointDisplacement"/> units from the <see cref="Node"/>s center.
    /// </summary>
    protected float minWaypointDisplacement = 5;

    /// <summary>
    /// The speed that the <see cref="Agent"/> is traveling.
    /// </summary>
    protected float speed = 10;

    /// <summary>
    /// 0 is walking forward, >0 are stacked back commands.
    /// Makes <see cref="Agent"/> more back to the spawn Node.
    /// </summary>
    public int movingBackwards = 0;

    /// <summary>
    /// Triggers the <see cref="Agent"/> to start moving forwards again.
    /// </summary>
    public bool StopMovingBackwards { get; set; }

    /// <summary>
    /// Gets or sets speed.
    /// </summary>
    public float Speed
    {
        get { return this.speed; }
        set
        {
            if (value < 0)
            {
                this.speed = 0;
                return;
            }
            this.speed = value;
        }
    }

    /// <summary>
    /// Sets the path.
    /// </summary>
    /// <param name="path">The new path.</param>
	public void SetPath(List<Node> path)
    {
        if (path == null || path.Count == 0)
            return;

        this.path = path;
        this.currentPathIndex = path.Count - 1;
    }


    // called in update
    // move the unit closer to the next tile in it's path.
    /// <summary>
    /// moves the unit closer to the next tile in it's path.
    /// </summary>
    public void Move()
    {
        if (this.path == null || this.currentPathIndex >= this.path.Count || this.currentPathIndex < 0)
            return;

        Node pathNode = this.path[this.currentPathIndex];

        // don't move in the Y direction.
        Vector3 moveVector = new Vector3(this.transform.position.x - pathNode.UnityPosition.x,
                                         0,
                                         this.transform.position.z - pathNode.UnityPosition.z).normalized;

        // update the position
        float gameSpeedAdjustedSpeed = this.speed * (float)this.objectManager.gameState.GameSpeed * Time.deltaTime;
        this.transform.position = new Vector3(this.transform.position.x - moveVector.x * gameSpeedAdjustedSpeed,
                                              this.transform.position.y,
                                              this.transform.position.z - moveVector.z * gameSpeedAdjustedSpeed);

        // unit has reached the waypoint
        Vector3 position = this.transform.position;
        position.y = pathNode.UnityPosition.y;
        if (Vector3.Distance(position, pathNode.UnityPosition) <= this.minWaypointDisplacement)
        {
            AgentChangedNodes();

            if (this.NodeExit != null)
            {
                this.NodeExit();
            }
            this.currentPathIndex--;

            // unit has reached the destination
            if (this.currentPathIndex < 0)
            {
                AgentReachedDestination();
                return;
            }
        }
    }

    /// <summary>
    /// Is called when the <see cref="Agnet"/> changes <see cref="Node"/>s.
    /// </summary>
    protected abstract void AgentChangedNodes();

    /// <summary>
    /// Is called when the <see cref="Agnet"/> reaches the destination.
    /// </summary>
    protected abstract void AgentReachedDestination();
}
