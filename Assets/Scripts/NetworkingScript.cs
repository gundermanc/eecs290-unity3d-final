using UnityEngine;
using System.Collections;

public class NetworkingScript : Photon.MonoBehaviour {
	// Use this for initialization
	public Transform spawnPoint;
	public int playerNumber = -1;
	void Start () {
		PhotonNetwork.ConnectUsingSettings("alpha 0.1");
	}
	
	void OnGUI(){
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}
	
	void OnJoinedLobby(){
		PhotonNetwork.JoinRandomRoom();
	}
	
	void OnPhotonRandomJoinFailed(){
		PhotonNetwork.CreateRoom(null);
	}
	
	void OnJoinedRoom(){
		playerNumber++;
		GameObject thisPlayer;

		int sortingHat = playerNumber % 3;
		if(sortingHat == (int)Element.Rock){
			thisPlayer = PhotonNetwork.Instantiate("RockPlayer", spawnPoint.position, Quaternion.identity, 0);
		} else if(sortingHat == (int)Element.Paper){
			thisPlayer = PhotonNetwork.Instantiate("PaperPlayer", spawnPoint.position, Quaternion.identity, 0);
		} else {
			thisPlayer = PhotonNetwork.Instantiate("ScissorsPlayer", spawnPoint.position, Quaternion.identity, 0);
		}

		thisPlayer.GetComponentInChildren<Camera>().enabled = true;
		thisPlayer.GetComponent<MouseLook>().enabled = true;
		thisPlayer.GetComponent<CharacterMotor>().enabled = true;
		thisPlayer.GetComponent<FPSInputController>().enabled = true;
		thisPlayer.GetComponent<PlayerControler>().enabled = true;

		//Vector3 spawnPoint = new Vector3(Random.Range(0f, 30f), 1000f, Random.Range(0f, 30f));
	//	GameObject myShip = PhotonNetwork.Instantiate("Ship1", spawnPoint, Quaternion.identity, 0);
	//	myShip.GetComponent<Aircraft>().isPlayer = true;
		//myShip.GetComponent<Aircraft> ().Control ();
		//myShip.transform.parent = GameObject.Find("Players").transform;
		//GameObject thisPlayer = PhotonNetwork.Instantiate("ScissorsPlayer", spawnPoint.position, Quaternion.identity, 0);
	}
}
