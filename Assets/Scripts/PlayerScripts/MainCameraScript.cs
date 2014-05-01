using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour {
	
	public AudioClip[] backgroundMusic;
	public AudioClip special;
	public AudioClip[] soundEffects;
	
	// Use this for initialization
	void Start () {
		audio.clip = backgroundMusic[Random.Range(0, 2)];
		audio.Play();
		//this.gameObject.GetComponent<AudioListener>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(Input.GetKeyDown(KeyCode.M)){
			if (AudioListener.pause == true) {
				AudioListener.pause = false;
			} else {
				AudioListener.pause = true;
			}
		}


		if(Input.GetKeyDown(KeyCode.B)) {
			this.audio.Stop ();
			this.audio.PlayOneShot(special);
		}
		*/
	}
	
	public void playSoundEffect(string name) {
		int soundID = 0;
		AudioClip soundToPlay;
		if (name == "MessageSound") {
			soundID = 0;
		}
		soundToPlay = soundEffects [soundID];
		audio.PlayOneShot (soundToPlay);
	}
}
