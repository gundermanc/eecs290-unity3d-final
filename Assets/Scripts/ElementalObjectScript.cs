using UnityEngine;
using System.Collections;

public enum Element {Rock, Paper, Scissors};

public class ElementalObjectScript : MonoBehaviour {

	public int Health;
	public Element thisType;

	public void Hurt(int amount){
		Debug.Log("hurt this amount: "+amount);
		Health -= amount;
	}

	public Element getElementalType(){
		return thisType;
	}
}
