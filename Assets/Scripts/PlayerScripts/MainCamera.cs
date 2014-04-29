using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public AudioClip[] backgroundMusic;

	// Use this for initialization
	void Start () {
		audio.clip = backgroundMusic[Random.Range(0, 2)];
		audio.Play();
		this.gameObject.GetComponent<AudioListener>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.M)){
			if (this.gameObject.GetComponent<AudioListener>().enabled == true) {
				this.gameObject.GetComponent<AudioListener>().enabled = false;
			} else {
				this.gameObject.GetComponent<AudioListener>().enabled = true;
			}
		}
	}
}
