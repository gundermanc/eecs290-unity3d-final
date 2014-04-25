using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public AudioClip[] backgroundMusic;

	// Use this for initialization
	void Start () {
		audio.clip = backgroundMusic[Random.Range(0, 2)];
		audio.Play ();
		AudioListener.pause = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.M)){
			if (AudioListener.pause == true) {
				AudioListener.pause = false;
			} else {
				AudioListener.pause = true;
			}
		}
	}
}
