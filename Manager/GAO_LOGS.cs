using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

/*
 * Load/save config dependent on player name
 * to remove mainmanager instant start
 * 
 * */

[ExecuteInEditMode]
public class GAO_LOGS : MonoBehaviour {

	public string Username = "Choose a name";
	public string Password = "Password";
	public string Error = "";
	public string gaoText = "Gun Art Online";
	public string tutorialText = "Gun Art Online";
	string msgConfig = " HOW TO CONFIGURE JOYSTICK " + System.Environment.NewLine +
		"STEP 0 * Press a button on Joy then touch the screen to assign button to action. " + System.Environment.NewLine +
		"STEP 1 * Check your joystick is connected to mobile device. " + System.Environment.NewLine +
		"STEP 2 * If pressing buttons on the joystick not changing the text in the middle of the screen" + System.Environment.NewLine +
		"STEP 3 *Try pressing the 'remove this' ONCE. And go back to STEP 0" + System.Environment.NewLine +
		"***WARNING***." + System.Environment.NewLine +
		"SOME BUTTONS will not be recognized" + System.Environment.NewLine +
		"these have to do with Unity Input_Button_mapping limits," + System.Environment.NewLine +
		"OPPOSIT DIRECTIONS OFTEN HAVE THE SAME NAME, THIS IS NOT A PROBLEM"+ System.Environment.NewLine +
		"You can check your joystick before continuing" + System.Environment.NewLine ;

	string msgShoot = " SHOOTING " + System.Environment.NewLine +
		"STEP 0 * Configure the 6 buttons needed to play the game first " + System.Environment.NewLine +
		"STEP 1 * While holding down the Aim button press the Shoot button " + System.Environment.NewLine +
		"You have to keep holding the the Aim button not press and release " + System.Environment.NewLine + 
		"****WARNING****" + System.Environment.NewLine + 
		"You cannot aim while running or walking " + System.Environment.NewLine +
		"Remember to buy some Ammos from time to time, otherwise it is GAMEOVER " + System.Environment.NewLine ;
	
	string msgInfos = " GAME INFOS " + System.Environment.NewLine +
		"After completing Training, WEAPON SHOP will be availale. " + System.Environment.NewLine +
		"To change weapon, press enter in option while browsing the weapon page " + System.Environment.NewLine +
		"Completing Missions in given time to unlock new Options, Characters and more " + System.Environment.NewLine + 
		"During Missions WEAPONS can be switch in OPTION MENU. " + System.Environment.NewLine + 
		"When Leveling UP, you will get POINTS, which you can use to increase your stats." + System.Environment.NewLine + 
		"Higher CAPACITY means more cash you'll collect" + System.Environment.NewLine +
		"To use points, go in the Player Selection Menu, choose stat you want to increse and  " + System.Environment.NewLine +
		"press the SHOOT button " + System.Environment.NewLine ;

	string networkStatus = "ONLINE";
	string PlayerName = "PlayerName";
	string PlayerPassword = "PlayerPassword";
	string PlayerLog = "PlayerLog";
	int width, height;
	int myFontSize = 1;
	public int fontSizeFactor = 20;
	public bool b_option = false;
	public bool b_login = true;
	public bool tutorial = false;
	bool btnDebounce = false;
	bool b_start = false;
	GUIContent infoContent = new GUIContent ();

	//private TouchScreenKeyboard keyboard; // ************************+++++++++++++++++++++++

	public static readonly string SceneJoystickConfig = "Joystick_config_Scene";
	public Text ErrorText;

	Color defaultColor;
	GUISkin myCustomSkin;
	GUIStyle myGuiStyle = new GUIStyle ();
	//#######################################

