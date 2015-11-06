using UnityEngine;
using System.Collections;

public class SpawnOfEnemy : EnemyBase
{
	// Runs when entity is Instantiated
	void OnEnable()
	{
		_ObjectManager = ObjectManager.GetInstance ();
		_ObjectManager.AddEntity (this);
		onNode = _ObjectManager.NodeManager.GetClosestNode (transform.position);
		InitAttributes();
	}
}
