using UnityEngine;
using System.Collections;

public class TowerSpinScript : MonoBehaviour {

	public float rotationSpeed;
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0f, rotationSpeed, 0f));
	}
}
