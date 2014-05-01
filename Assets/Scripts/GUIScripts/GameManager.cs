using UnityEngine;
using System.Collections;

/**
 * Handles game level loading, StartGame, EndGame, Pause, and GameModes
 * @author Christian Gunderman
 */
public class GameManager : MonoBehaviour {

	public bool[,] teamTowersDead = new bool[2,3];
	
	/** Saves a static reference to instance ... sloppy I know, I don't care */
	private static GameManager instance;
	/** Stores the current GameMode (pause, play, dead, menu etc. */
	private static GameMode mode;
	//Time when the game ends
	private static float gameEnd;
	//True once the game begins
	public static bool started;
	
	/**
	 * Script initialization. This is called by unity on object creation.
	 */
	void Start () {
		instance = this;
		Pause ();
		mode = GameMode.StartMenu;
		gameEnd = 0;
		started = false;

		// initiallizes all towers for each team as not dead
		for(int i = 0; i < teamTowersDead.GetLength(0); i++){
			for(int j = 0; j < teamTowersDead.GetLength(1); j++){
				teamTowersDead[i, j] = false;
			}
		}
	}
	
	/**
	 * Called once per frame by unity. Handles toggling of paused and play.
	 */
	void Update () {
		// guard case, exit function if this isn't a pause/unpause situation
		if (!Input.GetKeyDown ("escape")) {
			return;
		}

		//TODO
		//10 seconds after the game is done, the level is reloaded
		if (Time.timeSinceLevelLoad - gameEnd > 10 && gameEnd != 0) {
			gameEnd = 0;
			//PhotonNetwork.LoadLevel(0);
			// killing the players works when you pause
			foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
				player.GetComponent<PlayerControler>().Kill();
			}

			// This gets all the tower scripts so the towers get reset
			foreach (TowerScript tower in GameObject.FindObjectsOfType<TowerScript>()) {
				// i dont know why this is returning null
				tower.TowerReset();
			}
		}
		
