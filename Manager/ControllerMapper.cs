//#define DEBUG_CONTROLLER 
#define CONTROLLER_SETTING
using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;





[ExecuteInEditMode]
public class ControllerMapper : MonoBehaviour {
		

	
	string[] mPadName;
	string last_bt_pressed = "Press a button";
	List<string> removeWord;
	string key_pressed = "Press button";
	bool b_checkConfig = false;
	bool b_info = false;
	public static readonly string SceneLogin = "Login_Scene";
	float x_con =  0.0f;
	float y_con = 0.0f;

	public Texture2D buttonTexture = null;
	public Texture2D bt_texture = null;
	public Texture2D default_icon = null;
	public Texture2D default2_icon = null;
	public Texture2D up_down_icon = null;
	public Texture2D left_right_icon = null;
	public Texture2D info_icon = null;
	public Texture2D back_icon = null;

	GUIContent infoContent = new GUIContent ();
	GUIContent returnContent = new GUIContent ();
	GUIContent backContent = new GUIContent ();
	public string btId = "";
	LoadSaveManager LSM = null;
	Color defaultColor ;
	// Use this for initialization





	void Start () {
		removeWord = new List<string>();
		mPadName = Input.GetJoystickNames();
	//	GameManager.Notifications.AddListener("SaveGamePrepare",SaveGamePrepare);++++++++++++++++++++++++++++++++
		x_con = Screen.width / 16;
		y_con = Screen.height / 9;
		LSM = MainEventManager.StateManager;
		defaultColor = GUI.backgroundColor;

		infoContent.image = info_icon;
		infoContent.text = "CONFIG INFO";
		returnContent.image = back_icon;
		returnContent.text = "RETURN TO LOGIN";
		backContent.image = back_icon;
		backContent.text = "RETURN";
	}
	
	// Update is called once per frame
	void Update () {		
		GetKeyCode();		
	}
	
	
	
