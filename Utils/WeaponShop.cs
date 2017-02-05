using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class WeaponShop : MonoBehaviour {
	//
	public Text weaponName = null;
	public Text damage = null;
	public Text ammo = null;
	public Text weight = null;
	public Text gunPrice = null;
	public Text reloadSpeed = null;
	public Text owned = null;
	public Text cash = null;
	public Text ammoPrice = null;

	public int pointsys = 0;
	int pointsArray = 1;


	public GameObject[] mySelection;
	public int currentWeapon = 0;
	int index = -1;
	private float debounceAxisGuiInput = 0;
	private float debounceDelay = 0.35f;

	public string info;
	Color hightLightColor ;
	Color defaultColor;

	void Start () {
		hightLightColor = new Color (1f,0.5f,0.5f);
		defaultColor = new Color (1f,1f,1f);
//		if (MainEventManager.StateManager.GameState.dAvatar.Count == 0) {
//			Debug.Log ("Why is array avatar empty");
//			MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Dummy",100,2.3f,10));
//			MainEventManager.StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");
//		}
//		weaponName.text = "Player1";
//		updateAvatar ();
//		updateTextHightLight ();
		updateWeapon();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			//Application.LoadLevel ();
			Debug.Log ("Pressing OPT1");
		}
		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_return) && debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			Application.LoadLevel ("Main_Menu_Scene");
		}
		debounceAxisGuiInput -= Time.deltaTime;
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_right) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_right) > 0.7f)) &&
		    debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			currentWeapon++;
			if (currentWeapon >= MainEventManager.StateManager.GameState.weapons.Count) {
				currentWeapon = 0;
			}
			updateWeapon ();
		}
		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_left) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis1_left) > 0.7f)) &&
		    debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			currentWeapon--;
			if (currentWeapon < 0) {
				currentWeapon = MainEventManager.StateManager.GameState.weapons.Count - 1;
			}
			updateWeapon ();
		}



		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_forward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_up) > 0.7f) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.m_forward) > 0.7f)) &&
		    debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			pointsys--;
			if (pointsys < 0) {
				pointsys = pointsArray ;
			}
			Debug.Log ("points " + pointsys);
			updateWeapon ();
		}

		if ((Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.m_backward) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.m_backward) > 0.7f) ||
		    (Input.GetAxisRaw (MainEventManager.StateManager.GameState.myJoyStick.t_axis2_down) > 0.7f)) &&
		    debounceAxisGuiInput <= 0) {
			debounceAxisGuiInput = debounceDelay;
			pointsys++;
			if (pointsys > pointsArray) {
				pointsys = 0;
			}
			Debug.Log ("points " + pointsys);
			updateWeapon ();
		}


		if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_shoot)) {
			switch (pointsys) {
			case 0:
				if (MainEventManager.StateManager.GameState.Player.CollectedCash >= (MainEventManager.StateManager.GameState.weapons [index].price/10)) {
					MainEventManager.StateManager.GameState.Player.CollectedCash -= (MainEventManager.StateManager.GameState.weapons [index].price/10);
					MainEventManager.StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");
				}
				break;
			case 1:
				if (MainEventManager.StateManager.GameState.Player.CollectedCash >= MainEventManager.StateManager.GameState.weapons [index].price &&
					!MainEventManager.StateManager.GameState.weapons [index].owned) {
					MainEventManager.StateManager.GameState.Player.CollectedCash -= MainEventManager.StateManager.GameState.weapons [index].price;
					MainEventManager.StateManager.GameState.weapons [index].owned = true;
					MainEventManager.StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");
				}
				break;
			}
			updateWeapon ();

		}

	}


//	public void updateTextHightLight(){
//		health.color = defaultColor;
//		defense.color = defaultColor;
//		speed.color = defaultColor;
//		capacity.color = defaultColor;
//		switch (pointsys) {
//		case 0:
//			health.color = hightLightColor;
//			break;
//		case 1:
//			defense.color = hightLightColor;
//			break;
//		case 2:
//			speed.color = hightLightColor;
//			break;
//		case 3:
//			capacity.color = hightLightColor;
//			break;
//		default:
//
//			break;
//		}
//	}

	public void updateWeapon(){
		for (int i = 0; i < mySelection.Length; i++) {
			if (i != currentWeapon) {
				mySelection [i].SetActive (false);
			} else {
				mySelection [i].SetActive (true);
				index = MainEventManager.StateManager.getActWeapon (mySelection [i].name);
				Debug.Log ("getting index " + index);
				if (index < 0) {
					Debug.Log ("Error getting weapon " + mySelection [i].name);
				} else {
					weaponName.text = "Model\t\t " + MainEventManager.StateManager.GameState.weapons [index].name;
					damage.text = "Damage\t\t " + MainEventManager.StateManager.GameState.weapons [index].damage;
					ammo.text = "Ammo\t\t\t " + MainEventManager.StateManager.GameState.weapons [index].ammo;
					reloadSpeed.text = "Reload\t\t " + MainEventManager.StateManager.GameState.weapons [index].reloadSpeed;
					weight.text = "Weight\t\t " + MainEventManager.StateManager.GameState.weapons [index].weight;

					if (MainEventManager.StateManager.GameState.weapons [index].owned) {
						owned.text = "";
						gunPrice.text = "OWNED";
					} else {
						owned.text = "Buy " + MainEventManager.StateManager.GameState.weapons [index].name; //########################################################### BUYING THE GUNS
						gunPrice.text = "Price\t\t\t " + MainEventManager.StateManager.GameState.weapons [index].price;
					}
					ammoPrice.text = "100 ammo for " + (int)(MainEventManager.StateManager.GameState.weapons [index].price / 10);
					cash.text = "Cash\t\t\t " + MainEventManager.StateManager.GameState.Player.CollectedCash;
				}
			}
		}

		switch (pointsys) {
		case 0:
			ammoPrice.color = hightLightColor;
			gunPrice.color = defaultColor;
			break;
		case 1:
			ammoPrice.color = defaultColor;
			gunPrice.color = hightLightColor;
			break;

		}
	}


	
}