		// toggle game paused state
		switch(GameManager.GetGameMode()) {
		case GameMode.Paused:
			UnPause();
			break;
		case GameMode.UnPaused:
			Pause();
			break;
		}
	}

	/**
	 * Called when a tower has died
	 * flags that the tower from that team has died
	 * checks all towers for that team, if one is alive the method returns
	 * if all towers are dead, the loop is exited and the level is ended
	 * @param the team the tower was from
	 * @param the Element type the tower was
	 */
	public void TowerDied(int team, int towerType){
		//Sends team messages, if statement prevents duplicate messages
		if (!teamTowersDead [team, towerType]) {
			//Send sympathetic message to team whose tower just got destroyed
			TeamMessage (team, "Oh no! One of your towers has been destroyed!", Color.red);
			//Sends congratulatory message to the team that just destroyed a tower
			TeamMessage ((team + 1) % 2, "Your team just destroyed one of your enemies towers!", Color.green);
		}
		teamTowersDead[team, towerType] = true;
		// checks all tower flags for the team
		for(int i = 0; i < teamTowersDead.GetLength(team); i++){
			// if one tower is alive, the method returns
			if(!teamTowersDead[team, i]){
				return;
			}
		}
		// if the method reaches this point then the teams towers have been destroyed
		EndLevel((team+1)%1);
	}

	public void TowerReset(int team, int towerType){
		teamTowersDead[team, towerType] = false;
	}
	
	/**
	 * Checks if the game is paused.
	 * @return Returns true if paused, false if not.
	 */
	public static bool IsPaused() {
		return mode != GameMode.UnPaused;
	}
	
	/**
	 * Gets the GameMode of the current game. See GameMode enum definition
	 * for possible states.
	 * @return The current game mode.
	 */
	public static GameMode GetGameMode() {
		return mode;
	}

	/**
	 * Brings up game setup screen
	 */
	public static void SetupGame () {
		mode = GameMode.GameSetup;
	}

	/**
	 * Brings up instructions screen
	 */
	public static void InstructionsScreen() {
		mode = GameMode.Instructions;
	}

	/**
	 * Brings up start screen
	 */
	public static void StartScreen() {
		mode = GameMode.StartMenu;
	}
	
	/**
	 * Pauses the game.
	 */
	public static void Pause() {
		mode = GameMode.Paused;
		GameObject thisPlayer = NetworkingScript.GetThisPlayer ();
		if(thisPlayer != null) {
			thisPlayer.GetComponent<MouseLook> ().enabled = false;
			foreach(FPSInputController obj in thisPlayer.GetComponentsInChildren<FPSInputController> ()) {
				obj.enabled = false;
			}
			//thisPlayer.GetComponent<PlayerControler> ().enabled = false;
		}

		// show mouse
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}
	
	/**
	 * Unpauses the game.
	 */
	public static void UnPause() {
		mode = GameMode.UnPaused;
		GameObject thisPlayer = NetworkingScript.GetThisPlayer ();
		if(thisPlayer != null) {
			thisPlayer.GetComponent<MouseLook> ().enabled = true;
			foreach(FPSInputController obj in thisPlayer.GetComponentsInChildren<FPSInputController> ()) {
				obj.enabled = true;
			}
			thisPlayer.GetComponent<PlayerControler> ().enabled = true;
			thisPlayer.GetComponent<ElementalObjectScript> ().resetMoveSpeed ();
		}

		// hide mouse
		Screen.lockCursor = true;
		Screen.showCursor = false;
	}
	
	/**
	 * Starts the execution of the game.
	 */
	public static void StartGame() {
		// register handler for game over event
		//OnScreenDisplay.RegisterDeathCallback (new OnDeathHandler ());
		PhotonNetwork.ConnectUsingSettings("alpha 0.1");
		UnPause ();
	}
	
	/**
	 * Restarts the game at the first level with full health, battery, and ammo.
	 */
	public static void RestartGame() {
		Debug.LogError ("Game restart not supported.");
	}
	
	public void TeamChat(string message){
		int team = 0;
		string name = "Anonymous";
		foreach (GameObject p in GameObject.FindGameObjectsWithTag ("Player")) {
			if (p.GetComponent<PhotonView>().isMine){
				team = p.GetComponent<PlayerControler>().teamNumber;
				name = p.GetComponent<PhotonView>().owner.name;
			}
		}
		foreach(GameObject p in GameObject.FindGameObjectsWithTag ("Player")){
			p.GetComponent<PlayerControler>().SendTeamMessage(team, "Message from " + name + ": " +message);
		}
	}
	
	public static void TeamMessage(int team, string message, Color c){
		foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) {
			if (p.GetComponent<PhotonView>().isMine){
				if (p.GetComponent<PlayerControler>().teamNumber == team){
					OnScreenDisplayManager.PostMessage(message, c);
				}
			}	
		}
	}	

	/**
	 * Called when player reaches the end of a level. Dispatches EndLevel GUIs
	 * and loads next level.
	 * @param team - winning team
	 */
	public static void EndLevel(int team) {
		//Sends sympathetic message to losing team
		TeamMessage ((team+1)%2, "All of your towers are down! Better luck next time!", Color.red);
		//Sends congratulatory message to the winning team
		TeamMessage (team, "YOU WIN!!!", Color.green);

		TeamMessage ((team+1)%2, "Game is restarting in 10 seconds.", Color.green);
		TeamMessage (team, "Game is restarting in 10 seconds.", Color.green);
		//Sets time when the game was one
		gameEnd = Time.timeSinceLevelLoad;
	}
	
	/**
	 * An enum of possible game states.
	 */
	public enum GameMode {
		StartMenu,			// at the start screen
		GameSetup,			// setting up your game to play
		Instructions,		// instructions screen
		Paused,				// at the pause screen
		UnPaused,			// in normal game play
		EndLevel,			// at the end level screen
		EndGame				// at the end game screen
	}
}