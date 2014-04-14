using UnityEngine;
using System.Collections;

public enum Element {Rock, Paper, Scissors};

public class ElementalObjectScript : MonoBehaviour {

	public int Health;
	public Element thisType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Hurt(int amount){
		Health -= amount;
	}

	Element getType(){
		return thisType;
	}
}
