using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public AudioClip[] backgroundMusic;

	// Use this for initialization
	void Start () {
		audio.clip = backgroundMusic[Random.Range(0, 2)];
		audio.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
