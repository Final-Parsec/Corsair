﻿using UnityEngine;
using System.Collections;

public class Hard : MonoBehaviour {
	MainMenu _Menu;
	// Use this for initialization
	void Start () {
		_Menu = GameObject.Find("Main Camera").GetComponent<MainMenu>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/// <summary>
	/// Raises the mouse Down event.
	/// </summary>
	void OnMouseDown(){
		renderer.material.color = Color.red;
	}
	
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	void OnMouseUp(){
		renderer.material.color = Color.white;
		_Menu.gameState.PlayerHealth = 50;
		_Menu.gameState.playerMoney = 100;
        _Menu.gameState.dificultyFactor = .3f;
		_Menu.GoToGame();
	}
	
}
