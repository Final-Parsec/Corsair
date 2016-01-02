using System.Collections;
using UnityEngine;

public class SpawnerEnemy : EnemyBase {
	public GameObject babies;
	public int numBabies;
	
	public override IEnumerator DestroyThis ()
	{
		if(!(onNode == this.objectManager.WaveManager.destinationNode)){
			for(int x=0; x<numBabies; x++)
			{
				var go = babies.GetObjectFromPool(babies.gameObject.name , GetClosePosition(), Quaternion.Euler (new Vector3 (90, 0, 0)));
                go.name = babies.name;
            }
		}

		yield return StartCoroutine(base.DestroyThis());
	}
	
	private Vector3 GetClosePosition(){
		Vector3 returnVector = transform.position;
		
		returnVector.x += Random.Range(-1.5f, 1.5f); 
		returnVector.z += Random.Range(-1.5f, 1.5f); 
		
		return returnVector;
	}
	
}
