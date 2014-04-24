using UnityEngine;
using System.Collections;

public class NetworkingScript : Photon.MonoBehaviour {
	// Use this for initialization
	public Transform spawnPoint;
	public int playerNumber;
	public Material[] teamColors;
	public Transform[] spawnPoints;

	public static GameObject thisPlayer;

	void Start () {
		// STOP, this is now done in the GameManager.StartGame() cdg46
		// PhotonNetwork.ConnectUsingSettings("alpha 0.1");
	}
	
	void OnGUI(){
		// STOP, this is now drawn to the screen in the OnScreenDisplay Manager
		//GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}
	
	void OnJoinedLobby(){
		PhotonNetwork.JoinRandomRoom();
	}
	
	void OnPhotonRandomJoinFailed(){
		PhotonNetwork.CreateRoom(null);
	}
	
	void OnJoinedRoom(){
		playerNumber = PhotonNetwork.playerList.Length;

		// this assigns team 1 or team 2
		int groupNumber;
		if(playerNumber < 4){
			groupNumber = 0;
		} else {
			groupNumber = 1;
		}

		int sortingHat = playerNumber % 3;
		if(sortingHat == (int)Element.Rock){
			if(groupNumber == 0){
				spawnPoint = spawnPoints[1];
			} else {
				spawnPoint = spawnPoints[4];
			}

			thisPlayer = PhotonNetwork.Instantiate("RockPlayerV2", spawnPoint.position, Quaternion.identity, groupNumber);


		} else if(sortingHat == (int)Element.Paper){
			if(groupNumber == 0){
				spawnPoint = spawnPoints[0];
			} else {
				spawnPoint = spawnPoints[3];
			}

			thisPlayer = PhotonNetwork.Instantiate("PaperPlayerV2", spawnPoint.position, Quaternion.identity, groupNumber);
		} else {
			if(groupNumber == 0){
				spawnPoint = spawnPoints[2];
			} else {
				spawnPoint = spawnPoints[5];
			}

			thisPlayer = PhotonNetwork.Instantiate("ScissorsPlayerV2", spawnPoint.position, Quaternion.identity, groupNumber);
		}

		thisPlayer.GetComponent<PlayerControler>().RespawnPoint = spawnPoint;
		thisPlayer.GetComponentInChildren<Camera>().enabled = true;
		thisPlayer.GetComponentInChildren<Camera>().transform.parent = thisPlayer.transform;
		thisPlayer.GetComponentInChildren<AudioListener>().enabled = true;
		thisPlayer.GetComponentInChildren<TeamColorScript>().setPlayerMaterial(teamColors[groupNumber]);
		// need to get all mouse looks not individual ones
		thisPlayer.GetComponent<MouseLook>().enabled = true;
		thisPlayer.GetComponent<CharacterMotor>().enabled = true;
		thisPlayer.GetComponent<FPSInputController>().enabled = true;
		thisPlayer.GetComponent<PlayerControler>().enabled = true;
		thisPlayer.GetComponent<PlayerControler>().teamNumber = groupNumber;
		thisPlayer.GetComponent<ElementalObjectScript>().teamNumber = groupNumber;
	//	thisPlayer.GetComponent<MainCamera>().enabled = true;
	//	thisPlayer.GetComponentInChildren<TeamColorScript>().setPlayerMaterial(teamColors[groupNumber]);

		//Vector3 spawnPoint = new Vector3(Random.Range(0f, 30f), 1000f, Random.Range(0f, 30f));
	//	GameObject myShip = PhotonNetwork.Instantiate("Ship1", spawnPoint, Quaternion.identity, 0);
	//	myShip.GetComponent<Aircraft>().isPlayer = true;
		//myShip.GetComponent<Aircraft> ().Control ();
		//myShip.transform.parent = GameObject.Find("Players").transform;
		//GameObject thisPlayer = PhotonNetwork.Instantiate("ScissorsPlayer", spawnPoint.position, Quaternion.identity, 0);
	}

	public static GameObject GetThisPlayer() {
		return thisPlayer;
	}
}