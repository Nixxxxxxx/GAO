using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[ExecuteInEditMode]
public class DebugInfo : MonoBehaviour {

	public PlayerController playerControllerInfo;

	public Text Center_text;
	public Text Up_text;
	public Text Down_text;

	string directionMagnitude = "";
	string joystickReading = "";
	string speedTxt = "Speed ";
	float _speed = 0;
	// Use this for initialization
	void Start () {
		playerControllerInfo = this.transform.parent.GetComponent<PlayerController> ();
		_speed = playerControllerInfo.speed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.skin.label.fontSize = 25;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.color = Color.black;
		//GUI.Label (new Rect (100, 100, 250, 250), Input.acceleration.z.ToString ());
		float _debugSpeed = Input.GetAxis (MainEventManager.StateManager.GameState.myJoyStick.set_speed) ;
		if (_debugSpeed == 0) {
			_debugSpeed = 0.1f;
		}

		playerControllerInfo.speed = _debugSpeed * _speed;

		directionMagnitude  = "dirMagnitude " + System.Environment.NewLine + playerControllerInfo.direction.x.ToString () + "- dirX   " +	playerControllerInfo.direction.y.ToString () + "- dirY   " +playerControllerInfo.direction.z.ToString () + "- dirZ   " ;
		joystickReading 	= "Joystick Axis reading " + System.Environment.NewLine +
								"Speed set to : " +_speed.ToString() + "   joystickReading  " + _debugSpeed ;
		speedTxt 			=  "Speed Actual : " + playerControllerInfo.speed;


		Up_text.text = speedTxt;
		Center_text.text = directionMagnitude;
		Down_text.text = joystickReading;
	}

}







