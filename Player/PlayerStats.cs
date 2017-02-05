using UnityEngine;

using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerStats : Photon.MonoBehaviour {
	public GameOver gameOver;
	public GameObject options;
	public PlayerController PC = null;
	public int Health = 100; 
	public float Defense = 10;
	//public PhotonPlayerInGame PPIG;
//	public int life = 1;
	// Use this for initialization
	public List<string> enemyNames = new List<string>();
	void Start () {
		Debug.Log ("PlayerStats getting scene by name ");
	
		options = GameObject.FindGameObjectWithTag ("Options");
		gameOver = options.GetComponent<Options> ().GG.GetComponent<GameOver> ();

	//	PPIG = GameObject.Find ("Scripts").GetComponent<PhotonPlayerInGame> ();
		switch(gameObject.tag){
		case "Player":
			//PC.speed = MainEventManager.StateManager.GameState.Avatar.speed;
			Health = MainEventManager.StateManager.GameState.Avatar.health;
			Defense = MainEventManager.StateManager.GameState.Avatar.defense;
			Debug.Log ("XXXXXXXXXX");
			break;
		case "enemy":
			Health = 100;
			break;
		default:
			Health = 50;
			break;

		}

	}
	

	[PunRPC]
	public void GainEXP(int experience, string tagSender){
		Debug.Log ("CALLING from gain exp ");
		for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
			if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals (MainEventManager.StateManager.GameState.Avatar.name)) {
				MainEventManager.StateManager.GameState.dAvatar [i].level -= experience;
				if (MainEventManager.StateManager.GameState.dAvatar [i].level < 0) {
					MainEventManager.StateManager.GameState.dAvatar [i].rank++;
					MainEventManager.StateManager.GameState.dAvatar [i].level = MainEventManager.StateManager.GameState.dAvatar [i].nextLevel;
					MainEventManager.StateManager.GameState.dAvatar [i].nextLevel = (int)(1.5f * MainEventManager.StateManager.GameState.dAvatar [i].level);
					MainEventManager.StateManager.GameState.dAvatar [i].points += 2;

					MainEventManager.StateManager.GameState.Avatar = MainEventManager.StateManager.GameState.dAvatar [i];
				}
			}

		}
		if (tagSender.Equals ("Player")) {
			MainEventManager.StateManager.GameState.Player.kill++;
		}
	}

	[PunRPC]
	public void Damage2Player(int dmg,string fromEnemy, string _tag = "Default") {
		Damage2Player (dmg, fromEnemy, _tag, "Default");
	}

	[PunRPC]
	public void Damage2Player(int dmg,string fromEnemy, string _tag = "Default", string fTag = "Default") {
		Debug.Log (fromEnemy);
		int dmgDiff =  dmg -  (int) ( Defense/dmg);
		if (dmgDiff <= 0) {
			dmgDiff = 0;
		}
		Health -= dmgDiff;
		if (Health <= 0) {
//			Instantiate(expolsion, this.transform.position, Quaternion.identity);
//			if(SFX){SFX.PlayOneShot(explosionAudio, 1.0f);}
//			life--;
//
//			PhotonNetwork.LeaveRoom();
			if (_tag.Equals ("Player")) {
				statsUpdate (ACTION_STATE.DIE, fTag);
				int exp = MainEventManager.StateManager.GameState.Avatar.rank*10;
				GameObject.Find(fromEnemy).GetComponent<PlayerStats>().GetComponent<PhotonView>().RPC("GainEXP",PhotonTargets.AllBuffered,exp,transform.tag);
				for (int i = 0; i < MainEventManager.StateManager.GameState.dAvatar.Count; i++) {
					if (MainEventManager.StateManager.GameState.dAvatar [i].name.Equals (MainEventManager.StateManager.GameState.Avatar.name)) {
						MainEventManager.StateManager.GameState.dAvatar [i].level += (MainEventManager.StateManager.GameState.Avatar.rank*10);				
						MainEventManager.StateManager.GameState.Avatar = MainEventManager.StateManager.GameState.dAvatar [i];
						
					}

				}
				gameOver.msg.text = "GAME OVER \n DISQUALIFIED";
				//TODO
				// camera has to be set to this photonview is mine

				//gameObject.SetActive (false);//**************************************************************+++
//				var enemies = GameObject.FindGameObjectsWithTag ("enemy");
//				foreach (GameObject e in enemies) {
//					e.SetActive (false);
//				}

			} else if (_tag.Equals ("enemy") && !enemyNames.Contains(this.gameObject.transform.name)) { 
				Debug.Log ("One Enemy dead ###########################");
				gameObject.GetComponent<Enemy> ().ChangeState (Enemy.ENEMY_STATE.DEAD);
				gameOver.enemiesNum--;
				PhotonNetwork.Instantiate("Pickups", transform.position, Quaternion.identity, 0);
				Debug.Log (this.gameObject.name + " Enum " + gameOver.enemiesNum);
				enemyNames.Add (this.gameObject.transform.name);

				GameObject fromSender = GameObject.Find(fromEnemy);

				if(fTag.Equals("Player")){
					Debug.Log ( fTag +" <- fTag, Sending GainEXP explicit from enemy sender " + fromSender.tag);
				 	fromSender.GetComponent<PlayerStats>().GetComponent<PhotonView>().RPC("GainEXP",PhotonTargets.AllBuffered,10);
//				if (gameOver.enemiesNum <= 0) {
//					statsUpdate (ACTION_STATE.WIN, _tag);
//					gameOver.msg.text = "Level Cleared \n press ENTER to return to MENU ";
//				}
				}
			}
		}
		if (_tag.Equals ("enemy")){
			if (gameObject.GetComponent<Enemy> ().ActiveState == Enemy.ENEMY_STATE.PATROL) {
				gameObject.GetComponent<Enemy> ().ChangeState (Enemy.ENEMY_STATE.Alert);
			}
		}
		if (_tag.Equals ("WINNER")) { //###################SAVING GAME##########################################
			statsUpdate (ACTION_STATE.WIN, _tag);
			//MainEventManager.StateManager.Save (Application.persistentDataPath + "/SaveGame.xml");
			Debug.Log ("TODO GAMEOVER MESSAGE TEXT");

		}
	}


	void statsUpdate(ACTION_STATE AS, string _tag){
		PlayerController PC = null;
		if (_tag.Equals ("enemy")) {
			PC = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		} else if (_tag.Equals ("Player") || _tag.Equals ("WINNER")) {
			PC = GetComponent<PlayerController> ();
		}
		PC.action_state = AS;

		if (photonView.isMine) {
			gameOver.left.enabled = true;
			gameOver.right.enabled = true;
			PC.vr_leftCam.enabled = false;
			PC.vr_rightCam.enabled = false;
			PC.aimCam.enabled = false;
		}
	}

