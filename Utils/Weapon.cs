using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WEAPON_TYPE{
	HANDGUN = 0,
	M4MB = 1,
	AWpv2 = 2,
	AW50 = 3,
	ak103 = 4,
};

public class Weapon : MonoBehaviour {
	public Transform myBullet;
	protected BulletScript bulletScript;
	private AudioSource SFX = null;
	public AudioClip gunShot = null;
//	public Transform gunNauselMA14 = null;
//	public Transform gunNauselHG = null;

	public Transform gunNauselMA14 = null;
	public Transform gunNauselhandgun = null;

	public GameObject handgun = null;
	public GameObject M4MB = null;
	public GameObject AWpv2 = null;
	public GameObject AW50 = null;
	public GameObject ak103 = null;

	public GameObject bullseye = null;

	public Texture2D scopeTexture1;
	public Texture2D scopeTexture2;
	public Texture2D scopeTexture3;
	public Texture2D scopeTexture4;
	public Camera snippingCam = null;

	//	public GameObject m4_gunModel = null;
//	public GameObject hg_gunModel = null;
//	public GameObject sp_gunModel = null;


	public WEAPON_TYPE demo_weapon = WEAPON_TYPE.M4MB;
	public int actualWeaponIndex = 1;
	// Use this for initialization
	void Start () {
		Vector3 pos = transform.position;

		pos.x = 1000;
//		#if UNITY_EDITOR
//			myBullet = (Instantiate(Resources.Load("Bullet")) as GameObject).transform;
//		#elif UNITY_ANDROID
//			myBullet = PhotonNetwork.Instantiate("Bullet", pos, Quaternion.identity, 0).transform;
//		#endif
		myBullet = PhotonNetwork.Instantiate("Bullet", pos, Quaternion.identity, 0).transform;
		bulletScript = myBullet.GetComponent<BulletScript> ();
		bulletScript.yourNameOnIt = getLastParent (this.gameObject.transform).name;
		bulletScript.ownerTag = getLastParent (this.gameObject.transform).tag;
		MainEventManager.StateManager.GameState.ActualWeapon.weapon_type = (int)demo_weapon;
		// TODO use editor to set bullet exit point for each gun
		// TODO add animation to play according the carried gun
		// TODO Modify the PC to resolve any animation issue
		// TODO Attach this script to weapon reference point ################################
		// TODO declare weapon enum type#######################################################
		// TODO set daamage in bulletScript base on weapon type #################################
		// TODO add loop transform for sniping camera
		// TODO add run and shoot possibility
		// TODO add handGun 
		// TODO btn enable and disable
		// TODO change damage of bullet accordingly to weapon type
		// TODO check ethan emeny shooting still working with handgun
		StartCoroutine (weaponDelayReset());
		SFX = GetComponent<AudioSource> ();
	}

	IEnumerator weaponDelayReset(){
		yield return new WaitForSeconds (0.5f);
		updateActualWeapon (demo_weapon, 1);
		Debug.Log ("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
	}

	public void updateActualWeapon(WEAPON_TYPE wt, int index){
		Debug.Log ((int)wt + " WT ");
		resetWeapon ();

		demo_weapon = wt; // This could be source of error if more weapons were to be added
		if ((int)wt == 0) {
			bullseye.SetActive (false);
		} else {
			bullseye.SetActive (true);
		}
		switch (wt) {
		case WEAPON_TYPE.HANDGUN:
			handgun.SetActive (true);
			break;
		case WEAPON_TYPE.M4MB:
			M4MB.SetActive (true);
			bullseye.GetComponent<Renderer> ().material.mainTexture = scopeTexture4;
			snippingCam.fieldOfView = 55;
			break;
		case WEAPON_TYPE.AW50:
			AW50.SetActive (true);
			bullseye.GetComponent<Renderer> ().material.mainTexture = scopeTexture2;
			snippingCam.fieldOfView = 35;
			break;
		case WEAPON_TYPE.AWpv2:
			AWpv2.SetActive (true);
			bullseye.GetComponent<Renderer> ().material.mainTexture = scopeTexture3;
			snippingCam.fieldOfView = 20;
			break;
		case WEAPON_TYPE.ak103:
			ak103.SetActive (true);
			bullseye.GetComponent<Renderer> ().material.mainTexture = scopeTexture1;
			snippingCam.fieldOfView = 65;
			break;

		}
		actualWeaponIndex = index;
		MainEventManager.StateManager.GameState.ActualWeapon = MainEventManager.StateManager.GameState.weapons [index];
		bulletScript.damage = MainEventManager.StateManager.GameState.ActualWeapon.damage;
	}

	public void resetWeapon(){
		handgun.SetActive (false);
		M4MB.SetActive (false);
		AW50.SetActive(false);
		AWpv2.SetActive(false);
		ak103.SetActive(false);
	}

	Transform getLastParent(Transform child){
		Transform parentTransform = child;
		while ( parentTransform.parent != null) {
			parentTransform = parentTransform.parent;
		}
		//	Debug.Log (parentTransform.name);
		return parentTransform;
	}


	public void shoot(){
		Debug.Log ("Shooting outside");
		if (MainEventManager.StateManager.GameState.weapons[actualWeaponIndex].ammo > 0) {
			MainEventManager.StateManager.GameState.weapons [actualWeaponIndex].ammo--;
			Debug.Log ("Shooting inside");
			if (SFX) {
				SFX.PlayOneShot (gunShot, 1.0f);
			}

			myBullet.gameObject.SetActive (true);
			bulletScript.ttl = MainEventManager.StateManager.GameState.ActualWeapon.reloadSpeed;
			bulletScript.bulletReset ();
			Vector3 pos = new Vector3 (0, 0, 0);
			Quaternion rot = new Quaternion ();
			switch (demo_weapon) {
			case WEAPON_TYPE.HANDGUN:
				pos = gunNauselhandgun.position;
				rot = gunNauselhandgun.rotation;
				break;
			case WEAPON_TYPE.M4MB:
			case WEAPON_TYPE.AWpv2:
			case WEAPON_TYPE.AW50:
			case WEAPON_TYPE.ak103:
//			pos = gunNauselHG.position; //################################# TODO add sniper #################
//			rot = gunNauselHG.rotation;
				pos = gunNauselMA14.position;
				rot = gunNauselMA14.rotation;
				break;
			default:
				pos = this.transform.position;
				rot = this.transform.rotation;
				break;
			}

			myBullet.position = pos;
			myBullet.rotation = rot;
		}
	}
//
//	void Update(){
//		if (Input.GetKeyDown (KeyCode.S)) {
//			shoot();
//		}
//	}
}