	public Texture2D player_icon;
	public Texture2D password_icon;
	public Texture2D default_icon;
	public Texture2D info_icon ;
	// Use this for initialization
	void Start () {
		Error = gaoText;
		width = Screen.width;
		height = Screen.height;
		myFontSize = height / fontSizeFactor;
//		Invoke ("LoadConfig", 0.30f);
//		LoadConfig ();
		myGuiStyle.normal.textColor = Color.black;
		myGuiStyle.alignment = TextAnchor.MiddleCenter;
		myGuiStyle.fontSize = myFontSize;
	
		Username = PlayerPrefs.GetString( PlayerName, "Player" + UnityEngine.Random.Range (1, 10000));
		Password = PlayerPrefs.GetString (PlayerPassword, "Password");
		if (PlayerPrefs.GetString (PlayerLog, "xxxx").Equals ("logged")) {
			LoadConfig ();
		}
		Error = "GUN ART ONLINE";
		infoContent.image = info_icon;
		infoContent.text = "TUTORIAL";
		defaultColor = GUI.backgroundColor;

//		if (!MainEventManager.StateManager.GameState.stats.firstRun ) {
//			b_option = false;
//			b_login = false;
//			Username = MainEventManager.StateManager.GameState.Player.PlayerName ;
//			Password = MainEventManager.StateManager.GameState.Player.Password ;
//			LoadConfig ();
//		}
		if (!System.IO.File.Exists (Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveGame.xml")) {
			MainEventManager.StateManager.Save (Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveGame.xml");
		}
	}
	
	// Update is called once per frame//TODO : load network status
	public bool LoadConfig () {
		if(System.IO.File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + Username + ".xml")){
			Debug.Log ("Loading Game");
			Debug.Log (Application.persistentDataPath);
			MainEventManager.StateManager.Load (Application.persistentDataPath + Path.DirectorySeparatorChar + Username + ".xml");

			if (Password == MainEventManager.StateManager.GameState.Player.Password) {
				Debug.Log ("Loading Player DATA");
				PlayerPrefs.SetString (PlayerName, Username);
				PlayerPrefs.SetString (PlayerPassword, Password);
				PlayerPrefs.SetString (PlayerLog, "logged");
				PlayerPrefs.Save ();
				return true;
			} else {
				Debug.Log ("Player DATA not found");
				Error = "Name or Password are incorrect";
				MainEventManager.StateManager.Load (Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveGame.xml");
				return false;
			}

		}
//		if(MainEventManager.Instance.B_networkStatus){
//			networkStatus = "ONLINE";
//		}else{
//			networkStatus = "OFFLINE";   
//		}
		return false;
	}
		
	IEnumerator debounceBtn(){
		yield return new WaitForSeconds (0.4f);
		btnDebounce = false;
	}

	void OnGUI(){

		ErrorText.text = Error;

		GUI.skin.label.fontSize = myFontSize;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;

		GUI.skin.textField.fontSize = myFontSize;
		GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
	
		GUI.skin.button.fontSize = myFontSize;
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		GUI.backgroundColor = Color.clear;
		//####################################### TUTORIAL #############################################################
		if (tutorial) {
			GUI.Label (new Rect (width/4, height / 9, width / 4*3, height / 9*7), tutorialText);

			GUI.DrawTexture(new Rect(0, height/9, width/4, height/9),default_icon,ScaleMode.ScaleToFit,true);
			if (GUI.Button (new Rect (0, height/9, width/4, height/9), "CONFIG")) {
				tutorialText = msgConfig;
			}
			GUI.DrawTexture(new Rect(0, height/9*3, width/4, height/9),default_icon,ScaleMode.ScaleToFit,true);
			if (GUI.Button (new Rect (0, height/9*3, width/4, height/9), "SHOOT")) {
				tutorialText = msgShoot;
			}
			GUI.DrawTexture(new Rect(0, height/9*5, width/4, height/9),default_icon,ScaleMode.ScaleToFit,true);
			if (GUI.Button (new Rect (0, height/9*5, width/4, height/9), "INFOS")) {
				tutorialText = msgInfos;
			}
			GUI.DrawTexture(new Rect(0, height/9*7, width/4, height/9),default_icon,ScaleMode.ScaleToFit,true);
			if (GUI.Button (new Rect (0, height/9*7, width/4, height/9), "RETURN")) {
				tutorial = false;
			}

			return;
		}








		//###################################################################################################################################################################
		// CHECK FOR FIRST RUN AND SAVE DATA 
		if (b_login && !tutorial) {
			//###########################################################
			if (MainEventManager.Instance.start) { // also check you have conectivity
				GUI.DrawTexture(new Rect(width / 5-myFontSize * 2,height / 5,myFontSize * 2, myFontSize * 2),player_icon,ScaleMode.ScaleToFit,true);
				GUI.DrawTexture(new Rect(width / 5, height / 5, width / 4, myFontSize * 2),default_icon,ScaleMode.ScaleToFit,true);
				Username = GUI.TextField (new Rect (width / 5, height / 5, width / 4, myFontSize * 2), Username, 20);
				GUI.DrawTexture(new Rect(width / 5-myFontSize * 2,height / 5 + myFontSize * 4,myFontSize * 2, myFontSize * 2),password_icon,ScaleMode.ScaleToFit,true);
				GUI.DrawTexture(new Rect(width / 5, height / 5 + myFontSize * 4, width / 4, myFontSize * 2),default_icon,ScaleMode.ScaleToFit,true);
				Password = GUI.TextField  (new Rect (width / 5, height / 5 + myFontSize * 4, width / 4, myFontSize * 2), Password, 20);
				GUI.DrawTexture(new Rect(width / 5, height / 5 + myFontSize * 9, width / 4, myFontSize * 2),default_icon,ScaleMode.ScaleToFit,true);

				if (!btnDebounce && (GUI.Button (new Rect (width / 5, height / 5 + myFontSize * 9, width / 4, myFontSize * 2), "Login") ||
					Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter ))) {
					btnDebounce = true;
					StartCoroutine ("debounceBtn");
					//TODO: Call sun routine to set btnDebounce to false
					if (!System.IO.File.Exists (Application.persistentDataPath + Path.DirectorySeparatorChar + Username + ".xml")) {
						//####################################################
						MainEventManager.StateManager.Load (Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveGame.xml");
						Debug.Log ("File not found. Creating one");
						Debug.Log (Username);
						//add this  where the file data is being created
						Error = "New Player created";
						MainEventManager.StateManager.GameState.LvNames.Add ("Training1");
//						MainEventManager.StateManager.GameState.LvNames.Add ("DummyChallenge");
//						MainEventManager.StateManager.GameState.LvNames.Add ("Hangar");
//						MainEventManager.StateManager.GameState.LvNames.Add ("Snipping");
//						MainEventManager.StateManager.GameState.LvNames.Add ("Dawn");
//						MainEventManager.StateManager.GameState.LvNames.Add ("Stealth");
						MainEventManager.StateManager.GameState.Player.PlayerName = Username;
						MainEventManager.StateManager.GameState.Player.Password = Password;
						MainEventManager.StateManager.Save (Application.persistentDataPath + Path.DirectorySeparatorChar + Username + ".xml");
					} else {
						if (LoadConfig ()) {
							b_option = false;
							b_login = false;
							MainEventManager.StateManager.GameState.stats.firstRun = false;
						}
					}
				}
			}

			if (!btnDebounce && GUI.Button (new Rect (width / 5, height / 5 + myFontSize * 12, width / 4, myFontSize * 2), infoContent)){
				tutorial = true;
				Error = "";
			}

			return;
		} 
		//###################################################################################################################################################################

		if (!b_login && !b_option) {
			Error = "Remember to configure joystick in option first";
			GUI.Label (new Rect (width / 5 + myFontSize * 3, height / 5 + myFontSize * 9, width / 4, myFontSize * 3), "Press ENTER to continue");
			GUI.DrawTexture(new Rect(width / 5 + myFontSize * 4, height / 5 + myFontSize * 12, width / 4, myFontSize * 2),default_icon,ScaleMode.ScaleToFit,true);		
			if (GUI.Button (new Rect (width / 5 + myFontSize * 4, height / 5 + myFontSize * 12, width / 4, myFontSize * 2), "OPTION")) {
				Error = "";
				b_option = true;		
			}
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter ) && !btnDebounce) {
				Debug.Log ("pressing enter");
				Application.LoadLevel ("NewPlayer");
			}
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && !btnDebounce) {
				Debug.Log ("pressing return");
				b_login = true;	
				b_option = false;
				btnDebounce = true;
				StartCoroutine ("debounceBtn");
			}		
		} else {
			GUI.DrawTexture(new Rect (width / 6, height / 5, width/3, myFontSize * 2),default_icon,ScaleMode.StretchToFill,true);		
			if (GUI.Button (new Rect (width / 5, height / 5, width / 4, myFontSize * 2), "JOYSTICK CONFIG")) {
				PhotonNetwork.LoadLevel (SceneJoystickConfig);
			}
			GUI.DrawTexture(new Rect(width / 6 + myFontSize, height / 5 + myFontSize * 3, width/3, myFontSize * 2),default_icon,ScaleMode.StretchToFill,true);		
			if (GUI.Button (new Rect (width / 5 + myFontSize, height / 5 + myFontSize * 3, width / 4, myFontSize * 2), "MODUS : " + networkStatus)) {
				MainEventManager.Instance.B_networkStatus = !MainEventManager.Instance.B_networkStatus;
				if(MainEventManager.Instance.B_networkStatus){
					networkStatus = "ONLINE";
				}else{
					networkStatus = "OFFLINE";   
				}
				MainEventManager.StateManager.GameState.stats.networkStatus = MainEventManager.Instance.B_networkStatus;
				MainEventManager.StateManager.Save(Application.persistentDataPath + Username + ".xml");
			}
			GUI.DrawTexture(new Rect(width / 6 + myFontSize * 3, height / 5 + myFontSize * 6, width / 3, myFontSize * 2),default_icon,ScaleMode.StretchToFill,true);		
			if (GUI.Button (new Rect (width / 5 + myFontSize * 3, height / 5 + myFontSize * 6, width / 4, myFontSize * 2), "ACHIEVEMENTS")) {
				Error = "No ACHIEVEMENTS yet";
			}
			GUI.DrawTexture(new Rect(width / 6 + myFontSize * 4, height / 5 + myFontSize * 12, width / 3, myFontSize * 2),default_icon,ScaleMode.StretchToFill,true);		
			if (GUI.Button (new Rect (width / 5 + myFontSize * 4, height / 5 + myFontSize * 12, width / 4, myFontSize * 2), " RETURN ")) {
				b_option = false;
			}
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return)) {
				Error = "";
				MainEventManager.Instance.start = false;	
				btnDebounce = true;
				StartCoroutine ("debounceBtn");
				b_option = false;	
			}

		}
		
	}
		

	#region Testing credentials in GAO_Log file
	public bool testString(string option){
		Error = "";
		if (Username.Contains (" ") || Password.Contains(" ")) {
			Error = "No spaces in name";
			return false;
		}
		return true;
	}
	#endregion


}
