using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOver : MonoBehaviour {
	public bool _debounce = false;
	public Camera left;
	public Camera right;
	public Text msg ;
	public string msgText;
	public int enemiesNum = 0;
	public int cratesNum = 0;
	public PlayerStats PS;
	bool avatarNotFound = true;
	public float missionTimer = 300f;
	public List<string> enemyNames = new List<string>();

	void Start(){
		left.enabled = false;
		right.enabled = false;
		avatarNotFound = true;
		var enemies = GameObject.FindGameObjectsWithTag ("enemy");
		foreach (GameObject e in enemies) {
			enemiesNum++;
			enemyNames.Add (e.transform.name);
		}
		var crates = GameObject.FindGameObjectsWithTag ("Crate");
		foreach (GameObject c in crates) {
			cratesNum++;
		}

		PS = GameObject.Find (MainEventManager.StateManager.GameState.Player.PlayerName).GetComponent<PlayerStats> ();

		Debug.Log (SceneManager.GetActiveScene ());
	}
	void Update(){
		missionTimer -= Time.deltaTime;

		//Debug.Log ("xxxx----win---- ");

		if ( enemiesNum <= 0 ) {
			left.enabled = true;
			right.enabled = true;
			msgText = "Level cleared \n";
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter) ) {
				avatarNotFound = true;
				PS.GetComponent<PhotonView> ().RPC ("Damage2Player", PhotonTargets.All, 0, "WINNER", "WINNER");	
				Debug.Log (" EXIT WIN RETURN");

				switch (SceneManager.GetActiveScene ().name) {
				case "Training1":
					MainEventManager.StateManager.GameState.Lv.TrainingLevel1 = true;
					if (!MainEventManager.StateManager.GameState.LvNames.Contains ("Training2")) {
						MainEventManager.StateManager.GameState.LvNames.Add ("Training2");
						msgText += "Training2 unlocked";
					}
					break;
				case "Training2":
					MainEventManager.StateManager.GameState.Lv.TrainingLevel2 = true;
					if (!MainEventManager.StateManager.GameState.LvNames.Contains ("Stealth")) {
						MainEventManager.StateManager.GameState.LvNames.Add ("Stealth");
						msgText += "Stealth unlocked";
					}
					break;

//				case "Training1":
//					MainEventManager.StateManager.GameState.Lv.TrainingLevel1 = true;
//					break;
//				case "Training2":
//					if (avatarNotFound) {
//						for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
//							if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals ("Ryan")) {
//								avatarNotFound = false;
//							}
//						}
//
//					}
//					if (avatarNotFound && missionTimer > 0) {
//						msgText += "Amanda is now unlock";
//						avatarNotFound = false;
//						MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Ryan", 200, 2.0f, 19));
//					}
//					break;
				case "Stealth":
					MainEventManager.StateManager.GameState.Lv.Stealth = true;
					
					if (!MainEventManager.StateManager.GameState.LvNames.Contains ("Hangar")) {
						MainEventManager.StateManager.GameState.LvNames.Add ("Hangar");
						msgText += "Hangar unlocked";
					}
					if (avatarNotFound) {
						for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
							Debug.Log ("Avatar name " + MainEventManager.StateManager.GameState.dAvatar [i].name);
							if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals ("Ryan")) {
								avatarNotFound = false;
								Debug.Log ("Found *******RYAN******** " + MainEventManager.StateManager.GameState.dAvatar [i].name);
							}
						}
					}
					if (avatarNotFound && missionTimer > 0) {
						avatarNotFound = false;
						MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Ryan", 200, 2.0f, 10));
						msgText += "\nCharacter Ryan unlocked";
					}
					break;
				case "DummyChallenge":
					MainEventManager.StateManager.GameState.Lv.DummyChallenge = true;
					if (!MainEventManager.StateManager.GameState.LvNames.Contains ("Dawn")) {
						MainEventManager.StateManager.GameState.LvNames.Add ("Dawn");
						msgText += "Dawn unlocked";
					}
					if (avatarNotFound) {
						for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
							if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals ("Amanda")) {
								avatarNotFound = false;
							}
						}
					}
					if (avatarNotFound && missionTimer > 0) {
						msgText += "Amanda is now unlock";
						avatarNotFound = false;
						MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Amanda", 150, 2.3f, 8));
						msgText += "\nCharacter Amanda unlocked";
					}
					break;
				case "Hangar":
					MainEventManager.StateManager.GameState.Lv.Dawn = true;
					if (!MainEventManager.StateManager.GameState.LvNames.Contains ("DummyChallenge")) {
						MainEventManager.StateManager.GameState.LvNames.Add ("DummyChallenge");
						msgText += "Dawn unlocked";
					}
					if (avatarNotFound) {
						for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
							if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals ("Brute")) {
								avatarNotFound = false;
							}
						}
					}
					if (avatarNotFound && missionTimer > 0) {
						msgText += "Brute is now unlock";
						avatarNotFound = false;
						MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Brute", 400, 1f, 20));
						msgText += "\nCharacter Brute unlocked";
					}
					break;
				case "Dawn":
					MainEventManager.StateManager.GameState.Lv.Snipping = true;
					if (!MainEventManager.StateManager.GameState.LvNames.Contains ("Snipping")) {
						MainEventManager.StateManager.GameState.LvNames.Add ("Snipping");
						msgText += "Snipping unlocked";
					}
					if (avatarNotFound) {
						for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
							if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals ("Eve")) {
								avatarNotFound = false;
							}
						}
					}
					if (avatarNotFound && missionTimer > 0) {
						msgText += "Eve is now unlock";
						avatarNotFound = false;
						MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Eve", 180, 2.2f, 10));
						msgText += "\nCharacter Eve unlocked";
					}
					break;
				case "Abandoned":

					break;
				}
				updateWinner ();
				int kills = MainEventManager.StateManager.GameState.Player.kill;
				string Username = MainEventManager.StateManager.GameState.Player.PlayerName;

				DB db; 
				string myQuery = "http://gunart-onlineproject.rhcloud.com/update_gao_kill.php?mNAME=" + Username + "&mKILL="  + kills +"&mACTION=Winner";
				db = this.gameObject.GetComponent<DB> ();
				string results = db.GET(myQuery).ToString();
				MainEventManager.StateManager.Save (Application.persistentDataPath + Path.DirectorySeparatorChar + MainEventManager.StateManager.GameState.Player.PlayerName + ".xml");
				// create function for adding new avatar in list in loadmanager 
