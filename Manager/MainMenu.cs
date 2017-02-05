/* --------------------------------------------------------------------------------------------------------------------
//TODO :
//			after checking for special character join or use split o create name of rooms to keep tracking local
			check first time running.
			//
			// switch code with ID to quickly identify the art of result being returned
			//
			//
				open scene to check credentials
					textboxes for name, password, phne, identity and email
				server side
					create table sql design for holding attributes
						Id
						name
						pasword
						email
						phone-id
						online registration
						details
					check  art of request
						case registration
							case check identity exist
								case yes
									send notification to gao game and request new entry
								case no
									insert data in database
									send mail to player 
						case login 
							case check identity exist
								case yes
									check password match name and phoneID
										case yes 
											send ok to gao
										case no
											send password name does not match					
								case no
									send password name does not match	
				


--------------------------------------------------------------------------------------------------------------------*/

using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Random = UnityEngine.Random;


// add string array

public class MainMenu : MonoBehaviour
{
	public enum MENU_STATE{
		MAIN_MENU,
		NEW_GAME,
		OFFLINE,
		JOIN,
		SHOP,
		CHARACTER_STATUS,

	};


    public GUISkin Skin;
    private Vector2 WidthAndHeight ;
    private string roomName ;

    private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;

	public static readonly string Login_Scene = "Login_Scene";
	public static readonly string Main_Menu_Scene = "Main_Menu_Scene";
	public static readonly string GameOver_Scene = "GameOver_Scene";
	public static readonly string Joystick_config_Scene = "Joystick_config_Scene";

	public static readonly string SceneNameGame = "Default_Level_Scene";
	public static readonly string Training1 = "Training1";
	public static readonly string Training2 = "Training2";
	public static readonly string Stealth = "Stealth";
	public static readonly string Hanger = "Hangar";
	public static readonly string Dawn = "Dawn";
	public static readonly string Snipping = "Snipping";
	public static readonly string DummyChallenge = "DummyChallenge";
	public static readonly string Ammok = "Ammok";
	public static readonly string Rescue_Worker = "Rescue_Worker";
	//THIS IS JUST A DEBUG SCENE
	public static readonly string PlayerMovement = "PlayerMovement";

    private string errorDialog;
    private double timeToClearDialog;

	public string levelName = "";

	private float debounceAxisGuiInput = 0;
	private float debounceDelay = 0.7f;

	//LEVELs NAMES
	public string[] MainUI;
	private int MainUI_index = 0;
	public string[] Levels;
	private int LevelIndex = 0;
	public MENU_STATE menuState;
	private int onlineRoomIndex = 0;

	public List<string> OnlineRooms;
	private int roomCount = 0;

	public GameObject Center_text;
	public GameObject Up_text;
	public GameObject Down_text;
	public Text ErrorConnectionText;
	public string ErrorConnection = "";
	public RawImage msgBackground;
	public Text InfoText = null;

	private AudioSource SFX = null;
	public AudioClip changeAudio, selectAudio;

	List<string> onlinelevels = new List<string> ();



    public string ErrorDialog
    {
        get { return this.errorDialog; }
        private set
        {
            this.errorDialog = value;
            if (!string.IsNullOrEmpty(value))
            {
                this.timeToClearDialog = Time.time + 4.0f;
            }
        }
    }

	public void Start(){
		menuState = MENU_STATE.MAIN_MENU;


		WidthAndHeight = new Vector2 (Screen.width, Screen.height);
		roomName = "Area_" + Random.Range (1, 9999);
		
		msgBackground.enabled = false;
		SFX = GetComponent<AudioSource> ();

		OnlineRooms = new List<string> ();

		for (int i = 2; i < (MainEventManager.StateManager.GameState.LvNames.Count); i++) {
			onlinelevels.Add (MainEventManager.StateManager.GameState.LvNames [i]);
		}
	}


    public void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

