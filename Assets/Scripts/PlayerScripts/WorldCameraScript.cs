using UnityEngine;
using System.Collections;

public class WorldCameraScript : MonoBehaviour {

	public AudioClip special;
	public AudioClip[] soundEffects;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.N)) {
			this.audio.PlayOneShot(special);
		}
	}

	public void playSoundEffect(string name) {
		int soundID = 0;
		AudioClip soundToPlay;
		if (name == "UISound2") {
			soundID = 1;
		}
		this.enableAL ();
		soundToPlay = soundEffects [soundID];
		audio.PlayOneShot (soundToPlay);
	}

	public void enableAL(){
		GameObject.Find("World Camera").GetComponent<AudioListener>().enabled = true;
	}

	public void disableAL(){
		GameObject.Find("World Camera").GetComponent<AudioListener>().enabled = false;
	}

}
