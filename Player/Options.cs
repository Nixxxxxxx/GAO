using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class Options : MonoBehaviour {
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
	public Text missionBriefing = null;
	public Text mission = null;


	public RawImage opt_img;
	public Texture2D t2D_equip;
	public Texture2D t2D_stats;
	public Texture2D t2d_map;
	public Texture2D t2d_training;

	public int pageNumMax = 4;
	public int actualPageIndex = 0;
	public int weaponIndex = 1;
	public int pointsys = 0;
	public int pointsArray = 4;
	[TextArea(6,10)]
	public string info;
	[TextArea(3,10)]
	public string missionName;

	private float debounceAxisGuiInput = 0;
	private float debounceDelay = 0.15f;

	public GameObject STATUS = null;
	public GameObject Maps = null;

	public PlayerController PC;

	public GameObject GG = null;

	Color hightLightColor ;
	Color defaultColor;

	void Start () {
		hightLightColor = new Color (1f,0.5f,0.5f);
		defaultColor = avatarName.color;
		actualPageIndex = 2;
		weaponIndex = 1;
		//weaponIndex = MainEventManager.StateManager.GameState.ActualWeapon.weapon_type;
		resetText ();
		PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		if (STATUS.GetActive ()) {
			STATUS.SetActive (true);
		}
		Maps.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		
		updatePage();

		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			//Application.LoadLevel ("Main_Menu_Scene");

			Debug.Log("OPT1");
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			//Application.LoadLevel ("Login_Scene");
			STATUS.SetActive( !STATUS.GetActive());
			if (STATUS.GetActive ()) {
				//TODO disable playercontroller
				PC.isControllable = false;
				STATUS.SetActive (true);
			} else {
				PC.isControllable = true;
				STATUS.SetActive (false);
			}
			Debug.Log("OPT2");

		}
		debounceAxisGuiInput -= Time.deltaTime;

		if (STATUS.GetActive ()) {
			if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_right) ||
			   (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_right) > 0.7f) || 
				(Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.m_right) > 0.7f)) &&
			   debounceAxisGuiInput <= 0) {
				debounceAxisGuiInput = debounceDelay;
				actualPageIndex++;
				if (actualPageIndex > pageNumMax) {
					actualPageIndex = 0;
				}

				Debug.Log ("Right");

			}
			if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_left) ||
			   (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_left) < -0.7f) ||
				(Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.m_left) < -0.7f)) &&
			   debounceAxisGuiInput <= 0) {
				debounceAxisGuiInput = debounceDelay;
				actualPageIndex--;
				if (actualPageIndex < 0) {
					actualPageIndex = pageNumMax;
				}
				Debug.Log ("Left");
				Debug.Log (opt_img.texture.name);
			}

		}


	}


//	Transform getLastParent(Transform child){
//		Transform parentTransform = child;
//		while ( parentTransform.parent != null) {
//			parentTransform = parentTransform.parent;
//		}
//		Debug.Log (parentTransform.name);
//		return parentTransform;
//	}

	public void updatePage(){

		switch (actualPageIndex) {
		case 0: // STATUS PAGE
			resetText ();
			opt_img.texture = t2D_stats;
			mission.text = "STATS";
			avatarName.color = hightLightColor;
			avatarName.text = MainEventManager.StateManager.GameState.Player.PlayerName;		
			name.text = MainEventManager.StateManager.GameState.Avatar.name;
			rank.text = "Rank\t\t\t\t\t" + MainEventManager.StateManager.GameState.Avatar.rank.ToString ();
			health.text = "Health\t\t\t\t" + MainEventManager.StateManager.GameState.Avatar.health.ToString ();
			defense.text = "Defense \t\t\t" + MainEventManager.StateManager.GameState.Avatar.defense.ToString ();
			speed.text = "Speed\t\t\t\t" + MainEventManager.StateManager.GameState.Avatar.speed.ToString ();
			capacity.text = "Capacity\t\t\t" + MainEventManager.StateManager.GameState.Avatar.capacity.ToString ();
			nextLevel.text = "Next LV\t\t\t" + MainEventManager.StateManager.GameState.Avatar.level.ToString ();
			cash.text = "Cash\t\t\t\t" + MainEventManager.StateManager.GameState.Player.CollectedCash.ToString();
			points.text = "Points\t\t\t\t" + MainEventManager.StateManager.GameState.Avatar.points.ToString ();
			Maps.SetActive (false);
			break;
		case 1: // EQUIPEMENT PAGE
			resetText ();
			Maps.SetActive (false);
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
				debounceAxisGuiInput = debounceDelay;
				Debug.Log ("in option pressing opt1");
				do {
					weaponIndex++;
					if (weaponIndex > MainEventManager.StateManager.GameState.weapons.Count - 1) {
						weaponIndex = 0;
					}
					PC.testweapon.updateActualWeapon ((WEAPON_TYPE)MainEventManager.StateManager.GameState.weapons [weaponIndex].weapon_type, weaponIndex);
					Debug.Log ("weapon index " + weaponIndex);
				} while(!MainEventManager.StateManager.GameState.weapons [weaponIndex].owned);
			}
			mission.text = "EQUIPEMENT";
			opt_img.texture = (Texture2D)Resources.Load (MainEventManager.StateManager.GameState.weapons [weaponIndex].name);
			name.text = "Weapon\t\t " + MainEventManager.StateManager.GameState.weapons [weaponIndex].name;
			health.text = "Damage\t\t " + MainEventManager.StateManager.GameState.weapons [weaponIndex].damage;
			defense.text = "Ammo\t\t\t " + MainEventManager.StateManager.GameState.weapons [weaponIndex].ammo;
			speed.text = "Reload\t\t " + MainEventManager.StateManager.GameState.weapons [weaponIndex].reloadSpeed;
			capacity.text = "Weight\t\t " + MainEventManager.StateManager.GameState.weapons [weaponIndex].weight;
			break;
		case 2:
			resetText ();

			opt_img.texture = t2d_training;
			mission.text = missionName;
			missionBriefing.text = info;
			Maps.SetActive (false);
			break;
		case 3:
			resetText ();
			opt_img.texture = t2d_map;
			mission.text = "MAPS\n\nEnemies :" + GG.GetComponent<GameOver>().enemiesNum + "\nCrates :" + GG.GetComponent<GameOver>().cratesNum + "\nCountDown : " + ((int)GG.GetComponent<GameOver>().missionTimer).ToString();
			Maps.SetActive (true);
			break;
		default:

			break;
		
		}
		if (!STATUS.GetActive ()) {
			Maps.SetActive (false);
		}
	
	}
	public void resetText(){
			opt_img.texture = null;
			name.text = "";
			rank.text = "";
			health.text = "";
			defense.text = "";
			speed.text = "";
			capacity.text = "";
			nextLevel.text = "";
			cash.text = "";
			points.text = "";
			missionBriefing.text = "";
			mission.text = "";
		}


}