		// SETTING NETWORK MODUS
		PhotonNetwork.offlineMode = !MainEventManager.Instance.B_networkStatus;
		
        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("0.9");
        }




        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
	//		PhotonNetwork.playerName = MainEventManager.Instance.Username; // CHANGE AFTER DEBUGGING +++++++++++++++++++++++++++++++++++++++
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }






    public void OnGUI()
    {
		ErrorConnectionText.GetComponent<Text> ().text = ErrorConnection;

		#region Check Connection
		if (this.Skin != null) {
			GUI.skin = this.Skin;
		}
		// Checking Connection status
		if (!PhotonNetwork.connected) {
			if (PhotonNetwork.connecting) {
				//GUILayout.Label ("Connecting to: " + PhotonNetwork.ServerAddress);//+++++++++++++++++++++++
				GUILayout.Label ("Check that you have online access");//+++++++++++++++++++++++
			} else {
				//GUILayout.Label ("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
				GUILayout.Label ("Connecting ERROR ");//+++++++++++++++++++++++
			}
			
			if (this.connectFailed) {
				GUILayout.Label ("Connection failed. Check setup and use Setup Wizard to fix configuration.");
				InfoText.GetComponent<Text> ().text = "No internet access. Change modus to offline in Option to continue";
				//GUILayout.Label(String.Format("Server: {0}", new object[] {PhotonNetwork.ServerAddress})); //+++++++++++++++++++++++++++++++++
				//GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID.Substring(0, 8) + "****"); // only show/log first 8 characters. never log the full AppId.
				
				if (GUILayout.Button ("Try Again", GUILayout.Width (300))) {
					this.connectFailed = false;
					PhotonNetwork.ConnectUsingSettings ("0.9");
				}
				if (GUILayout.Button ("To login", GUILayout.Width (300))) {
					Application.LoadLevel(0);
				}
			}
			
			return;
		}
#endregion

		if (PhotonNetwork.connected) {
			InfoText.GetComponent<Text> ().text = PhotonNetwork.countOfPlayers + " users are online";
		} else {
			InfoText.GetComponent<Text> ().text = "PhotonNetwork is not connected you are offline";
		}

		switch (menuState) {
		case MENU_STATE.MAIN_MENU:
			NavigationGui();
			break;
		case MENU_STATE.NEW_GAME:
			NewGame ();
			break;
		case MENU_STATE.OFFLINE:
			OfflineMission();
			break;
		case MENU_STATE.JOIN:
			JoinOnlineGame();
			break;
		case MENU_STATE.CHARACTER_STATUS:
			//JoinOnlineGame();
			break;
		}
		

    }




	#region Photon ActivityCycle
    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }
	
	public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		switch(levelName){
		case "Training1":
			PhotonNetwork.LoadLevel(Training1);
			break;
		case "Training2":
			PhotonNetwork.LoadLevel(Training2);
			break;
		case "Stealth":
			PhotonNetwork.LoadLevel(Stealth);
			break;
		case "Rescue Worker":
			PhotonNetwork.LoadLevel(Rescue_Worker);
			break;
		case "DummyChallenge":
			PhotonNetwork.LoadLevel(DummyChallenge);
			break;
		case "Ammok":
			PhotonNetwork.LoadLevel(Ammok);
			break;
		case "Hangar":
			PhotonNetwork.LoadLevel (Hanger);
			break;
		case "Dawn":
			PhotonNetwork.LoadLevel (Dawn);
			break;
		case "Snipping":
			PhotonNetwork.LoadLevel (Snipping);
			break;
		default:
			Debug.Log("LevelName could not find scene");
			break;


		}
	}
	
	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("Disconnected from Photon. " + transform.name);
	}

	#endregion


	#region PHOTON FAILED
    public void OnPhotonCreateRoomFailed()
    {
        ErrorDialog = "Error: Can't create room (room name maybe already used).";
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
    }

    public void OnPhotonJoinRoomFailed(object[] cause)
    {
        ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    }

    public void OnPhotonRandomJoinFailed()
    {
        ErrorDialog = "Error: Can't join random room (none found).";
        Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
    }
    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }
	#endregion

	#region Methods
	public bool SpecialCharacterCheck(string line){
		var regexItem = new Regex("^[a-zA-Z0-9_]*$");
		
		if (regexItem.IsMatch (line)) {
			return true;
		} else 
			return false;
	}

	public void resetOnlineRoomList(){
		onlineRoomIndex = 0;
		roomCount = PhotonNetwork.GetRoomList().Length;
		OnlineRooms.Clear();
		OnlineRooms.Add ("BACK TO LOGIN");
		foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList()) {
			//GUILayout.Label (roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
			OnlineRooms.Add(roomInfo.name);
		}
		OnlineRooms.Add ("BACK TO MAIN MENU");
	}
	#endregion


	#region NAVIGATION GUI 
	void NavigationGui(){
		debounceAxisGuiInput -= Time.deltaTime;
		
		

		if (MainEventManager.StateManager.GameState.Lv.TrainingLevel1 && MainEventManager.StateManager.GameState.Lv.TrainingLevel2 && onlinelevels.Count > 0) {
			Up_text.GetComponent<Text> ().text = MainUI [MainUI_index % MainUI.Length];
			Center_text.GetComponent<Text> ().text = MainUI [(MainUI_index + 1) % MainUI.Length];
			Down_text.GetComponent<Text> ().text = MainUI [(MainUI_index + 2) % MainUI.Length];
		} else {
			Up_text.GetComponent<Text> ().text = "";
			Center_text.GetComponent<Text> ().text = "OFFLINE MISSION";
			Down_text.GetComponent<Text> ().text = "";
		}


		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_forward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) > 0.7f)) && debounceAxisGuiInput <= 0) {
			MainUI_index++;
			debounceAxisGuiInput = debounceDelay;
			if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
			ErrorConnection = "";
			msgBackground.enabled =false;
		}
		
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_backward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) < -0.7f)) && debounceAxisGuiInput <= 0) {
			MainUI_index--;
			debounceAxisGuiInput = debounceDelay;
			if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
			ErrorConnection = "";
			msgBackground.enabled =false;
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;

			switch(Center_text.GetComponent<Text> ().text){

			case "ONLINE MISSION":
				Debug.Log("Starting new game");
				LevelIndex = 0;
				menuState = MENU_STATE.NEW_GAME;
				break;
			case "OFFLINE MISSION":
				Debug.Log("Please choose Mision");
				LevelIndex = 0;
				menuState = MENU_STATE.OFFLINE;
				break;
			case "JOIN MISSION":
				Debug.Log("Goodluck");
				if (PhotonNetwork.GetRoomList ().Length == 0) {
					ErrorConnection = "Currently no games are available. \n Rooms will be listed here, when they become available.";
					msgBackground.enabled = true;
				}else{
				resetOnlineRoomList();
				menuState = MENU_STATE.JOIN;
				}

				break;
			case  "WEAPON SHOP":
				Application.LoadLevel("WeaponShop");
				break;

//			case  "BACK TO LOGIN":
//				Application.LoadLevel(0);
//				break;
				
			}
			if(SFX){SFX.PlayOneShot(selectAudio,1.0f);}
		}

		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			Application.LoadLevel("NewPlayer");
		}

		if (MainUI_index < 0) {
			MainUI_index = MainUI.Length-1;
		}
	}
	#endregion


	#region OFFLINE
	public void OfflineMission(){
		debounceAxisGuiInput -= Time.deltaTime;
		int lvs = MainEventManager.StateManager.GameState.LvNames.Count;
		Up_text.GetComponent<Text> ().text = MainEventManager.StateManager.GameState.LvNames[LevelIndex%lvs];
		Center_text.GetComponent<Text> ().text = MainEventManager.StateManager.GameState.LvNames[(LevelIndex+1)%lvs];
		Down_text.GetComponent<Text> ().text = MainEventManager.StateManager.GameState.LvNames [(LevelIndex + 2) % lvs];
		

		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_forward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) > 0.7f)) && debounceAxisGuiInput <= 0) {
			LevelIndex++;
			debounceAxisGuiInput = debounceDelay;
			if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
		}
		
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_backward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) < -0.7f)) && debounceAxisGuiInput <= 0) {
			LevelIndex--;
			debounceAxisGuiInput = debounceDelay;
			if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			if(SFX){SFX.PlayOneShot(selectAudio,1.0f);}
			levelName = Center_text.GetComponent<Text> ().text;
			if (levelName.Equals ("Avatar Selection")) {
				Application.LoadLevel ("NewPlayer");
			} else {
				PhotonNetwork.CreateRoom (levelName + "_" + this.roomName, new RoomOptions () { isVisible = true, maxPlayers = 10 }, null);		
			}
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			menuState = MENU_STATE.MAIN_MENU;	
			if(SFX){SFX.PlayOneShot(selectAudio,1.0f);}
		}
		if (LevelIndex < 0) {
			LevelIndex = lvs-1;
		}
		if (LevelIndex >= lvs) {
			LevelIndex = 0;
		}
	
	}
	#endregion


	#region NEW GAME
	void NewGame(){
		if (LevelIndex > onlinelevels.Count - 1) {
			LevelIndex = 0;
		}
		debounceAxisGuiInput -= Time.deltaTime;
		Debug.Log (" online levels count " + onlinelevels.Count + "   levelindex "+ LevelIndex);
		if (onlinelevels.Count > 2) {
			Debug.Log ("oooooooooooooooooooooooooooooooooo");
			Up_text.GetComponent<Text> ().text = onlinelevels [LevelIndex % onlinelevels.Count];
			Center_text.GetComponent<Text> ().text = onlinelevels [(LevelIndex + 1) % onlinelevels.Count];
			Down_text.GetComponent<Text> ().text = onlinelevels [(LevelIndex + 2) % onlinelevels.Count];
		} else if (onlinelevels.Count == 2) {
			Debug.Log ("yyyyyyyyyyyyyyyyyyyyyyyyyy");
			Up_text.GetComponent<Text> ().text = onlinelevels [(LevelIndex+1) % 2];
			Center_text.GetComponent<Text> ().text = onlinelevels [LevelIndex % 2 ];
			Down_text.GetComponent<Text> ().text = onlinelevels [(LevelIndex+1) % 2 ];
		}else {
			Debug.Log ("mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm");
			Up_text.GetComponent<Text> ().text = "";
			Center_text.GetComponent<Text> ().text = onlinelevels [LevelIndex];
			Down_text.GetComponent<Text> ().text = "";
		}
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_forward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) > 0.7f)) && debounceAxisGuiInput <= 0) {
			LevelIndex++;
			debounceAxisGuiInput = debounceDelay;
			if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
		}
		
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_backward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) < -0.7f)) && debounceAxisGuiInput <= 0) {
			LevelIndex--;
			debounceAxisGuiInput = debounceDelay;
			if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			levelName = Center_text.GetComponent<Text> ().text;
			PhotonNetwork.CreateRoom(levelName+"_"+this.roomName, new RoomOptions() {isVisible = true,maxPlayers = 10}, null);			
			if(SFX){SFX.PlayOneShot(selectAudio,1.0f);}
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			menuState = MENU_STATE.MAIN_MENU;
			if(SFX){SFX.PlayOneShot(selectAudio,1.0f);}
		}
		if (LevelIndex < 0) {
			LevelIndex = onlinelevels.Count-1;
		}
		if (LevelIndex >= onlinelevels.Count) {
			LevelIndex = 0;
		}
	}
	#endregion


	#region JOIN GAME
	void JoinOnlineGame(){

		if (PhotonNetwork.GetRoomList ().Length == 0) {
			menuState = MENU_STATE.MAIN_MENU;
		} else {
			if(PhotonNetwork.GetRoomList ().Length != roomCount){
				resetOnlineRoomList();
			}else{
				//----------------------------------------------------------------------------------------------------------------------------
			
				debounceAxisGuiInput -= Time.deltaTime;	
				
				Up_text.GetComponent<Text> ().text = OnlineRooms[onlineRoomIndex%OnlineRooms.Count];
				Center_text.GetComponent<Text> ().text = OnlineRooms[(onlineRoomIndex+1)%OnlineRooms.Count];
				Down_text.GetComponent<Text> ().text = OnlineRooms[(onlineRoomIndex+2)%OnlineRooms.Count];
				
				if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_forward) ||
				    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) > 0.7f)) && debounceAxisGuiInput <= 0) {
					onlineRoomIndex++;
					debounceAxisGuiInput = debounceDelay;
					if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
				}
				
				if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_backward) ||
				    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) < -0.7f)) && debounceAxisGuiInput <= 0) {
					onlineRoomIndex--;
					debounceAxisGuiInput = debounceDelay;
					if(SFX){SFX.PlayOneShot(changeAudio,1.0f);}
				}
				if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
					debounceAxisGuiInput = debounceDelay;
					levelName = Center_text.GetComponent<Text> ().text;
					PhotonNetwork.JoinRoom(levelName);

					if(SFX){SFX.PlayOneShot(selectAudio,1.0f);}
				}
				if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && debounceAxisGuiInput <= 0) {
					debounceAxisGuiInput = debounceDelay;
					menuState = MENU_STATE.MAIN_MENU;
					if(SFX){SFX.PlayOneShot(selectAudio,1.0f);}
				}
				if (onlineRoomIndex < 0) {
					onlineRoomIndex = OnlineRooms.Count;
				}

			}
		
		}

	}
	#endregion


}













