using UnityEngine;
using System.Collections;

public class ProjectileSmoothingScript : Photon.MonoBehaviour {
	
	float smoothingSpeed;
	Vector3 realPosition;
	Quaternion realRotation = Quaternion.identity;
	Vector3 realVelocity;
	// Use this for initialization
	void Start () {
		smoothingSpeed = 0.001f;
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			// the other scripts handle all of this business
		} else {
			// this will be used for movement smoothing
			transform.position = Vector3.Lerp(transform.position, realPosition, smoothingSpeed);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, smoothingSpeed);
			rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, realVelocity, smoothingSpeed);

		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){
			// sending actual position to network
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(rigidbody.velocity);
			//stream.SendNext(rigidbody);
		} else {
			// recieving positional info of other players, and updating position locally
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
			realVelocity = (Vector3)stream.ReceiveNext();
		}
	}
}
