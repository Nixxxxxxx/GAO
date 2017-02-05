// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerInGame.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonPlayerInGame : Photon.MonoBehaviour
{

	public GameObject Player = null;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
			Debug.Log("Project started without loging.\n Retuning to Login Scene PhotonPlayerInGame With SceneManager LoadScene MainMenu Loging Scene defintion");
            SceneManager.LoadScene(MainMenu.Login_Scene);
            return;
        }

		var sArea = GameObject.FindGameObjectsWithTag ("spawnArea");

		Vector3 sPosition =    sArea[ Random.Range(0,sArea.Length)].transform.position;
        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
//		switch (MainEventManager.StateManager.GameState.Avatar.name) {
//		case ("Ryan"):
//			Player = PhotonNetwork.Instantiate(this.playerPrefabRyan.name, sPosition, Quaternion.identity, 0);
//			break;
//		case ("Dummy"):
//			Player =	PhotonNetwork.Instantiate(this.playerPrefabDummy.name, sPosition, Quaternion.identity, 0);
//			break;
//		case ("Amanda"):
//			Player =	PhotonNetwork.Instantiate(this.playerPrefabAmanda.name, sPosition, Quaternion.identity, 0);
//			break;
//		case ("Rosales"):
//			Player =	PhotonNetwork.Instantiate(this.playerPrefabRosales.name, sPosition, Quaternion.identity, 0);
//			break;
//		}
		Player =	PhotonNetwork.Instantiate(MainEventManager.StateManager.GameState.Avatar.name, sPosition, Quaternion.identity, 0);
		Player.name = MainEventManager.StateManager.GameState.Player.PlayerName;

    }

	/*
    public void OnGUI()
    {
        if (GUILayout.Button("Return to Lobby"))
        {
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room

        }
    }
*/


    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.isLocal)
            {
                message = "You are Master Client now.";
            }
            else
            {
                message = player.name + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)" + transform.name);

        // back to main menu
        SceneManager.LoadScene(MainMenu.Main_Menu_Scene);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu
        SceneManager.LoadScene(MainMenu.Main_Menu_Scene);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu
        SceneManager.LoadScene(MainMenu.Main_Menu_Scene);
    }
}
