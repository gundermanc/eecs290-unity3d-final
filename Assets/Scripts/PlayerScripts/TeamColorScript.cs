using UnityEngine;
using System.Collections;

public class TeamColorScript : Photon.MonoBehaviour {
	
	public void setPlayerMaterial(Material playerMat){
		renderer.materials[0] = playerMat;
		renderer.materials[1] = playerMat;
	}
}
