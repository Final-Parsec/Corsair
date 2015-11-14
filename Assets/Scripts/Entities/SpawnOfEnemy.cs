using UnityEngine;
using System.Collections;

public class SpawnOfEnemy : EnemyBase
{
	// Runs when entity is Instantiated
	void OnEnable()
	{
		this.objectManager = ObjectManager.GetInstance ();
		this.objectManager.AddEntity (this);
		onNode = this.objectManager.NodeManager.GetClosestNode (transform.position);
		InitAttributes();
	}
}
