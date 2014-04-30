using UnityEngine;
using System.Collections;

public class RemoveAfterTimeLocal : MonoBehaviour {

	public float time;
	private float creationtime;

	// Use this for initialization
	void Start () {
		creationtime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - creationtime > time)
			Destroy (gameObject);
	}
}
