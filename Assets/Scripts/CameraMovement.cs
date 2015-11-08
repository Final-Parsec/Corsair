using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour{

	static int NO_DRAG = int.MinValue;
	
	int sensitivity = 200;
	int scrollSensitivity = 75;
	Vector2 cameraMinDistance;
	Vector2 cameraMaxDistance;
	int scrollMaxDistance = 150;
	int scrollMinDistance = 50;

	private static bool isZooming;
	private static int dragTouchId;
	private float dragZone = 2;

	private ObjectManager objectManager;

	public static bool IsCameraMoving()
	{
		return dragTouchId != NO_DRAG || Input.touchCount > 1 || isZooming;
	}
	
	void Start() {
		objectManager = ObjectManager.GetInstance();

		Node firstNode = objectManager.NodeManager.nodes [0, 0];
		Node lastNode = objectManager.NodeManager.nodes [objectManager.NodeManager.size_x - 1,
		                                                 objectManager.NodeManager.size_y - 1];

		cameraMinDistance = new Vector2(firstNode.UnityPosition.x,
		                                firstNode.UnityPosition.z);
		cameraMaxDistance = new Vector2(lastNode.UnityPosition.x,
		                                lastNode.UnityPosition.z);

		Vector3 center = new Vector3 ((lastNode.UnityPosition.x + firstNode.UnityPosition.x) / 2f,
		                              transform.position.y,
		                              (lastNode.UnityPosition.z + firstNode.UnityPosition.z) / 2f);

		transform.position = center;
	}
	
	// Update is called once per frame
	void Update () {
		float theScreenWidth = Screen.width;
		float theScreenHeight = Screen.height;

		float moveRate = sensitivity * Time.deltaTime;
		float scrollRate = scrollSensitivity * Time.deltaTime;
		
		float deltaX = 0;
		float deltaY = 0;
		float deltaZ = 0;
		
		// Move the camera with the arrow keys or with the mouse.
		if ( Input.GetKey(KeyCode.W)){
			if (transform.position.z + moveRate < cameraMaxDistance.y){
				deltaY = moveRate;
			}
		}
		if ( Input.GetKey(KeyCode.S)){
			if (transform.position.z - moveRate > cameraMinDistance.y){
				deltaY = -moveRate;
			}
		}
		if ( Input.GetKey(KeyCode.D)){
			if (transform.position.x + moveRate < cameraMaxDistance.x){
				deltaX = moveRate;
			}
		}
		if ( Input.GetKey(KeyCode.A)){
			if (transform.position.x - moveRate > cameraMinDistance.x){
				deltaX = -moveRate;
			}
		}

		if(Input.touchCount == 1){

			Touch touch = Input.touches[0];
			if(dragTouchId !=  NO_DRAG|| Math.Abs(touch.deltaPosition.x) > dragZone || Math.Abs(touch.deltaPosition.y) > dragZone){

				deltaX = -touch.deltaPosition.x/2f;
				deltaY = -touch.deltaPosition.y/2f;

				dragTouchId = Input.touchCount == 1?touch.fingerId:-1;
			}
		}

		if(Input.touchCount != 1)
		{
			dragTouchId = NO_DRAG;
		}

		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			isZooming = true;

			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			
			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			Camera.main.orthographicSize += deltaMagnitudeDiff * scrollRate;
			
			// Make sure the orthographic size never drops below zero.
			Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, scrollMinDistance);
			Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, scrollMaxDistance);

		}

		if(isZooming && Input.touchCount == 0)
		{
			isZooming = false;
		}
		
		// Zoom in/out with the scroll wheel.
		if (Input.GetAxis("Mouse ScrollWheel") > 0){
			if (Camera.main.orthographicSize - scrollRate > scrollMinDistance)
				Camera.main.orthographicSize = Camera.main.orthographicSize - scrollRate;
			else
				Camera.main.orthographicSize = scrollMinDistance;
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0){
			if (Camera.main.orthographicSize + scrollRate < scrollMaxDistance)
				Camera.main.orthographicSize = Camera.main.orthographicSize + scrollRate;
			else
				Camera.main.orthographicSize = scrollMaxDistance;
			
		}
		
		moveCamera(deltaX, deltaY, deltaZ);
	}
	
	private void moveCamera(float x, float y, float z){
		// Different coordinate standards.
		transform.position = new Vector3((transform.position.x + x > cameraMaxDistance.x)?cameraMaxDistance.x:Mathf.Max(cameraMinDistance.x,transform.position.x + x),
		                                 transform.position.y + z,
		                                 (transform.position.z + y > cameraMaxDistance.y)?cameraMaxDistance.y:Mathf.Max(cameraMinDistance.y,transform.position.z + y));
	}
	
	public void setStartingPosition(Vector3 position){
		transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, transform.position.y, position.z), .1f);
	}
	
	public float GetDistanceRatio(){
		return Camera.main.orthographicSize / scrollMaxDistance;
	}
	
}

