using UnityEngine;
using System.Collections;





public class DynamicAssets : MonoBehaviour {
	public int resistant = 100;
	public Transform explosion = null; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}


	[PunRPC]
	public void Damage(int dmg, string tag = "Default") {
		resistant -= dmg;
//		if (resistant > 0) {
//			if(tag.Equals("Player")){
//	
//			}
//		}
//		
		if (resistant <= 0) {
			if(tag.Equals("RoomDynamicAsset")){
//				PhotonNetwork.LeaveRoom();
				transform.parent.GetComponent<Mission>().MissionNum--;
				PhotonNetwork.Instantiate(explosion.transform.name, transform.position, Quaternion.identity, 0);
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}


	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

}
