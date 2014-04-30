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
	public int teamNumber;						//Identifies the team that the player is on
	public Texture2D altTexture;
	public AudioClip PowerUp;
	public bool speedDebuffed;					//Marks the player as debuffed, preventing sprinting
	public float timeSinceDebuffed;				//Stores the time since debuffed
	private bool fatiguedOut;					//Marks the player as fatigued, slowing them down and preventing sprinting
	private float normalAttackCooldownTimer;	//Stores the time left until the next normal attack can be done
	private float specialOneCooldownTimer;		//Stores the time left until the next special #1 attack can be done
	private float specialTwoCooldownTimer;		//Stores the time left until the next special #2 attack can be done
	private bool hasShotOnce = false;
	private bool toDie;
	private bool dead;
	private float deathtime;
	private int deathcount;
	private Camera killcam;
	private Vector3 killcamstart;
	private Vector3 killcamend;
	private int respawnreport;

	public Material[] skyboxMats;

	// Use this for initialization
	void Start () {
		speedDebuffed = false;				//By default the player has not been debuffed
		timeSinceDebuffed = 0.0f;			//Thus the timer is set to 0
		fatiguedOut = false;				//By default they are not out of stamina
		normalAttackCooldownTimer = 0.0f;
		toDie = false;
		dead = false;
		deathtime = -1f;
		deathcount = 0;
		killcam = (Camera) Camera.Instantiate(GameObject.FindWithTag("MainCamera").camera, new Vector3(0, 0, 0), GameObject.FindWithTag("MainCamera").transform.rotation);
		respawnreport = 0;
		//TODO
		//IF SOMETHING WITH CAMERAS BREAKS THIS WILL BE THE CULPRATE I CANT SPELL
		transform.GetComponentInChildren<Camera>().depth = 1;
		//if()
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
				//Rock-Type Attack: Shoots faster and more powerful "Mega Rock"
				if (elementalType == Element.Rock) {
					GameObject newProjectile;
					newProjectile = PhotonNetwork.Instantiate("RockProMega", ProjectileSpawnLocation.transform.position + gameObject.transform.forward*1, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
					//newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
					GameManager.TeamMessage(this.gameObject.GetComponent<ElementalObjectScript>().teamNumber, "Rock Player shot a Mega Rock!", Color.white);
					OnScreenDisplayManager.PostMessage("4 second cooldown!");
				}
				//Paper-Type Attack: Create a static paper-stack wall that blocks objects and disappears after time.
				if (elementalType == Element.Paper) {
					GameObject newPaperWall = PhotonNetwork.Instantiate("PaperWall", gameObject.transform.position + gameObject.transform.forward*5 + Vector3.up*3, gameObject.transform.rotation, 0) as GameObject;
					GameManager.TeamMessage(this.gameObject.GetComponent<ElementalObjectScript>().teamNumber, "Paper Player created a Paper Wall!", Color.white);
					OnScreenDisplayManager.PostMessage("5 second cooldown!");
				}
				//Scissors-Type Attack: Temporarily decrease attack speed
				if (elementalType == Element.Scissors) {
					GameObject newProjectile = PhotonNetwork.Instantiate("TrimmingScissors", ProjectileSpawnLocation.transform.position + gameObject.transform.forward*1, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
					//newProjectile.transform.Rotate(0,90f,0);
					//newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
					//newProjectile.rigidbody.AddTorque(ProjectileSpawnLocation.transform.right * (ProjectileSpeed));
					GameManager.TeamMessage(this.gameObject.GetComponent<ElementalObjectScript>().teamNumber, "Scissors Player used Trim!", Color.white);
					OnScreenDisplayManager.PostMessage("6 second cooldown!");
				}
			}

		} else {		
			if (Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.Mouse0)) {
				//If the cooldown is reset (0), then proceed with the attack and set the cooldown time
				if (specialTwoCooldownTimer == 0) {
					specialTwoCooldownTimer = specialCooldownTwo;
					//Rock-Type Special: Temporarily increase defense
					if (elementalType == Element.Rock) {
						//this.GetComponentInChildren<Renderer>().material.mainTexture = altTexture;
						this.GetComponentInChildren<ElementalObjectScript>().decreaseDefense(-2);
						this.gameObject.audio.PlayOneShot(PowerUp);
						GameManager.TeamMessage(this.gameObject.GetComponent<ElementalObjectScript>().teamNumber, "Rock Player used Harden! Defense increased for 3 seconds!", Color.white);
						OnScreenDisplayManager.PostMessage("12 second cooldown!");
					}
					//Paper-Type Attack: Create a horizontal line of paper airplanes
					if (elementalType == Element.Paper) {
						Vector3 right = new Vector3 (ProjectileSpawnLocation.transform.forward.z, 0, -ProjectileSpawnLocation.transform.forward.x);
						GameObject paperAirplaneA = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
						paperAirplaneA.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
						GameObject paperAirplaneB = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position - right*1, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
						paperAirplaneB.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
						GameObject paperAirplaneC = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position - right*2, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
						paperAirplaneC.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
						GameObject paperAirplaneD = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position + right*1, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
						paperAirplaneD.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
						GameObject paperAirplaneE = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position + right*2, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
						paperAirplaneE.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
						GameManager.TeamMessage(this.gameObject.GetComponent<ElementalObjectScript>().teamNumber, "Paper Player used Copy Machine!", Color.white);
						OnScreenDisplayManager.PostMessage("7 second cooldown!");
					}
					//Scissors-Type Attack: Super Scissors
					if (elementalType == Element.Scissors) {
						GameObject newProjectile = PhotonNetwork.Instantiate("SuperScissors", ProjectileSpawnLocation.transform.position + Vector3.down*1 + gameObject.transform.forward*1, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
						//newProjectile.transform.Rotate(90f,0,0);
						//newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
						GameManager.TeamMessage(this.gameObject.GetComponent<ElementalObjectScript>().teamNumber, "Scissors Player threw Super Scissors!", Color.white);
						OnScreenDisplayManager.PostMessage("8 second cooldown!");
					}
				}
			} else {
			//Launches a normal projectile if the attack cooldown timer is reset, i.e. at 0, and the mouse is left-clicked
			if(Input.GetKeyDown(KeyCode.Mouse0) && (normalAttackCooldownTimer == 0)){
				//Sets the timer to the cooldown value specified for the character
				normalAttackCooldownTimer = NormalAttackCooldown;
				//For hold-click rapid-fire
				hasShotOnce = true;
				//Instantiates the projectile
				GameObject newProjectile;
				newProjectile = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
				//Differentiates the physics depending on what type of projectile it is
					newProjectile.GetComponent<ProjectileScript>().setWhoShot(transform.GetComponent<PhotonView>().viewID);
				newProjectile.GetComponent<ProjectileScript>().teamNumber = teamNumber;
			}
		}
		}

		if (Input.GetKeyUp(KeyCode.Mouse0)) {
			hasShotOnce = false;
		}

		if (Input.GetKey (KeyCode.Mouse0) && (hasShotOnce == true)) {
			if (normalAttackCooldownTimer == 0) {
				normalAttackCooldownTimer = NormalAttackCooldown * 2f;
				//Instantiates the projectile
				GameObject newProjectile;
				newProjectile = PhotonNetwork.Instantiate(Projectile.name, ProjectileSpawnLocation.transform.position, ProjectileSpawnLocation.transform.rotation, 0) as GameObject;
				//Differentiates the physics depending on what type of projectile it is
				/*
				if(elementalType != Element.Paper){
					newProjectile.transform.Rotate(0,90f,0);
					newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
					newProjectile.rigidbody.AddTorque(ProjectileSpawnLocation.transform.right * (ProjectileSpeed));
				} else {
					newProjectile.rigidbody.AddForce(ProjectileSpawnLocation.transform.forward * ProjectileSpeed);
				}
				*/
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
			OnScreenDisplayManager.PostMessage("Special Attack 1 Ready!", Color.yellow);
		}

		//Special attack one cooldown timer slowly decreases over time until it hits 0
		if (specialTwoCooldownTimer > 0) {
			specialTwoCooldownTimer -= Time.deltaTime;
		}
		//In case above timer goes under 0, reset it back to 0
		if (specialTwoCooldownTimer < 0) {
			specialTwoCooldownTimer = 0;
			OnScreenDisplayManager.PostMessage("Special Attack 2 Ready!", Color.yellow);
		}
		
		//Allows sprinting by holding left-shift
		if(Input.GetKeyDown(KeyCode.LeftShift)) {
			//Stamina must be greater than 0, cannot be debuffed or fatigued
			if (stamina > 0 && speedDebuffed == false && fatiguedOut == false) {
				this.gameObject.GetComponent<ElementalObjectScript>().changeMoveSpeed(1);
			}
		}

		//What's this doing here
		if(Input.GetKeyDown(KeyCode.O)) {
			this.gameObject.GetComponent<ElementalObjectScript>().changeMoveSpeed(2);
			this.gameObject.GetComponent<CharacterMotor>().jumping.baseHeight = 20.0f;
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

		//Resets speed and defense to normal eventually
		if(speedDebuffed == true) {
			if (timeSinceDebuffed > 0.1f) {
				timeSinceDebuffed -= Time.deltaTime;
			} else {
				timeSinceDebuffed = 0.0f;
				speedDebuffed = false;
				this.gameObject.GetComponent<ElementalObjectScript>().resetMoveSpeed();
				this.gameObject.GetComponent<ElementalObjectScript>().resetDefense();
				Debug.Log("Move speed and defense reset");
			}
		}

		// update Chris's stamina bar
		OnScreenDisplayManager.SetFatigue ((int)stamina);
	}

	[RPC]
	public void Die(int ID){
		if (gameObject.GetComponent<PhotonView> ().viewID == ID && !dead) {
			if (gameObject.GetComponent<PhotonView>().isMine){
				killcam.depth = 10;
				OnScreenDisplayManager.PostMessage ("DEAD. You will respawn in front of your tower in 15 seconds.", Color.red);
			}
			dead = true;
			deathtime = Time.timeSinceLevelLoad;
 			deathcount++;
			killcamstart = new Vector3 (transform.position.x, transform.position.y + 20, transform.position.z);
			killcamend = new Vector3 (transform.position.x, transform.position.y + 40, transform.position.z);
			transform.position = new Vector3 (0, 0, -10);
		}
	}

	public void Kill(){
		gameObject.GetComponent<PhotonView>().RPC("Die", PhotonTargets.All, gameObject.GetComponent<PhotonView> ().viewID);
	}

	public void SendTeamMessage(int team, string message){
		gameObject.GetComponent<PhotonView>().RPC ("RPCTeamMessage", PhotonTargets.All, team, message);
	}

	[RPC]
	public void RPCTeamMessage(int team, string message){
		if (teamNumber == team && gameObject.GetPhotonView().isMine){
			OnScreenDisplayManager.PostMessage(message, Color.green);
		}
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