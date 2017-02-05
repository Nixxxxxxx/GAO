using UnityEngine;
using System.Collections;

public class WeaponEnemy : MonoBehaviour {
	public Transform myBullet;
	protected BulletScript bulletScript;
	private AudioSource SFX = null;
	public AudioClip gunShot;
	// Use this for initialization
	void Start () {
		Vector3 pos = transform.position;
		pos.x = 1000;
		myBullet = PhotonNetwork.Instantiate("Bullet", pos, Quaternion.identity, 0).transform;
		bulletScript = myBullet.GetComponent<BulletScript> ();
		bulletScript.ownerTag = getLastParent (transform).tag;
		SFX = GetComponent<AudioSource> ();
	}
	
	public void shoot(){
		if(SFX){SFX.PlayOneShot(gunShot,1.0f);}
		myBullet.gameObject.SetActive (true);
		bulletScript.ttl = 4.0f;
		bulletScript.bulletReset ();
		myBullet.position = this.transform.position;
		myBullet.rotation = this.transform.rotation;
	}

//
//	void Update(){
//		if (Input.GetKeyDown (KeyCode.S)) {
//			shoot();
//		}
	//	}
	Transform getLastParent(Transform child){
		Transform parentTransform = child;
		while ( parentTransform.parent != null) {
			parentTransform = parentTransform.parent;
		}
		//	Debug.Log (parentTransform.name);
		return parentTransform;
	}
}
