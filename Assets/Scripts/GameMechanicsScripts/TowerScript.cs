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
			GameObject.Find("World Camera").GetComponent<GameManager>().TowerDied(teamNumber, (int)elementType);
			cloud = Instantiate (dustCloud, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity) as GameObject;
			cloud.GetComponent<ParticleSystem> ().Play ();
		}
	}

	void Update(){
		if(dead){
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, (transform.position.y - 0.05f), transform.position.z), lerpSpeed);
		}
		if(timeOfDeath != 0 && timeToDestroy < Time.time - timeOfDeath){
			cloud.GetComponent<ParticleSystem>().Stop();
			Destroy (cloud);
			Destroy(gameObject);
			//PhotonNetwork.Destroy(this.photonView);
		}
	}
}
