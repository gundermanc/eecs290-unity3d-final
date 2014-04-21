using UnityEngine;
using System.Collections;

public enum Element {Rock, Paper, Scissors};

public class ElementalObjectScript : MonoBehaviour {
	
	public int Health = 100;
	public float attackSpeed = 1.0f;
	public float defense = 1.0f;
	public float moveSpeed = 1.0f;
	public Element thisType;
	
	//Decreases the health of the object
	public void Hurt(int amount){
		int damageDealt = Mathf.FloorToInt(amount / defense);
		Debug.Log("Hurt this amount: "+damageDealt);
		if (Health - damageDealt > 0) {
			Health -= damageDealt;
		} else {
			if(transform.tag == "Player"){
				Destroy(this.gameObject);
			} else {
				Debug.Log("I DIED!");
				transform.parent.GetComponent<TowerScript>().Death();
			}
		}
	}
		
		public void decreaseAttackSpeed(int amount) {
			this.attackSpeed -= amount;
		}
		
		public void resetAttackSpeed() {
			this.attackSpeed = 1.0f;
		}
		
		public void decreaseDefense(int amount) {
			this.defense -= amount;
		}
		
		public void resetDefense() {
			this.defense = 1.0f;
		}
		
		public void decreaseMoveSpeed(int amount) {
			this.moveSpeed -= amount;
		}
		
		public void resetMoveSpeed(int amount) {
			this.moveSpeed = 1.0f;
		}
		
		public Element getElementalType(){
			return thisType;
		}
	}
