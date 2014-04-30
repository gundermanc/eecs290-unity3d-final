using UnityEngine;
using System.Collections;

public class TowerScript : Photon.MonoBehaviour {
	public Element elementType;
	public bool dead = false;
	public int teamNumber;
	public float timeOfDeath;
	public float timeToDestroy;
	public float lerpSpeed;
	public GameObject dustCloud;
	private GameObject cloud;

	[RPC]
	public void Death(int ID){
		if (gameObject.GetComponent<PhotonView> ().viewID == ID) {
			dead = true;
			timeOfDeath = Time.time;
			// tell the GameManager that a tower has died
			GameObject.Find("GameManager").GetComponent<GameManager>().TowerDied(teamNumber, (int)elementType);
			cloud = Instantiate (dustCloud, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity) as GameObject;
			cloud.GetComponent<ParticleSystem> ().Play ();
		}
	}

	void Update(){
		if(dead){
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, (transform.position.y - 0.05f), transform.position.z), lerpSpeed);
		}
		if(timeOfDeath != 0 && timeToDestroy < Time.time - timeOfDeath){
			Destroy(gameObject);
			//PhotonNetwork.Destroy(this.photonView);
		}

		int health = (int)(gameObject.GetComponent<ElementalObjectScript> ().Health / 10f);

		if(health < 0) {
			health = 0;
		}
		// update health bars:
		switch(elementType) {
		case Element.Rock:
			if(teamNumber == 0) {
				OnScreenDisplayManager.SetTeam0RockTowerHealth(health);
			} else {
				OnScreenDisplayManager.SetTeam1RockTowerHealth(health);
			}
			break;
		case Element.Paper:
			if(teamNumber == 0) {
				OnScreenDisplayManager.SetTeam0PaperTowerHealth(health);
			} else {
				OnScreenDisplayManager.SetTeam1PaperTowerHealth(health);
			}
			break;
		case Element.Scissors:
			if(teamNumber == 0) {
				OnScreenDisplayManager.SetTeam0ScissorsTowerHealth(health);
			} else {
				OnScreenDisplayManager.SetTeam1ScissorsTowerHealth(health);
			}
			break;
		}
	}
}
