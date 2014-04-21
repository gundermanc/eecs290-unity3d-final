using UnityEngine;
using System.Collections;

public class TowerScript : MonoBehaviour {
	public bool dead = false;
	public float lerpSpeed;

	public void Death(){
		dead = true;
	}

	void Update(){
		if(dead){
			GetComponentInChildren<BoxCollider>().enabled = false;
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, (transform.position.y - 0.01f), transform.position.z), lerpSpeed);
		}
	}
}
