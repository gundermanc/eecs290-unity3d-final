using UnityEngine;
using System.Collections;

public enum Element {Rock, Paper, Scissors};

public class ElementalObjectScript : MonoBehaviour {
	
	public int Health = 100;
	public float attackSpeed = 1.0f;
	public float defense = 1.0f;
	public float moveSpeed = 1.0f;
	public Element thisType;
	public int teamNumber;
	private bool dead = false;
	
	//Decreases the health of the object
	[RPC]
	public void Hurt(int ID, int amount){
		if (gameObject.GetComponent<PhotonView> ().viewID == ID) {
			int damageDealt = Mathf.CeilToInt(amount / defense);
			Debug.Log("Damage Dealt: "+damageDealt);
			Health -= damageDealt;
			if(Health <= 0 && !dead) {
				if(transform.tag == "Player"){
					Health = 100;
					gameObject.transform.GetComponent<PlayerControler>().Kill();
				} else {
					Debug.Log("Death flag.");
					transform.parent.GetComponent<PhotonView>().RPC("Death", PhotonTargets.All, transform.parent.GetComponent<PhotonView> ().viewID);
					dead = true;
				}
			}
		}
	}
	
	public void decreaseAttackSpeed(float amount) {
		this.attackSpeed -= amount;
	}
	
	public void resetAttackSpeed() {
		this.attackSpeed = 1.0f;
	}
	
	public void decreaseDefense(float amount) {
		this.defense -= amount;
	}
	
	public void resetDefense() {
		this.defense = 1.0f;
	}
	
	public void changeMoveSpeed(float amount) {
		this.moveSpeed += amount;
		CharacterMotor script = this.gameObject.GetComponent<CharacterMotor>();
		script.movement.maxForwardSpeed = script.movement.maxForwardSpeed*moveSpeed;
		script.movement.maxSidewaysSpeed = script.movement.maxSidewaysSpeed*moveSpeed;
		script.movement.maxBackwardsSpeed = script.movement.maxBackwardsSpeed*moveSpeed;
	}
	
	public void resetMoveSpeed() {
		CharacterMotor script = this.gameObject.GetComponent<CharacterMotor>();
		script.movement.maxForwardSpeed = script.movement.maxForwardSpeed/moveSpeed;
		script.movement.maxSidewaysSpeed = script.movement.maxSidewaysSpeed/moveSpeed;
		script.movement.maxBackwardsSpeed = script.movement.maxBackwardsSpeed/moveSpeed;
		this.moveSpeed = 1.0f;
	}
	
	public Element getElementalType(){
		return thisType;
	}

	public void Update () {
		 //update Chris's OnScreenDisplayManager.
		OnScreenDisplayManager.SetHealthPoints (Health, false);
	}
}
