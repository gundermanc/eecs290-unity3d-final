using UnityEngine;
using System.Collections;

public class MovementSmoothingScript : Photon.MonoBehaviour {

	float smoothingSpeed;
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	// Use this for initialization
	void Start () {
		smoothingSpeed = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			// the other scripts handle all of this business
		} else {
			// this will be used for movement smoothing
			transform.position = Vector3.Lerp(transform.position, realPosition, smoothingSpeed);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, smoothingSpeed); 
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){
			// sending actual position to network
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		} else {
			// recieving positional info of other players, and updating position locally
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
		}
	}
}
