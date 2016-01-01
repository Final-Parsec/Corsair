using UnityEngine;
using System.Collections;

public class DeathIntAction : MonoBehaviour {
	private static float timeToLive = .8f;
	private static int speed = 5;

	private float madeAt;
	private ObjectManager objectManager;

	// Use this for initialization
	void OnEnable () {
        gameObject.GetComponent<MeshRenderer>().sortingOrder = 32767;
        objectManager = ObjectManager.GetInstance();
		madeAt = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		Move();
		if(Time.time > madeAt+timeToLive)
		{
			this.gameObject.ReturnToPool(this.gameObject.name);
		}
	}

	public void Move ()
	{
		// don't move in the Y direction.
		Vector3 moveVector = new Vector3 (-1, 0, -1).normalized;
		
		// update the position
		transform.position = new Vector3 (transform.position.x - moveVector.x * (speed * (int)objectManager.gameState.GameSpeed) * Time.deltaTime,
		                                  transform.position.y,
		                                  transform.position.z - moveVector.z * (speed * (int)objectManager.gameState.GameSpeed) * Time.deltaTime);
	}
}
