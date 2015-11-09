using UnityEngine;
using System.Collections;

public class Press_Start : MonoBehaviour {
	/// <summary>
	/// Raises the mouse Down event.
	/// </summary>
	void OnMouseDown(){
		GetComponent<Renderer>().material.color = Color.red;
	}
	
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	void OnMouseUp(){
		GetComponent<Renderer>().material.color = Color.white;
	}
}
