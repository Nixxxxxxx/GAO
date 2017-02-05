using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public float speed = 30.0f;
	public Transform parentTransform;
	public float ttl = 4.0f;
	public string yourNameOnIt = "";
	public string ownerTag = "";
	public int damage = 20;
	public LayerMask layer1;
	public LayerMask layer2;
	// Use this for initialization
	void Start () {
		StartCoroutine ("moveBulletFwd");

	}
//
//
	IEnumerator moveBulletFwd() {
		while (true) {
			transform.position += (transform.rotation * Vector3.forward * Time.deltaTime * speed);
			if (ttl <= 0) {
				this.gameObject.SetActive (false);
			}
			ttl -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

	}

	public void bulletReset(){
		StopAllCoroutines();
		StartCoroutine ("moveBulletFwd");
	}

	void OnTriggerEnter(Collider hit){
		string parentTag = getLastParent (hit.transform).tag;
		Debug.Log ("hit tag "+ hit.tag + "   last parent of hit " + parentTag);
		RaycastHit rhit;
		if (Physics.Raycast (transform.position, transform.forward, out rhit)) {
			PhotonNetwork.Instantiate("BulletHit", rhit.point, Quaternion.Euler(0,180f,0), 0);
		}

		switch(hit.transform.tag){
		case "Environment":
			Vector3 tmp = transform.position;
			tmp.x = 1000;
			transform.position = tmp;
			break;
		case "head":
	//		Debug.Log("head shot " + hit.transform.name);
			getLastParent(hit.transform).GetComponent<PlayerStats>().GetComponent<PhotonView>().RPC("Damage2Player",PhotonTargets.All,damage*6,yourNameOnIt,parentTag, ownerTag);
			break;
		case "bodyParts":
			getLastParent(hit.transform).GetComponent<PlayerStats>().GetComponent<PhotonView>().RPC("Damage2Player",PhotonTargets.All,damage,yourNameOnIt,parentTag, ownerTag);
			break;
		case "chest":
		//	Debug.Log("Chest");
			getLastParent(hit.transform).GetComponent<PlayerStats>().GetComponent<PhotonView>().RPC("Damage2Player",PhotonTargets.AllBuffered,damage*2,yourNameOnIt,parentTag, ownerTag);
			break;
		case "Crate":
	//		Debug.Log("Crate to do parent target tag");
			hit.transform.GetComponent<Crate>().GetComponent<PhotonView>().RPC("Damage2Object",PhotonTargets.AllBuffered,damage*4,yourNameOnIt,hit.tag, ownerTag);
			break;
		case "Pickup":
			//		Debug.Log("Crate to do parent target tag");
			hit.transform.GetComponent<destruction>().GetComponent<PhotonView>().RPC("Damage2Object",PhotonTargets.AllBuffered,damage*4,yourNameOnIt,hit.tag, ownerTag);
			break;
		case "Pickup2":
			//		Debug.Log("Crate to do parent target tag");
			hit.transform.GetComponent<MetalBarrel>().GetComponent<PhotonView>().RPC("Damage2Object",PhotonTargets.AllBuffered,damage*4,yourNameOnIt,hit.tag, ownerTag);
			break;

		}
		this.transform.position = new Vector3 (1000, 1000, 1000);
	}

	Transform getLastParent(Transform child){
		Transform parentTransform = child;
		while ( parentTransform.parent != null) {
			parentTransform = parentTransform.parent;
		}
	//	Debug.Log (parentTransform.name);
		return parentTransform;
	}
}




