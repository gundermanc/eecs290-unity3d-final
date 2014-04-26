using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	public Element ProjectileType;
	public float baseDamage;
	public int teamNumber;
	public float destroytimer;
	public AudioClip explosionSound;
	private bool active;
	private float createtime;
	private GameObject explosion;

	// Use this for initialization
	void Start () {
		createtime = Time.timeSinceLevelLoad;
		active = true;
	}

	void Update(){
		if (Time.timeSinceLevelLoad - createtime > destroytimer)
			Remove ();
	}

	/*@param: Target - the collider object with which to check to see if a collision has occured with
	 * This function checks to see if the projectile has collided with a player or a tower.
	 * It then plays a particle system and deals damage according to the "elemental type".
	 */
	void OnCollisionEnter(Collision Target){

		if (Target.collider.tag == "Ground") {
			active = false;	
		}

		if((Target.collider.tag == "Player" || Target.collider.tag == "Tower") && active){
			if(teamNumber != Target.transform.GetComponent<ElementalObjectScript>().teamNumber){
				if (Target.collider.tag == "Tower"){
					explosion = PhotonNetwork.Instantiate("WayRadExplosion", transform.position, Quaternion.identity, 0) as GameObject;
					explosion.GetComponent<ParticleSystem>().Play();
					audio.PlayOneShot(explosionSound);
				}
				Element enemyType = Target.transform.GetComponent<ElementalObjectScript>().getElementalType();


				int collisionResult = ElementComparer(ProjectileType, enemyType);
				//Debug.Log("Element Comparer Result: "+collisionResult);

				if(collisionResult < 0){
					Target.transform.GetComponent<PhotonView>().RPC("Hurt", PhotonTargets.All, Target.transform.GetComponent<PhotonView>(), ((int)(baseDamage*.5f)));
				}
				if (collisionResult == 0) {
					Target.transform.GetComponent<PhotonView>().RPC("Hurt", PhotonTargets.All, Target.transform.GetComponent<PhotonView>(), ((int)(baseDamage*1.0f)));
				}
				if (collisionResult > 0) {
					Target.transform.GetComponent<PhotonView>().RPC("Hurt", PhotonTargets.All, Target.transform.GetComponent<PhotonView>(), ((int)(baseDamage*2.0f)));
				}
			}
			active = false;
		}
	}

	/**
	 * @param: playerType - Element of the player (e.g. rock, paper, or scissors)
	 * @param: enemyType - Element of the enemy (e.g. rock, paper, or scissors)
	 * Compares the element of the projectile with the element of the collided with object
	 * returns 1 if the projectile type beats the collided object type
	 * returns -1 if the projectile ype gets beat by the collided object type
	 * returns 0 if they are neutral
	 */
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
		int result = playerType - enemyType;
		switch(playerType){
			
		case Element.Rock: 
			// 1 is loss, 2 is win
			if(result == -2){
				result = 1;
			} else if(result == -1) {
				result = -1;
			} else {
				result = 0;
			}
			return result;
			
		case Element.Paper:
			// 1 is win -1 is loss
			return result;
			
		case Element.Scissors:
			// 2 is loss, 1 is win
			if(result == 1){
				result = 1;
			} else if(result == 2) {
				result = -1;
			} else {
				result = 0;
			}
			return result;
		}
		return result;
	}

	private void Remove(){
		PhotonNetwork.Destroy (this.gameObject);
	}
}
