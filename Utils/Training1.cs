using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Training1 : MonoBehaviour {

	public bool _debounce = false;
	public Text msg ;
	public int cratesNum = 0;

	public GameObject leftCamera;
	public GameObject rightCamera;

	void Start(){
		leftCamera.SetActive (false);
		rightCamera.SetActive(false);
		var crates = GameObject.FindGameObjectsWithTag ("Crate");
		foreach (GameObject c in crates) {
			cratesNum++;
		}

	}
	//
	void Update(){
		
		if (cratesNum <= 0) {
			leftCamera.SetActive (true);
			rightCamera.SetActive(true);
			if (Input.GetButtonDown (MainEventManager.StateManager.GameState.myJoyStick.bt_enter)) {
				Debug.Log (" EXIT RETURN");
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerStats> ().GetComponent<PhotonView> ().RPC ("Damage2Player", PhotonTargets.All, 0, "WINNER");	
				//PhotonNetwork.LeaveRoom();
				Debug.Log ("xxxx----ccccc----");

			}
		}
	}

}
