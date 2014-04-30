using UnityEngine;
using System.Collections;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

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
		int groupNumber;
		playerNumber = PhotonNetwork.playerList.Length; 

		// this assigns team 1 or team 2 based on which team has more players
		int balance = 0;
		foreach (PhotonPlayer player in PhotonNetwork.otherPlayers) {
			if ((int) player.customProperties["Team Number"] == 1)
				balance++;
			else if((int) player.customProperties["Team Number"] == 0)
				balance--;
		}
		if (balance < 0) {
			OnScreenDisplayManager.PostMessage("You have been assigned to TEAM BLUE", Color.blue);
			groupNumber = 1;
		} else {
			OnScreenDisplayManager.PostMessage("You have been assigned to TEAM RED", Color.red);

			groupNumber = 0;
		}

		OnScreenDisplayManager.PostMessage ("There are currently " 
		                                    + PhotonNetwork.playerList.Length + " players in this game.");

		//Assigns player type based on which is needed to keep the teams reasonably balanced
		int[] types = {0, 1, 2};
		foreach (PhotonPlayer player in PhotonNetwork.otherPlayers) {
			if ((int) player.customProperties["Team Number"] == groupNumber){
				types[(int) player.customProperties["Type"]] += 3;
			}
		}
		int sortingHat = Mathf.Min (types) % 3;

		PhotonHashTable PlayerProperties = new PhotonHashTable() {{"Team Number", groupNumber}, {"Type", sortingHat}};
		PhotonNetwork.player.SetCustomProperties (PlayerProperties);

		if(sortingHat == (int)Element.Rock){
			if(groupNumber == 0){
				spawnPoint = spawnPoints[1];
				thisPlayer = PhotonNetwork.Instantiate("RockPlayerRed", spawnPoint.position, spawnPoint.rotation, 0);
			} else {
				spawnPoint = spawnPoints[4];
				thisPlayer = PhotonNetwork.Instantiate("RockPlayerBlue", spawnPoint.position, spawnPoint.rotation, 0);
			}

		} else if(sortingHat == (int)Element.Paper){
			if(groupNumber == 0){
				spawnPoint = spawnPoints[0];
				thisPlayer = PhotonNetwork.Instantiate("PaperPlayerRed", spawnPoint.position, spawnPoint.rotation, 0);
			} else {
				spawnPoint = spawnPoints[3];
				thisPlayer = PhotonNetwork.Instantiate("PaperPlayerBlue", spawnPoint.position, spawnPoint.rotation, 0);
			}

		} else {
			if(groupNumber == 0){
				spawnPoint = spawnPoints[2];
				thisPlayer = PhotonNetwork.Instantiate("ScissorsPlayerRed", spawnPoint.position, spawnPoint.rotation, 0);
			} else {
				spawnPoint = spawnPoints[5];
				thisPlayer = PhotonNetwork.Instantiate("ScissorsPlayerBlue", spawnPoint.position, spawnPoint.rotation, 0);

			}

		}

		thisPlayer.GetComponent<PlayerControler>().RespawnPoint = spawnPoint;
		thisPlayer.GetComponentInChildren<Camera>().enabled = true;
		thisPlayer.GetComponentInChildren<Camera>().transform.parent = thisPlayer.transform;
		thisPlayer.GetComponentInChildren<AudioListener>().enabled = true;
		// need to get all mouse looks not individual ones
		thisPlayer.GetComponent<MouseLook>().enabled = true;
		thisPlayer.GetComponent<CharacterMotor>().enabled = true;
		thisPlayer.GetComponent<FPSInputController>().enabled = true;
		thisPlayer.GetComponent<PlayerControler>().enabled = true;
		thisPlayer.GetComponent<PlayerControler>().teamNumber = groupNumber;
		thisPlayer.GetComponent<ElementalObjectScript>().teamNumber = groupNumber;
	

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