using UnityEngine;
using System.Collections;

public class TeamColorScript : Photon.MonoBehaviour {
	
	public void setPlayerMaterial(Material playerMat){
		renderer.material = playerMat;
	}
}
