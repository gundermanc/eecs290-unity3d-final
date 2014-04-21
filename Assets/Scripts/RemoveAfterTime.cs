using UnityEngine;
using System.Collections;

public class RemoveAfterTime : MonoBehaviour {

	public float destroyTime = 5.0f;

	// Use this for initialization
	void Start () {
		//Starts a Coroutine that destroys the object
		StartCoroutine (DestroyObject());
	}

	/* @param destroyTime - The time to wait until destroying the object.
	 * This function simply destroys the object after a set time (as to
	 * not clutter the game screen or overcrowd it with objects).
	 */
	IEnumerator DestroyObject() {
		yield return new WaitForSeconds(destroyTime);
		Destroy (this.gameObject);
	}
	
}
