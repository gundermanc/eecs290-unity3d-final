using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {

	public int ProjectileSpeed;
	public GameObject ProjectileSpawnLocation;
	public GameObject Projectile;
	public Element elementalType;
	public float stamina = 100.0f;
	private bool speedDebuffed = false;
	private bool fatiguedOut = false;
	//private float waitTime = 1.0f; 			Was used for debugging
	//private float timer = 0.0f;				Was used for debugging

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		/* A code to send debug messages every second for testing purposes
		 * timer += Time.deltaTime;
		
		if (timer > waitTime)
		{
			timer = 0.0f;
			Debug.Log("Stamina is currently: "+stamina +fatiguedOut);
		}
		*/

		//When stamina is depleted, player becomes fatigued and slows down
		if (stamina <= 0) {
			this.gameObject.GetComponent<ElementalObjectScript>().resetMoveSpeed();
			this.gameObject.GetComponent<ElementalObjectScript>().changeMoveSpeed(-0.5f);
			fatiguedOut = true;
		}

		//If fatigued and stamina comes above 20, becomes unfatigued
		if (fatiguedOut == true && stamina > 20) {
			fatiguedOut = false;
			this.gameObject.GetComponent<ElementalObjectScript>().resetMoveSpeed();
		}
			
		//Launches a projectile
		if(Input.GetKeyDown(KeyCode.Mouse0)){
			GameObject newProjectile;
			newProjectile = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
			if(elementalType != Element.Paper){
				newProjectile.transform.Rotate(0,90f,0);
				newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
				newProjectile.rigidbody.AddTorque(ProjectileSpawnLocation.transform.right * (ProjectileSpeed));
			} else {
				newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
			}
		}

		//Allows sprinting by holding left-shift
		if(Input.GetKeyDown(KeyCode.LeftShift)) {
			//Stamina must be greater than 0, cannot be debuffed or fatigued
			if (stamina > 0 && speedDebuffed == false && fatiguedOut == false) {
				this.gameObject.GetComponent<ElementalObjectScript>().changeMoveSpeed(1);
			}
		}
		
		//Decreases stamina over time if left-shift is being held and the stamina is greater than or equal to 0
		if(Input.GetKey(KeyCode.LeftShift) && (stamina >= 0)) {
			stamina = stamina - Time.deltaTime * 10;
		} else {
			//If not and stamina isn't maxed, slowly increase it
			if (stamina < 100) {
				stamina += Time.deltaTime * 5;
			}
			//Cap the stamina at 100% in case it goes slightly over
			if (stamina > 100) {
				stamina = 100;
			}
		}

		//Returns the move speed back to normal upon letting go of left-shift, but only if not debuffed or fatigued
		if(Input.GetKeyUp(KeyCode.LeftShift)) {
			if (speedDebuffed == false && fatiguedOut == false) {
				this.gameObject.GetComponent<ElementalObjectScript>().resetMoveSpeed();
			}
		}
	}	
}
