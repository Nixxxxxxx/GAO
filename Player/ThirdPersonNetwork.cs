using UnityEngine;
using System.Collections;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
	//  ThirdPersonCamera cameraScript; //+++++++++++++++++++++++ThirdPersonCamera cameraScript+++++++++++++++++++++++++++++++++
    PlayerController controllerScript;

	OpendivePlayer opendivePlayer; // do we need the opendivePluginPlayer Here

	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	public GameObject VRcam;//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++Camera
	public GameObject Loop;

    void Awake()
    {
		//        cameraScript = GetComponent<ThirdPersonCamera>(); //+++++++++++++++++++++++++++ThirdPersonCamera cameraScript+++++++++++++++++++++++++++++
        controllerScript = GetComponent<PlayerController>();

		opendivePlayer = GetComponent<OpendivePlayer> ();

         if (photonView.isMine)
        {
            //MINE: local player, simply enable the local scripts
			//            cameraScript.enabled = true;//+++++++++++++++++++++++++ThirdPersonCamera cameraScript+++++++++++++++++++++++++++++++
           
			controllerScript.enabled = true;
			controllerScript.isControllable = true;


			VRcam.SetActive(true);
			Loop.SetActive(true);
			opendivePlayer.enabled = true;

        }
        else
        {           
			//            cameraScript.enabled = false; //+++++++++++++++++++++++++ThirdPersonCamera cameraScript+++++++++++++++++++++++++++++++
            controllerScript.enabled = true;

            controllerScript.isControllable = false;

			VRcam.SetActive(false);
			Loop.SetActive(false);
			opendivePlayer.enabled = false;
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
			stream.SendNext(controllerScript.isSniping);// should be action state int value for all action state
			stream.SendNext(controllerScript.snipeAnimNom); 
			stream.SendNext(controllerScript.shoot);


        }
        else
        {
            //Network player, receive data
            controllerScript.action_state = (ACTION_STATE)(int)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
			controllerScript.isSniping = (bool)stream.ReceiveNext();
			controllerScript.snipeAnimNom = (float)stream.ReceiveNext();
			controllerScript.shoot = (bool)stream.ReceiveNext();
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