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
			newProjectile = Instantiate(Projectile, ProjectileSpawnLocation.transform.position, Quaternion.identity) as GameObject;
			newProjectile.rigidbody.AddForce(newProjectile.transform.forward * ProjectileSpeed);
			newProjectile.rigidbody.AddTorque(newProjectile.transform.forward * (-1*ProjectileSpeed));
		}
	}
}