	void OnGUI() {
		if (Input.anyKeyDown) {
			key_pressed = Input.inputString;		
			Debug.Log (Input.inputString);
		}



		GUI.skin.button.fontSize = Screen.width / 40;
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.fontSize = Screen.width / 40;
#if DEBUG_CONTROLLER
		
		var textArea = new Rect(10,10,Screen.width, Screen.height-100);
		string txtLine1 = " Controller name: " ;
		if(mPadName.Length == 1){
			txtLine1 +=  mPadName[0]+"\n";
			txtLine1+= DebugPad();
		}		
		GUI.Label(textArea,txtLine1);
		
#endif
			
#if CONTROLLER_SETTING

		if(!b_checkConfig){
			GUI.backgroundColor = Color.clear;
			//Move Buttons
			GUI.DrawTexture(new Rect(0,0,Screen.height/8,Screen.height/8),up_down_icon,ScaleMode.StretchToFill,true);
			GUI.DrawTexture(new Rect(Screen.height/8,(Screen.height/8)*0,Screen.width/3-Screen.height/8,Screen.height/8),default_icon,ScaleMode.StretchToFill,true);
			if (GUI.Button(new Rect(Screen.height/8,(Screen.height/8)*0,Screen.width/3-Screen.height/8,Screen.height/8), MainEventManager.StateManager.GameState.myJoyStick.m_forward)){
				MainEventManager.StateManager.GameState.myJoyStick.m_forward = last_bt_pressed;
				MainEventManager.StateManager.GameState.myJoyStick.m_backward = last_bt_pressed;
				MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up = last_bt_pressed;
				MainEventManager.StateManager.GameState.myJoyStick.t_axis2_down = last_bt_pressed;
			}
			GUI.DrawTexture(new Rect(0,(Screen.height/8)*2,Screen.height/8,Screen.height/8),left_right_icon,ScaleMode.StretchToFill,true);
			GUI.DrawTexture(new Rect(Screen.height/8,(Screen.height/8)*2,Screen.width/3-Screen.height/8,Screen.height/8),default_icon,ScaleMode.StretchToFill,true);
			if (GUI.Button(new Rect(Screen.height/8,(Screen.height/8)*2,Screen.width/3-Screen.height/8,Screen.height/8), MainEventManager.StateManager.GameState.myJoyStick.m_left)){
				MainEventManager.StateManager.GameState.myJoyStick.m_left = last_bt_pressed;
				MainEventManager.StateManager.GameState.myJoyStick.m_right = last_bt_pressed;
				MainEventManager.StateManager.GameState.myJoyStick.t_axis1_right = last_bt_pressed;
				MainEventManager.StateManager.GameState.myJoyStick.t_axis1_left = last_bt_pressed;

			}

			GUI.Label(new Rect((Screen.width/3)*1,(Screen.height/8),Screen.width/3,(Screen.height/8) * 3), "JOY CONFIG \n" +
				"Press buttons on Joystick then touch the sreen to assign corresponding action ");

			GUI.DrawTexture(new Rect((Screen.width/3)*2,(Screen.height/8)*0,Screen.width/3,Screen.height/8),default2_icon,ScaleMode.StretchToFill,true);
			if (GUI.Button(new Rect((Screen.width/3)*2,(Screen.height/8)*0,Screen.width/3,Screen.height/8), "SNIPE:   " + MainEventManager.StateManager.GameState.myJoyStick.bt_sniping )){
				Debug.Log("Clicked the button with text " + MainEventManager.StateManager.GameState.myJoyStick.bt_sniping);
				MainEventManager.StateManager.GameState.myJoyStick.bt_sniping = last_bt_pressed;
			}
			GUI.DrawTexture(new Rect((Screen.width/3)*2,(Screen.height/8)*1,Screen.width/3,Screen.height/8),default2_icon,ScaleMode.StretchToFill,true);
			if (GUI.Button(new Rect((Screen.width/3)*2,(Screen.height/8)*1,Screen.width/3,Screen.height/8), "TRIGGER: " + MainEventManager.StateManager.GameState.myJoyStick.bt_shoot )){
				Debug.Log("Clicked the button with text " + MainEventManager.StateManager.GameState.myJoyStick.bt_shoot);
				MainEventManager.StateManager.GameState.myJoyStick.bt_shoot = last_bt_pressed;
			}
			GUI.DrawTexture(new Rect((Screen.width/3)*2,(Screen.height/8)*2,Screen.width/3,Screen.height/8),default2_icon,ScaleMode.StretchToFill,true);
			if (GUI.Button(new Rect((Screen.width/3)*2,(Screen.height/8)*2,Screen.width/3,Screen.height/8), "ENTER:   " + MainEventManager.StateManager.GameState.myJoyStick.bt_enter)){
				MainEventManager.StateManager.GameState.myJoyStick.bt_enter = last_bt_pressed;
			}
			GUI.DrawTexture(new Rect((Screen.width/3)*2,(Screen.height/8)*3,Screen.width/3,Screen.height/8),default2_icon,ScaleMode.StretchToFill,true);
			if (GUI.Button(new Rect((Screen.width/3)*2,(Screen.height/8)*3,Screen.width/3,Screen.height/8), "RETURN:  " + MainEventManager.StateManager.GameState.myJoyStick.bt_return)){
				MainEventManager.StateManager.GameState.myJoyStick.bt_return = last_bt_pressed;
			}

			GUI.backgroundColor = defaultColor;
//			if (GUI.Button(new Rect((Screen.width/3)*1,(Screen.height/8)*5,Screen.width/3,Screen.height/8), infoContent)){
//				b_checkConfig = true;
//			}
			if (GUI.Button(new Rect((Screen.width/3)*1,(Screen.height/8)*6,Screen.width/3,Screen.height/8), returnContent)){
				MainEventManager.StateManager.Save(Application.persistentDataPath + Path.DirectorySeparatorChar + MainEventManager.StateManager.GameState.Player.PlayerName + ".xml");
				PhotonNetwork.LoadLevel (SceneLogin);
			}		
			if (GUI.Button(new Rect((Screen.width/3)*1,(Screen.height/8)*7,Screen.width/3,Screen.height/8), "***Remove Button***")){
				removeWord.Add(last_bt_pressed);
			}
			
			//Last Buttons pressed
		//	GUI.Label(new Rect((Screen.width/3)*1,(Screen.height/8)*2,Screen.width/3,Screen.height/8), key_pressed);
			GUI.Label(new Rect((Screen.width/3)*1,(Screen.height/8)*4,Screen.width/3,Screen.height/8), last_bt_pressed);
		}
		// CHECK IF ALL BUTTONS MAPPED CORRECTLY
//		if(b_checkConfig){
//
//		
//			if(!b_info){
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.m_forward)){
//					btId = "forward";
//				}
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.m_backward)){
//					btId = "backward";
//				}
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.m_left)){
//					btId = "left";
//				}
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.m_right)){
//					btId = "right";
//				}
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.bt_sniping)){
//					btId = "aim";
//				}
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.bt_enter)){
//					btId = "option 1";
//				}
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.bt_shoot)){
//					btId = "shoot";
//				}			
//				if(last_bt_pressed.Equals(LSM.GameState.myJoyStick.bt_return)){
//					btId = "option 2";
//				}
//					
//
//				DrawLabel(1,4,1,1,buttonTexture,"left"); // LEFT
//				DrawLabel(3,4,1,1,buttonTexture,"right"); // RIGHT
//				DrawLabel(2,3,1,1,buttonTexture,"forward"); // FORWARD
//				DrawLabel(2,5,1,1,buttonTexture,"backward");	// BACKWARD
//
//				DrawLabel(12,3,1,1,bt_texture,"aim"); // AIM
//				DrawLabel(14,3,1,1,bt_texture,"option 1"); // OPT1
//				DrawLabel(12,4,1,1,bt_texture,"shoot"); // SHOOT
//				DrawLabel(14,4,1,1,bt_texture,"option 2");	// OPT2
//
//				GUI.Label(new Rect(x_con*6,y_con*5,x_con*4,y_con), btId);
//						
//				if (GUI.Button(new Rect(x_con*6,y_con,x_con*4,y_con), backContent) ){
//					b_checkConfig = false;
//					b_info = false;
//				}
//			}else if (b_info){
//
//				GUI.Label(new Rect(20,Screen.height/3, Screen.width-40, Screen.height/3*2 ), msg);
//
//			}
//
//			if (GUI.Button(new Rect(x_con*6,y_con*2,x_con*4,y_con), infoContent)){
//				b_info = !b_info;
//			}
//			
//
//		}

#endif



	}
	
	void DrawLabel(int i1, int i2, int i3, int i4, Texture2D t2d, string id){
		if (id.Equals(btId)) {
			GUI.contentColor = Color.red;
		}
		GUI.Label(new Rect(x_con*i1,y_con*i2,x_con*i3,y_con*i4), t2d);
		GUI.contentColor = defaultColor;

	}
	
		//============================================================================================
	//============================================================================================
	void GetKeyCode()
	{
		for(int i = 0; i < 30; i++){
			if(!removeWord.Contains("joystick 1 button "+i.ToString())){
				if(Input.GetButtonDown("joystick 1 button "+i.ToString())){
					last_bt_pressed =  "joystick 1 button "+i.ToString();
					Debug.Log(last_bt_pressed);
				}
			}
		}
		
		for(int n = 1; n <= 20; n++){
			if(!removeWord.Contains("1_"+n+"axis")){
				if( Mathf.Abs(Input.GetAxis("1_"+n+"axis")) == 1 ){
					last_bt_pressed =  "1_"+n+"axis";
					Debug.Log(last_bt_pressed);
				}
			}
		}		
			
	}
	
	//============================================================================================
	//============================================================================================
	//============================================================================================
	//============================================================================================
	string DebugPad()
	{
		string idString = (1).ToString(); //internally joysticks start at 1 not 0
		string txtLine1= " ";
		
		txtLine1 += " but0:" +(Input.GetButton("joystick "+idString+" button 0") ? "1": "0");
		txtLine1 += " but1:" +(Input.GetButton("joystick "+idString+" button 1") ? "1": "0");
		txtLine1 += " but2:" +(Input.GetButton("joystick "+idString+" button 2") ? "1": "0");
		txtLine1 += " but3:" +(Input.GetButton("joystick "+idString+" button 3") ? "1": "0");

		txtLine1 += " but4:" +(Input.GetButton("joystick "+idString+" button 4") ? "1": "0");
		txtLine1 += " but5:" +(Input.GetButton("joystick "+idString+" button 5") ? "1": "0");
		txtLine1 += " but6:" +(Input.GetButton("joystick "+idString+" button 6") ? "1": "0");
		txtLine1 += " but7:" +(Input.GetButton("joystick "+idString+" button 7") ? "1": "0");

		txtLine1 += " but8:" +(Input.GetButton("joystick "+idString+" button 8") ? "1": "0");
		txtLine1 += " but9:" +(Input.GetButton("joystick "+idString+" button 9") ? "1": "0");
		txtLine1 += " but10:" +(Input.GetButton("joystick "+idString+" button 10") ? "1": "0");
		txtLine1 += " but11:" +(Input.GetButton("joystick "+idString+" button 11") ? "1": "0");

		txtLine1 += " but12:" +(Input.GetButton("joystick "+idString+" button 12") ? "1": "0");
		txtLine1 += " but13:" +(Input.GetButton("joystick "+idString+" button 13") ? "1": "0");
		txtLine1 += " but14:" +(Input.GetButton("joystick "+idString+" button 14") ? "1": "0");
		txtLine1 += " but15:" +(Input.GetButton("joystick "+idString+" button 15") ? "1": "0");

		txtLine1 += " but16:" +(Input.GetButton("joystick "+idString+" button 16") ? "1": "0");
		txtLine1 += " but17:" +(Input.GetButton("joystick "+idString+" button 17") ? "1": "0");
		txtLine1 += " but18:" +(Input.GetButton("joystick "+idString+" button 18") ? "1": "0");
		txtLine1 += " but19:" +(Input.GetButton("joystick "+idString+" button 19") ? "1": "0");
		txtLine1 += "\n";
		
		txtLine1 += " X axis:" +Input.GetAxis(idString+"_1axis");  
		txtLine1 += " Y axis:" +Input.GetAxis(idString+"_2axis");
		txtLine1 += " 3rd axis:" +Input.GetAxis(idString+"_3axis");
		txtLine1 += " 4th axis:" +Input.GetAxis(idString+"_4axis");
		txtLine1 += " 5th axis:" +Input.GetAxis(idString+"_5axis");
		txtLine1 += " 6th axis:" +Input.GetAxis(idString+"_6axis");
		txtLine1 += " 7th axis:" +Input.GetAxis(idString+"_7axis");
		txtLine1 += " 8th axis:" +Input.GetAxis(idString+"_8axis");
		txtLine1 += " 9th axis:" +Input.GetAxis(idString+"_9axis");
		txtLine1 += "10th axis:" +Input.GetAxis(idString+"_10axis");
		txtLine1 += "\n\n";

		return txtLine1;
	}//internally joysticks start at 1 not 0
		
	
//	public void SaveGamePrepare(int param){
//		LoadSaveManager.GameStateData.DataController dCon = GameManager.StateManager.GameState.dController;
//		
//		dCon.bt_a = GameManager.controller.bt_a;
//		dCon.bt_b = GameManager.controller.bt_b;
//		dCon.bt_c = GameManager.controller.bt_c;
//		dCon.bt_d = GameManager.controller.bt_d;
//		
//		dCon.bt_start = GameManager.controller.bt_start;
//		dCon.bt_select = GameManager.controller.bt_select;
//		dCon.bt_left = GameManager.controller.bt_left;
//		dCon.bt_right = GameManager.controller.bt_right;
//		
//		dCon.up = GameManager.controller.up;
//		dCon.down = GameManager.controller.down;
//		dCon.left = GameManager.controller.left;
//		dCon.right = GameManager.controller.right;
//		dCon.axis1_x = GameManager.controller.axis1_x;
//		dCon.axis1_y = GameManager.controller.axis1_y;
//		dCon.axis2_x = GameManager.controller.axis2_x;
//		dCon.axis2_y = GameManager.controller.axis2_y;
//		
//	}
	
	
}
