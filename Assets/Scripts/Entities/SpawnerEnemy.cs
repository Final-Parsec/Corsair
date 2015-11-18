using UnityEngine;

public class SpawnerEnemy : EnemyBase {
	public GameObject babies;
	public int numBabies;
	
	public override void DestroyThis ()
	{
		if(!(onNode == this.objectManager.WaveManager.destinationNode)){
			for(int x=0; x<numBabies; x++)
			{
				babies.GetObjectFromPool(babies.gameObject.name , GetClosePosition(), Quaternion.Euler (new Vector3 (90, 0, 0)));
			}
		}
		base.DestroyThis();
	}
	
	private Vector3 GetClosePosition(){
		Vector3 returnVector = transform.position;
		
		returnVector.x += Random.Range(-1.5f, 1.5f); 
		returnVector.z += Random.Range(-1.5f, 1.5f); 
		
		return returnVector;
	}
	
}