//	void Update(){
//		if(Health <= 0 && GetComponent<PhotonView>().isMine){
//			if (Input.GetKeyDown(KeyCode.X)) {
//				PhotonNetwork.LeaveRoom();
//			}
//		}
//		
//	}

		void OnLeftRoom()
		{

			Debug.Log ("Playerstats onleft room " + transform.name + "  " );
			//	PhotonNetwork.Disconnect ();
//			if (life <= 0) {
//				Application.LoadLevel (0);
//			}
		}
		


	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

}

//
//using UnityEngine;
//using System.Collections;
//
//public class PlayerStats : MonoBehaviour {
//	
//	public int Health = 100;
//	//public GameObject expolsion;
//	//public AudioClip explosionAudio = null;
//	//private AudioSource SFX = null;
//	
//	int life = 1;
//	
//	//GameObject spawnPt;
//	public static readonly string SceneNameMenu = "Main_Menu_Scene";
//	// Use this for initialization
//	void Start () {
//		//		spawnPt = GameObject.Find ("NetworkManager");
//		//		if (spawnPt == null) {
//		//			Debug.Log("Spawn point not found");
//		//		}
//		//		SFX = GetComponent<AudioSource>();
//		
//	}
//	
//	// Update is called once per frame
//	[PunRPC]
//	public void Damage(int dmg,string text) {
//		Debug.Log (text + "XXXX");
//		Health -= dmg;
//		if (Health <= 0) {
//			//Instantiate(expolsion, this.transform.position, Quaternion.identity);
//			//if(SFX){SFX.PlayOneShot(explosionAudio, 1.0f);}
//			life--;
//			/*
//			if (Health <= 0) { // not happening on my side
//				Health = 100;
//				transform.position = spawnPt.transform.position;
//			}
//*/
//		}
//	}
//	
//	void Update(){
//		if(life <= 0){
//			PhotonNetwork.LeaveRoom();
//		}
//		
//	}
//	
//	void OnLeftRoom()
//	{
//		//	PhotonNetwork.Disconnect ();
//		//	Application.LoadLevel(WorkerMenu.SceneNameMenu);
//	}
//	
//	void Networkdestroy(){
//		//	PhotonNetwork.Destroy (gameObject);
//	}
//	
//	
//	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//	{
//	}
//}
//
