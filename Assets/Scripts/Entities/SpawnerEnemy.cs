using UnityEngine;
using System.Collections;

public class SpawnerEnemy : EnemyBase {
	public GameObject babies;
	public int numBabies;
	
	public override void DestroyThisEntity ()
	{
		if(!(onNode == this.objectManager.Map.destinationNode)){
			for(int x=0; x<numBabies; x++)
			{
				babies.GetObjectFromPool(babies.gameObject.name , GetClosePosition(), Quaternion.Euler (new Vector3 (90, 0, 0)));
			}
		}
		base.DestroyThisEntity();
	}
	
	private Vector3 GetClosePosition(){
		Vector3 returnVector = transform.position;
		
		returnVector.x += Random.Range(-1.5f, 1.5f); 
		returnVector.z += Random.Range(-1.5f, 1.5f); 
		
		return returnVector;
	}
	
}
