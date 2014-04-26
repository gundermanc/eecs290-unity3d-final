using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {

	public int ProjectileSpeed;					//Speed of the projectile
	public GameObject ProjectileSpawnLocation;  //Location to spawn the projectile, specified by a GameObject
	public GameObject Projectile;				//The projectile GameObject itself
	public Transform RespawnPoint;				//Empty gameobject where player will respawn to
	public Element elementalType;				//The type of the GameObject with this class
	public float stamina = 100.0f;				//Stamina value where 100 equates to 100% stamina, the max
	public float NormalAttackCooldown;			//The minimum wait time between two consecutive normal shooting attacks 
	public float specialCooldownOne;			//The minimum wait time between two consecutive special #1 attacks
	public float specialCooldownTwo;			//The minimum wait time between two consecutive special #2 attacks
	public int teamNumber;
	private bool speedDebuffed;					//Marks the player as debuffed, preventing sprinting
	private bool fatiguedOut;					//Marks the player as fatigued, slowing them down and preventing sprinting
	private float normalAttackCooldownTimer;	//Stores the time left until the next normal attack can be done
	private float specialOneCooldownTimer;		//Stores the time left until the next special #1 attack can be done
	private float specialTwoCooldownTimer;		//Stores the time left until the next special #2 attack can be done
	private bool toDie;
	private bool dead;
	private float deathtime;
	private int deathcount;
	private Camera killcam;
	private Vector3 killcamstart;
	private Vector3 killcamend;
	private int respawnreport;


	// Use this for initialization
	void Start () {
		speedDebuffed = false;				//By default the player has not been debuffed
		fatiguedOut = false;				//By default they are not out of stamina
		normalAttackCooldownTimer = 0.0f;
		toDie = false;
		dead = false;
		deathtime = -1f;
		deathcount = 0;
		killcam = (Camera) Camera.Instantiate(GameObject.FindWithTag("MainCamera").camera, new Vector3(0, 0, 0), GameObject.FindWithTag("MainCamera").transform.rotation);
		killcam.GetComponent<OnScreenDisplayManager> ().enabled = false;
		killcam.GetComponent<GameManager> ().enabled = false;
		respawnreport = 0;
	}
	
	// Update is called once per frame
	void Update () {

		//When stamina is depleted, player becomes fatigued and slows down
		//if (gameObject.GetComponent<ElementalObjectScript>().Health <= 0) {
		//	Die ();	
		//}
		if (dead && deathtime != -1f && Time.timeSinceLevelLoad - deathtime > 15) {
			Respawn();
		}
		if (dead) {
			killcam.transform.position = Vector3.Slerp(killcamstart, killcamend, (Time.timeSinceLevelLoad - deathtime)/15f);
			if (Mathf.Floor(Time.timeSinceLevelLoad - deathtime) > respawnreport){
				respawnreport++;
				OnScreenDisplayManager.PostMessage("Respawning in: " + (15-respawnreport).ToString(), Color.red);
			}
		}
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

		//Special One Attacks; Requires "Q" + Left Mouse-Click
		if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.Mouse0)) {
			//If the cooldown is reset (0), then proceed with the attack and set the cooldown time
			if (specialOneCooldownTimer == 0) {
				specialOneCooldownTimer = specialCooldownOne;
				//Rock-Type Attack
				if (elementalType == Element.Rock) {
				}
				//Paper-Type Attack: Create a static paper-stack wall that blocks objects and disappears after time.
				if (elementalType == Element.Paper) {
					GameObject newPaperWall = PhotonNetwork.Instantiate("PaperWall", gameObject.transform.position + gameObject.transform.forward*5 + Vector3.up*3, gameObject.transform.rotation, 0) as GameObject;
				}
				//Scissors-Type Attack
				if (elementalType == Element.Scissors) {
				}
			}
		} else {			
			//Launches a normal projectile if the attack cooldown timer is reset, i.e. at 0, and the mouse is left-clicked
			if(Input.GetKeyDown(KeyCode.Mouse0) && normalAttackCooldownTimer == 0){
				//Sets the timer to the cooldown value specified for the character
				normalAttackCooldownTimer = NormalAttackCooldown;
				//Instantiates the projectile
				GameObject newProjectile;
				newProjectile = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
				//Differentiates the physics depending on what type of projectile it is
				if(elementalType != Element.Paper){
					newProjectile.transform.Rotate(0,90f,0);
					newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
					newProjectile.rigidbody.AddTorque(ProjectileSpawnLocation.transform.right * (ProjectileSpeed));
				} else {
					newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
				}
				newProjectile.GetComponent<ProjectileScript>().teamNumber = teamNumber;
			}
		}

		//Normal attack cooldown timer slowly decreases over time until it hits 0
		if (normalAttackCooldownTimer > 0) {
			normalAttackCooldownTimer -= Time.deltaTime;
		}
		//In case above timer goes under 0, reset it back to 0
		if (normalAttackCooldownTimer < 0) {
			normalAttackCooldownTimer = 0;
		}

		//Special attack one cooldown timer slowly decreases over time until it hits 0
		if (specialOneCooldownTimer > 0) {
			specialOneCooldownTimer -= Time.deltaTime;
		}
		//In case above timer goes under 0, reset it back to 0
		if (specialOneCooldownTimer < 0) {
			specialOneCooldownTimer = 0;
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
		if(Input.GetKeyUp(KeyCode.H)) {
			//Die();
		}

		// update Chris's stamina bar
		OnScreenDisplayManager.SetFatigue ((int)stamina);
	}

	[RPC]
	public void Die(int ID){
		if (gameObject.GetComponent<PhotonView> ().viewID == ID) {
			killcam.depth = 10;
			dead = true;
			deathtime = Time.timeSinceLevelLoad;
			OnScreenDisplayManager.PostMessage ("DEAD. You will respawn in front of your tower in 15 seconds.", Color.red);
			deathcount++;
			killcamstart = new Vector3 (transform.position.x, transform.position.y + 20, transform.position.z);
			killcamend = new Vector3 (transform.position.x, transform.position.y + 40, transform.position.z);
			transform.position = new Vector3 (0, 0, -10);
		}
	}

	public void Kill(){
		gameObject.GetComponent<PhotonView>().RPC("Die", PhotonTargets.All, gameObject.GetComponent<PhotonView> ().viewID);
	}

	public void Respawn(){
		dead = false;
		gameObject.transform.position = RespawnPoint.position;
		gameObject.transform.rotation = RespawnPoint.rotation;
		killcam.depth = -1;
		respawnreport = 0;
		toDie = false;
	}
}