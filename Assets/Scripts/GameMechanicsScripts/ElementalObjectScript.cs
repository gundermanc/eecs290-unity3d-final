﻿using UnityEngine;
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

	public void RPCHurt(int ID, int amount, bool parent){
		int damageDealt = Mathf.CeilToInt(amount / defense);
		Health -= damageDealt;
		if (parent)
			transform.parent.GetComponent<PhotonView>().RPC("Hurt", PhotonTargets.All, ID, Health);
		else
			transform.GetComponent<PhotonView>().RPC("Hurt", PhotonTargets.All, ID, Health);
	}
	
	//Decreases the health of the object
	[RPC]
	public void Hurt(int ID, int newHealth){
		if (gameObject.GetComponent<PhotonView> ().viewID == ID) {
			Health = newHealth;
			if(Health <= 0 && !dead) {
				if(transform.tag == "Player"){
					Health = 100;
					gameObject.transform.GetComponent<PlayerControler>().Kill();
				} else {
					Debug.Log("Death flag.");
					transform.GetComponent<PhotonView>().RPC("Death", PhotonTargets.All, transform.GetComponent<PhotonView> ().viewID);
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
		if (gameObject.tag == "Player") {
			if (gameObject.transform.GetComponent<PhotonView> ().isMine)
				OnScreenDisplayManager.SetHealthPoints (Health, false);
		}
	}
}