//				MainEventManager.StateManager.GameState.dAvatar.Add (new LoadSaveManager.GameStateData.DataAvatar ("Ryan",200,1,20));
//				MainEventManager.StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");
				PhotonNetwork.LeaveRoom();
			}
			msg.text = msgText;
		}
//#########END OF CRATES MISSION
		if (PS.Health <= 0) {
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter)) {
				Debug.Log (" EXIT LOST RETURN");
				Debug.Log (PS.Health);
				MainEventManager.StateManager.Save (Application.persistentDataPath + Path.DirectorySeparatorChar + MainEventManager.StateManager.GameState.Player.PlayerName + ".xml");
				PhotonNetwork.LeaveRoom();
				Debug.Log ("xxxx----lost----");
			}
		}
	}

	public void updateWinner(){
		PS.GetComponent<PlayerStats> ().GetComponent<PhotonView> ().RPC ("Damage2Player", PhotonTargets.All, 0,"WINNER", "WINNER");	
		for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
			if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals (MainEventManager.StateManager.GameState.Avatar.name) && missionTimer > 0){
				Debug.Log ("Found avatar");

				MainEventManager.StateManager.GameState.dAvatar [i].level -= (MainEventManager.StateManager.GameState.dAvatar [i].rank*10);
				if (MainEventManager.StateManager.GameState.dAvatar [i].level < 0) {
					MainEventManager.StateManager.GameState.dAvatar [i].rank++;
					MainEventManager.StateManager.GameState.dAvatar [i].level = MainEventManager.StateManager.GameState.dAvatar [i].nextLevel;
					MainEventManager.StateManager.GameState.dAvatar [i].nextLevel = (int)(1.5f * MainEventManager.StateManager.GameState.dAvatar [i].level);
					MainEventManager.StateManager.GameState.dAvatar [i].points += 2;
					MainEventManager.StateManager.GameState.Avatar = MainEventManager.StateManager.GameState.dAvatar [i];
				}
			}

		}
	}

}


