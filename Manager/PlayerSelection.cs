using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class PlayerSelection : MonoBehaviour {
	//
	public Text avatarName = null;
	public Text name = null;
	public Text rank = null;
	public Text health = null;
	public Text defense = null;
	public Text speed = null;
	public Text capacity = null;
	public Text nextLevel = null;
	public Text cash = null;
	public Text points = null;

	public int pointsys = 0;
	int pointsArray = 4;


	public GameObject[] mySelection;
	public int currentAv = 0;
	private float debounceAxisGuiInput = 0;
	private float debounceDelay = 0.35f;
	public Texture2D leftTextureIcon;
	public Texture2D rightTextureIcon;
	public Texture2D selectionTextureIcon;
	public string info;
	Color hightLightColor ;
	Color defaultColor;

	void Start () {
		hightLightColor = new Color (1f,0.5f,0.5f);
		defaultColor = avatarName.color;
		avatarName.text = "Player1";
		if(System.IO.File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + MainEventManager.StateManager.GameState.Player.PlayerName + ".xml")){
			Debug.Log ("Loading Game");
			MainEventManager.StateManager.Load(Application.persistentDataPath + Path.DirectorySeparatorChar + MainEventManager.StateManager.GameState.Player.PlayerName + ".xml");
		}

		if (MainEventManager.StateManager.GameState.dAvatar.Count == 0) {
			Debug.Log ("Why is array avatar empty");
			MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Rosales", 180, 2.0f, 8));
			MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Dummy",100,1.8f,10));
//			MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Eve", 150, 2.2f, 18));
//			MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Brute", 400, 1.0f, 25));
//			MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Ryan", 400, 1.0f, 25));
//			MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Amanda", 400, 1.0f, 25));

			MainEventManager.StateManager.GameState.weapons.Add (new LoadSaveManager.GameStateData.DataWeapon ("handgun",25,1f,1f,(int)WEAPON_TYPE.HANDGUN,100000,100, false));
			MainEventManager.StateManager.GameState.weapons.Add (new LoadSaveManager.GameStateData.DataWeapon ("M4MB",60,1.1f,3f,(int)WEAPON_TYPE.M4MB,4000,400, true));
			MainEventManager.StateManager.GameState.weapons.Add (new LoadSaveManager.GameStateData.DataWeapon ("AWpv2",75,1f,4f,(int)WEAPON_TYPE.AWpv2, 7000,100, false));
			MainEventManager.StateManager.GameState.weapons.Add (new LoadSaveManager.GameStateData.DataWeapon ("AW50",85,0.9f,5f,(int)WEAPON_TYPE.AW50, 12000,100, false));
			MainEventManager.StateManager.GameState.weapons.Add (new LoadSaveManager.GameStateData.DataWeapon ("ak103",55,0.7f,4.5f,(int)WEAPON_TYPE.ak103, 7000,100, false));


			MainEventManager.StateManager.Save(Application.persistentDataPath + Path.DirectorySeparatorChar + MainEventManager.StateManager.GameState.Player.PlayerName + ".xml");
		}
		updateAvatar ();
		updateTextHightLight ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			Application.LoadLevel ("Main_Menu_Scene");
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			Application.LoadLevel ("Login_Scene");
		}
		debounceAxisGuiInput -= Time.deltaTime;
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_right) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_right) > 0.7f)) && 
			debounceAxisGuiInput <= 0) 
		{
			debounceAxisGuiInput = debounceDelay;
			currentAv++;
			if (currentAv >= MainEventManager.StateManager.GameState.dAvatar.Count) {
				currentAv = 0;
			}
			updateAvatar ();
		}
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_left) ||
			(Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_left) > 0.7f)) && 
			debounceAxisGuiInput <= 0) 
		{
			debounceAxisGuiInput = debounceDelay;
			currentAv--;
			if (currentAv < 0) {
				currentAv = MainEventManager.StateManager.GameState.dAvatar.Count - 1;
			}
			updateAvatar ();
		}



		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_forward) ||
			(Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) > 0.7f) ||
			(Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.m_forward) > 0.7f)) && 
			debounceAxisGuiInput <= 0) 
		{
			pointsys++;
			if (pointsys >= pointsArray) {
				pointsys = 0;
			}
			debounceAxisGuiInput = debounceDelay;
			updateTextHightLight ();
		}

		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_backward) ||
			(Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.m_backward) < -0.7f) ||
			(Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_down) < -0.7f)) && 
			debounceAxisGuiInput <= 0) 
		{
			debounceAxisGuiInput = debounceDelay;
			pointsys--;
			if (pointsys < 0) {
				pointsys = pointsArray-1;
			}
			updateTextHightLight ();
		}


		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_shoot)) {
				if (MainEventManager.StateManager.GameState.Avatar.points > 0) {
					for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
						if ( MainEventManager.StateManager.GameState.dAvatar[i].name.Equals(MainEventManager.StateManager.GameState.Avatar.name)) {
							 MainEventManager.StateManager.GameState.dAvatar [i].points--;
							switch (pointsys) {
							case 0://health
							MainEventManager.StateManager.GameState.dAvatar [i].health = (int)(MainEventManager.StateManager.GameState.dAvatar [i].health*1.1f);
								break;
							case 1:
							MainEventManager.StateManager.GameState.dAvatar [i].defense = (MainEventManager.StateManager.GameState.dAvatar [i].defense* 1.1f);
								break;
							case 2:
								MainEventManager.StateManager.GameState.dAvatar [i].speed += 0.05f;
								break;
							case 3:
								MainEventManager.StateManager.GameState.dAvatar [i].capacity += 3;
								break;

							}
							MainEventManager.StateManager.GameState.Avatar = MainEventManager.StateManager.GameState.dAvatar [i];
						MainEventManager.StateManager.Save(Application.persistentDataPath + Path.DirectorySeparatorChar + MainEventManager.StateManager.GameState.Player.PlayerName + ".xml");
							updateAvatar ();
						}
					}

				}
		}


	}


	public void updateTextHightLight(){
		health.color = defaultColor;
		defense.color = defaultColor;
		speed.color = defaultColor;
		capacity.color = defaultColor;
		switch (pointsys) {
		case 0:
			health.color = hightLightColor;
			break;
		case 1:
			defense.color = hightLightColor;
			break;
		case 2:
			speed.color = hightLightColor;
			break;
		case 3:
			capacity.color = hightLightColor;
			break;
		default:

			break;
		}
	}

	public void updateAvatar(){
		for (int i = 0; i < mySelection.Length; i++) {
			mySelection [i].SetActive (false);
		}
		MainEventManager.StateManager.GameState.Avatar = MainEventManager.StateManager.GameState.dAvatar [currentAv];
		avatarName.color = hightLightColor;
		avatarName.text = MainEventManager.StateManager.GameState.Player.PlayerName;
//		info = "Health : " + MainEventManager.StateManager.GameState.Avatar.health.ToString() + "\n" +
//			"Defense : " + MainEventManager.StateManager.GameState.Avatar.defense.ToString() + "\n" +
//			"Speed : " + MainEventManager.StateManager.GameState.Avatar.speed.ToString();
//		Stats.text = info;

		name.text = MainEventManager.StateManager.GameState.Avatar.name;
		rank.text 		= "Rank   \t\t" + MainEventManager.StateManager.GameState.Avatar.rank.ToString();
		nextLevel.text 	= "Next LV \t\t" + MainEventManager.StateManager.GameState.Avatar.level.ToString();
		cash.text 		= "Cash    \t\t" + MainEventManager.StateManager.GameState.Player.CollectedCash.ToString();
		points.text 	= "Points  \t\t" + MainEventManager.StateManager.GameState.Avatar.points.ToString();


		health.text 	= "Health  \t\t" + MainEventManager.StateManager.GameState.Avatar.health.ToString();
		defense.text  	= "Defense \t\t" + MainEventManager.StateManager.GameState.Avatar.defense.ToString();
		speed.text 		= "Speed   \t\t" + MainEventManager.StateManager.GameState.Avatar.speed.ToString();
		capacity.text 	= "Capacity\t\t" + MainEventManager.StateManager.GameState.Avatar.capacity.ToString();



		for (int i = 0; i < mySelection.Length; i++) {
			if (mySelection [i].name.Equals (MainEventManager.StateManager.GameState.Avatar.name)) {
				mySelection [i].SetActive (true);
			}
		}
	}
		

}
