using UnityEngine;
using System.Collections;

public class Crate : Photon.MonoBehaviour {
	[SerializeField]
	public int hp = 100;
	public GameObject explosion = null;
	int Defense = 10;
	int exp = 1;
	bool moving = false;

	public float distance = 4;
	int speed = 1;


	void Start () {		
//		SFX = GetComponent<AudioSource> ();
		StopAllCoroutines();
		StartCoroutine ("bobPickups");
	}

	IEnumerator bobPickups() {
		int rInt = Random.Range (0, 4);
		if (rInt == 2) {
			moving = true;
		}
		while (true) {
			distance += (Time.deltaTime * speed);
			if (distance > 4 ) {
				speed = -speed;
			}else if(distance < 0 && speed < 0){
				speed = -speed;
			}
			transform.position += (transform.rotation * Vector3.forward * Time.deltaTime * speed);
		//	transform.Rotate (Vector3.up * Time.deltaTime*60,Space.World);
			yield return new WaitForEndOfFrame();
		}

	}
//	IEnumerator destroyPickups() {
//		if(SFX){SFX.PlayOneShot(pickupClip,1.0f);}
//		yield return new WaitForSeconds(1.5f);
//		Destroy (this.gameObject);
//	}


	[PunRPC]
	public void Damage2Object(int dmg, string fromEnemy,string _tag = "Default", string otherTag = "Default") {
		int dmgDiff =  dmg -  (int) ( Defense/dmg);
 
		if (dmgDiff < 0) {
			dmgDiff = 10;
		}
		Debug.Log (dmgDiff);
		hp -= dmgDiff;
	
		if (hp <= 0) {
			GameObject.Find(fromEnemy).GetComponent<PlayerStats>().GetComponent<PhotonView>().RPC("GainEXP",PhotonTargets.AllBuffered,exp);
			GameObject.FindGameObjectWithTag ("GameOver").transform.GetComponent<GameOver> ().cratesNum -= 1;
			Instantiate (explosion,transform.position, Quaternion.identity);


			if (moving) {
				GameObject pKups = PhotonNetwork.Instantiate ("Pickups", transform.position, Quaternion.identity, 0);
				pKups.GetComponent<Pickups> ().amout = 10;
			}
			Destroy (this.gameObject);
		}

		Debug.Log("inside crate script");
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}
}
