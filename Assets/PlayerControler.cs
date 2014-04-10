using UnityEngine;
using System.Collections;

public enum Element {Rock, Paper, Scissors};

public class PlayerControler : MonoBehaviour {

	public Element PlayerType;
	public int Health;
	public int ProjectileSpeed;
	public GameObject ProjectileSpawnLocation;
	public GameObject Projectile;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Mouse0)){
			GameObject newProjectile;
			newProjectile = Instantiate(Projectile, ProjectileSpawnLocation.transform.position, ProjectileSpawnLocation.transform.rotation) as GameObject;
			newProjectile.transform.Rotate(0,90f,0);
			newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
			newProjectile.rigidbody.AddTorque(ProjectileSpawnLocation.transform.right * (ProjectileSpeed));
		}
	}

	public int ElementComparer(Element playerType, Element enemyType){
		/* rock - paper = 0 - 1 = -1
		 * rock - scissors = 0 - 2 = -2
		 * 
		 * paper - rock = 1 - 0 = 1
		 * paper - scissors = 1 - 2 = -1
		 * 
		 * scissors - rock = 2 - 0 = 2
		 * scissors - paper = 2 - 1 = 1
		 */
		int result = enemyType - playerType;
		switch(playerType){

			case Element.Rock: 
				// 1 is loss, 2 is win
				if(result == 2){
					result = 1;
				} else if(result == 1) {
					result = -1;
				} else {
					result = 0;
				}
				return result;
				break;

			case Element.Paper:
				// 1 is win -1 is loss
				return enemyType - playerType;
				break;

			case Element.Scissors:
				// 2 is loss, 1 is win
			 result = playerType - enemyType;
				if(result == 2){
					result = 1;
				} else if(result == 1) {
					result = -1;
				} else {
					result = 0;
				}
				return result;
				break;
		}
		return result;
	}
}
