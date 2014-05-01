using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	public Element ProjectileType;
	public float baseDamage;
	public int teamNumber;
	public float debuffValue;
	public float ProjectileSpeed;
	private bool Ishot = false;
	private bool active;
	private GameObject explosion;

	// Use this for initialization
	void Start () {
		active = true;
		if(ProjectileType != Element.Paper){
			transform.Rotate(0,90f,0);
			rigidbody.AddForce(transform.right * -ProjectileSpeed);
			rigidbody.AddTorque(transform.forward * (ProjectileSpeed));
		}
		else {
			rigidbody.AddForce(transform.forward * ProjectileSpeed);
		}
	}

	void Update(){
	}

	public void Claim(){
		Ishot = true;
	}

	/*@param: Target - the collider object with which to check to see if a collision has occured with
	 * This function checks to see if the projectile has collided with a player or a tower.
	 * It then plays a particle system and deals damage according to the "elemental type".
	 */
	void OnCollisionEnter(Collision Target){

		if (Target.collider.tag == "Ground") {
			active = false;	
		}

		if((Target.collider.tag == "Player" || Target.collider.tag == "Tower") && active && GameManager.started){
			active = false;
			if(teamNumber != Target.transform.GetComponent<ElementalObjectScript>().teamNumber){
				Element enemyType = Target.transform.GetComponent<ElementalObjectScript>().getElementalType();
				int collisionResult = ElementComparer(ProjectileType, enemyType);
				if (Target.collider.tag == "Tower"){
					if (GameObject.FindGameObjectsWithTag("Explosion").Length < 3){
						explosion = PhotonNetwork.Instantiate("WayRadExplosion", transform.position, Quaternion.identity, 0) as GameObject;
						explosion.GetComponent<ParticleSystem>().Play();
					}
					//
					if(Ishot){
						if(collisionResult < 0){
							Target.transform.GetComponent<ElementalObjectScript>().RPCHurt(Target.transform.parent.GetComponent<PhotonView>().viewID, (int)(baseDamage*.5f), true);
						}
						if (collisionResult == 0) {
							Target.transform.GetComponent<ElementalObjectScript>().RPCHurt(Target.transform.parent.GetComponent<PhotonView>().viewID, (int)(baseDamage*1f), true);
						}
						if (collisionResult > 0) {
							Target.transform.GetComponent<ElementalObjectScript>().RPCHurt(Target.transform.parent.GetComponent<PhotonView>().viewID, (int)(baseDamage*2f), true);
						}
					}
				} else {
					//Debug.Log("Element Comparer Result: "+collisionResult);
					if(Ishot){
						if(collisionResult < 0){
							Target.transform.GetComponent<ElementalObjectScript>().RPCHurt(Target.transform.GetComponent<PhotonView>().viewID, (int)(baseDamage*.5f), false);
						}
						if (collisionResult == 0) {
							Target.transform.GetComponent<ElementalObjectScript>().RPCHurt(Target.transform.GetComponent<PhotonView>().viewID, ((int)(baseDamage*1f)), false);
						}
						if (collisionResult > 0) {
							Target.transform.GetComponent<ElementalObjectScript>().RPCHurt(Target.transform.GetComponent<PhotonView>().viewID, ((int)(baseDamage*2f)), false);
						}
					}
					if (debuffValue == 1) {
						Target.transform.GetComponent<ElementalObjectScript>().changeMoveSpeed(-0.25f);
						Target.transform.GetComponent<ElementalObjectScript>().decreaseDefense(0.25f);
						Target.transform.GetComponent<PlayerControler>().speedDebuffed = true;
						Target.transform.GetComponent<PlayerControler>().timeSinceDebuffed = 10.0f;
						OnScreenDisplayManager.PostMessage("You were hit by Trim! Your speed and defense have been debuffed!");
					}
				}
			}
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
