using UnityEngine;
using System.Collections;

public class Pickups : MonoBehaviour {
	public int type = 1;
	public int amout = 10;
	float speed = 0.5f;
	float distance = 0f;
	private AudioSource SFX = null;
	public AudioClip pickupClip;
	// Use this for initialization
	void Start () {		
		SFX = GetComponent<AudioSource> ();
		StopAllCoroutines();
		StartCoroutine ("bobPickups");
	}

	IEnumerator bobPickups() {
		while (true) {
			distance += (Time.deltaTime * speed);
			if (distance > 1 || distance < 0) {
				speed = -speed;
			}
			transform.position += (transform.rotation * Vector3.up * Time.deltaTime * speed);
			transform.Rotate (Vector3.up * Time.deltaTime*60,Space.World);
			yield return new WaitForEndOfFrame();
		}

	}
	IEnumerator destroyPickups() {
		if(SFX){SFX.PlayOneShot(pickupClip,1.0f);}
		yield return new WaitForSeconds(1.5f);
		Destroy (this.gameObject);
	}

	void OnTriggerEnter(Collider hit){
		switch(hit.transform.tag){
		case "Player":
			MainEventManager.StateManager.GameState.Player.CollectedCash += (amout *MainEventManager.StateManager.GameState.dAvatar.Capacity);
			Debug.Log ("Amount added o cash " + amout);
			StartCoroutine ("destroyPickups");
			break;
		case "head":
			//		Debug.Log("head shot " + hit.transform.name);
			break;
		case "bodyParts":
			break;
		case "chest":
			//	Debug.Log("Chest");
			break;
		case "Crate":
			//		Debug.Log("Crate to do parent target tag");
			break;

		}
	}
}
