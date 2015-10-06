using UnityEngine;
using System.Collections;

public class ScreenAnimationEvents : MonoBehaviour {
	
	
	public void FadeOutFinished()
	{
		gameObject.SetActive (false);
	}

	public void SwipeOutFinished()
	{
		ObjectManager.GetInstance().TurretFocusMenu.SelectedTurret = null;
	}
}