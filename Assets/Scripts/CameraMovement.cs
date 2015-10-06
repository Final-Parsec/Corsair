using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour{
	
	int sensitivity = 120;
	int scrollSensitivity = 50;
	Vector2 cameraMinDistance;
	Vector2 cameraMaxDistance;
	int scrollMaxDistance = 100;
	int scrollMinDistance = 30;

	private static bool up;
	private static bool Up
	{
		get{return up;}
		set
		{
			if(value)
			{
				Down = false;
				Left = false;
				Right = false;
			}
			up = value;
		}
	}
	private static bool down;
	private static bool Down
	{
		get{return down;}
		set
		{
			if(value)
			{
				Up = false;
				Left = false;
				Right = false;
			}
			down = value;
		}
	}
	private static bool left;
	private static bool Left
	{
		get{return left;}
		set
		{
			if(value)
			{
				Up = false;
				Down = false;
				Right = false;
			}
			left= value;
		}
	}
	private static bool right;
	private static bool Right
	{
		get{return right;}
		set
		{
			if(value)
			{
				Up = false;
				Down = false;
				Left = false;
			}
			right = value;
		}
	}

	private static bool wasZooming;

	public static bool IsCameraMoving()
	{
		return up||down||right||left||Input.touchCount > 1||wasZooming;
	}
	
	void Start() {
		
		Map _tileMap = GameObject.Find("Map").GetComponent<Map>();
		
		cameraMinDistance = new Vector2(transform.position.x - (_tileMap.size_x * _tileMap.nodeSize.x)/2f, transform.position.z - (_tileMap.size_z * _tileMap.nodeSize.y)/2f);
		cameraMaxDistance = new Vector2((_tileMap.size_x * _tileMap.nodeSize.x)/2f + transform.position.x, (_tileMap.size_z * _tileMap.nodeSize.y)/2f + transform.position.z);
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
		if ( Input.GetKey(KeyCode.UpArrow) || Input.touchCount == 1 && Input.touches[0].deltaPosition.y > 1 || Up){
			if (transform.position.z + moveRate < cameraMaxDistance.y){
				deltaY = moveRate;
				Up = true;
			}
		}
		if ( Input.GetKey(KeyCode.DownArrow) || Input.touchCount == 1 && Input.touches[0].deltaPosition.y < -1 || Down){
			if (transform.position.z - moveRate > cameraMinDistance.y){
				deltaY = -moveRate;
				Down = true;
			}
		}
		if ( Input.GetKey(KeyCode.RightArrow) || Input.touchCount == 1 && Input.touches[0].deltaPosition.x > 1 || Right){
			if (transform.position.x + moveRate < cameraMaxDistance.x){
				deltaX = moveRate;
				Right = true;
			}
		}
		if ( Input.GetKey(KeyCode.LeftArrow) || Input.touchCount == 1 && Input.touches[0].deltaPosition.x < -1 || Left){
			if (transform.position.x - moveRate > cameraMinDistance.x){
				deltaX = -moveRate;
				Left = true;
			}
		}

		if(Input.touchCount != 1)
		{
			Up = false;
			Down = false;
			Right = false;
			Left = false;
		}

		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			wasZooming = true;

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

		if(wasZooming && Input.touchCount == 0)
		{
			wasZooming = false;
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
		transform.position = new Vector3(transform.position.x + x, transform.position.y + z, transform.position.z + y);
	}
	
	public void setStartingPosition(Vector3 position){
		transform.position = new Vector3(position.x, transform.position.y, position.z);
	}
	
	public float GetDistanceRatio(){
		return Camera.main.orthographicSize / scrollMaxDistance;
	}
	
}

