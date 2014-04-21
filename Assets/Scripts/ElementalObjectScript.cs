using UnityEngine;
using System.Collections;

public enum Element {Rock, Paper, Scissors};

public class ElementalObjectScript : MonoBehaviour {

	public int Health;
	public Element thisType;

	public void Hurt(int amount){
		Debug.Log("hurt this amount: "+amount);
		Health -= amount;
		if(Health <= 0){
			if(transform.tag == "Player"){

			} else {
				Debug.Log("I DIED!");
				transform.parent.GetComponent<TowerScript>().Death();
			}
		}
	}

	public Element getElementalType(){
		return thisType;
	}
}
