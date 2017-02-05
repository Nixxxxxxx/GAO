using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour
{
	//  ThirdPersonCamera cameraScript; //+++++++++++++++++++++++ThirdPersonCamera cameraScript+++++++++++++++++++++++++++++++++
	CharacterDynamics controllerScript;
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	
	void Awake()
	{
		//        cameraScript = GetComponent<ThirdPersonCamera>(); //+++++++++++++++++++++++++++ThirdPersonCamera cameraScript+++++++++++++++++++++++++++++
		controllerScript = GetComponent<CharacterDynamics>();

		
		if (photonView.isMine)
		{
			//MINE: local player, simply enable the local scripts
			controllerScript.enabled = true;
			controllerScript.isControllable = true;	
		}
		else
		{           
			controllerScript.enabled = true;
			controllerScript.isControllable = false;
		}
		
		gameObject.name = gameObject.name + photonView.viewID;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			stream.SendNext((int)controllerScript.action_state);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation); 						
		}
		else
		{
			//Network player, receive data
			controllerScript.action_state = (ACTION_STATE)(int)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
	
	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
		}
	}
	
}