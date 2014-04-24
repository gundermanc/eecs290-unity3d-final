using UnityEngine;
using System.Collections;

public class TowerScript : Photon.MonoBehaviour {
	public Element elementType;
	public bool dead = false;
	public int teamNumber;
	public float timeOfDeath;
	public float timeToDestory;
	public float lerpSpeed;

	public void Death(){
		dead = true;
		timeOfDeath = Time.time;
		// tell the GameManager that a tower has died
		GameObject.Find("World Camera").GetComponent<GameManager>().TowerDied(teamNumber, (int)elementType);
	}

	void Update(){
		if(dead){
			GetComponentInChildren<BoxCollider>().enabled = false;
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, (transform.position.y - 0.05f), transform.position.z), lerpSpeed);
		}
		if(timeOfDeath > Time.time - timeToDestory){
			PhotonNetwork.Destroy(this.photonView);
		}
	}
}
