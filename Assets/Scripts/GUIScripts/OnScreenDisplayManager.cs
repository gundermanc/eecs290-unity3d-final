using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

/**
 * Handles the rendering of all GUI elements, HUD elements, pause screen, menus,
 * etc.
 * @author Christian Gunderman
 */
public class OnScreenDisplayManager : MonoBehaviour {
	
	// script parameters
	public Texture2D reticle;                   //the texture frile for use for the reticle

	public Texture healthBarTexture;			// the texture file to use for the health bar
	public Texture healthBarBackgroundTexture;	// the texture for the background of the empty health bar
	private Rect healthBarRect;					// the rectangle which the FULL health bar will occupy
	private Rect healthBarBackgroundRect;		// the rectangle which the empty health bar will occupy
	
	public Texture fatigueBarTexture;				// the texture file to use for the ammo bar
	public Texture fatigueBarBackgroundTexture;	// the texture for the background of the empty ammo bar
	private Rect fatigueBarRect;					// the rectangle which the FULL ammo bar will occupy
	private Rect fatigueBarBackgroundRect;			// the rectangle which the empty ammo bar will occupy

	public int messageDisplayTime = 5;			// the number of seconds to display a message
	public Texture bloodDecalsTexture;			// the texture for the game over blood graphics
	public Texture logoTexture;
	public Texture overlayTexture;
	private const string gameTitle = "Rock, Paper, Scissors"; //text for the game title
	
	// private constants
	private const int pauseMenuMargins = 70; //Coordinates for the rectangle used in the pause menu
	private const int shadowOffset = -2; //Value used to draw a shadow rectangle
	private const string soundSettingStringFalse = "<size=30>Toggle Sound (Off)</size>";
	private const string soundSettingStringTrue = "<size=30>Toggle Sound (On)</size>";
	private const string instructionsText = "Rock beats Scissors beats Paper beats Rock.Become a fighter with the elements and fight a 3-on-3"
				+ " battle against your enemies. Your goal: take down the opposing team's towers before they take yours down.\n\n"
				+ "Arrow Keys, WASD - Movement\nMouse - Rotate Camera\nLeft-Click - Default Attack\nLeft-Click + Hold Q - Special Attack 1"
				+ "Left-Click + Hold E - Special Attack 2\nLeft-Click + Hold Q and E - Ultimate Special Attack\nHold Shift - Sprint"
				+ "Space - Jump\nEscape - Pause\n\n";

	// private fields
	private int healthPoints = 100; //Player character's health points
	private int ammoCount = 100; //Default ammo count for throwing knives
	private int maxAmmoCount = 100; //Maximum amount of ammo the player can have at one time
	private LinkedList<Message> messageQueue; //A linked list containing all the queue'd messages to be displayed on screen
	private PeerState lastState = PeerState.Disconnected;
	private bool soundOn = false; //Sound is set to off by default

	private string teamMessage = "";

	// singleton instance reference
	private static OnScreenDisplayManager instance; 
	
	/**
	 * Run at initialization
	 */
	void Start () {
		/* Initializes the various bars to help you keep track of HP, Fatigue
		 * The "x" and "y" refer to the top-left co-ordinates of any rectangle
		 * The "width" and "height" refer to the actual size of the rectangle
		 * Each bar has both a rectangle for the actual meter as well as one for the background behind the bar
		 */
		
		healthBarRect.x = 17;
		healthBarRect.y = 20;
		healthBarRect.width = 155; 
		healthBarRect.height = 25;
		healthBarBackgroundRect.x = 15;
		healthBarBackgroundRect.y = 15;
		healthBarBackgroundRect.width = 160;
		healthBarBackgroundRect.height = 35;
		
		fatigueBarRect.x = 17;
		fatigueBarRect.y = 65;
		fatigueBarRect.width = 155; 
		fatigueBarRect.height = 25;
		fatigueBarBackgroundRect.x = 15;
		fatigueBarBackgroundRect.y = 60;
		fatigueBarBackgroundRect.width = 160;
		fatigueBarBackgroundRect.height = 35;

		// class is a singleton. save a static reference
		instance = this;
		messageQueue = new LinkedList<Message> ();
	}
	
	/**
	 * Draws the GUI elements pertaining to the current game state.
	 */
	void OnGUI () {
		
		// draw the GUI appropriate for the current game state.
		switch(GameManager.GetGameMode()) {
		case GameManager.GameMode.Paused: //If the game mode is paused
			DrawPauseMenu();			
			DrawPlayersList ();			
			break;
		case GameManager.GameMode.Instructions:
			DrawInstructionsScreen ();
			break;
		case GameManager.GameMode.StartMenu:
			DrawGameStart();	  //Draw the start menu
			break;
		case GameManager.GameMode.GameSetup:
			DrawGameSetup ();
			break;
		case GameManager.GameMode.UnPaused: //Draw the usual display for when the game is unpaused and is being played
			DrawHUD();
			break;
		case GameManager.GameMode.EndLevel: //Draw the screen for when a level is won (the end of the maze is reached)
			DrawWinLevelScreen();
			break;
		case GameManager.GameMode.EndGame: //Draw the screen that congratulates the player for when all levels have been completed
			Debug.Log ("No end game implemented.");
			break;
		}
	}
	
	void Update() {
		
		// don't discard any messages while the game is paused
		if (GameManager.IsPaused ()) {
			return;
		}

		UpdateConnectionState ();
		//Otherwise, slowly remove messages from message log
		DiscardMessages ();
	}
	
	/**
	 * Sets the health points values in the onscreen display. If the player dies
	 * because of this health change, the deathCallback function is called.
	 * It also ensures the health does not go above the max health (100).
	 * @param healthPoints A value between 0 and 100
	 * @param addToExistingValue If true, the function adds healthPoints to
	 * the existing health points. If false, the old value is thrown out and
	 * replaced with the new value.
	 */
	public static void SetHealthPoints(int healthPoints, bool addToExistingValue) {
		if (healthPoints < 0) {
			Debug.LogError ("Invalid health points value: " + healthPoints);
			return;
		}
		
		
		// set value
		if (addToExistingValue) {
			if (healthPoints > 100) {
				instance.healthPoints = 100;
			} else {
				instance.healthPoints += healthPoints;
			}
		} else {
			if (healthPoints > 100) {
				instance.healthPoints = 100;
			} else {
				instance.healthPoints = healthPoints;
			}
		}
	}
	
	/**
	 * Sets the ammo count values in the onscreen display.
	 * @param ammo A value between 0 and 100
	 * @param addToExistingValue If true, the function adds healthPoints to
	 * the existing health points. If false, the old value is thrown out and
	 * replaced with the new value.
	 */
	public static void SetFatigue(int ammo) {
		if(ammo < 0 || ammo > instance.maxAmmoCount) {
			Debug.LogError("Invalid OSD ammo values.");
		}
		instance.ammoCount = ammo;
	}
	
	/** 
	 * Sets the max ammo count for the OnScreenDisplay
	 * @param maxAmmoCount The new maximum amount of ammo the player can have
	 */
	public static void SetMaxFatigue(int maxAmmoCount) {
		instance.maxAmmoCount = maxAmmoCount;
	}
	
	/**
	 * Post a message given only the message to post: simply uses default color (white)
	 * @param message the message to add to the messageQueue, which will be converted to a string
	 */
	public static void PostMessage(object message) {
		PostMessage (message, new Color (1.0f, 1.0f, 1.0f, 1.0f));
	}
	
	/**
	 * Post a message given both the message to post and a color for the message to be in
	 * @param message the message to add to the messageQueue, which will be converted to a string
	 * @param color the value of the color the text should use
	 */
	public static void PostMessage(object message, Color color) {
		instance.messageQueue.AddLast(new Message(DateTime.Now, message.ToString(),
		                                          color));
	}

	/**
	 * Draws the Instructions screen stuff
	 */
	private void DrawInstructionsScreen () {
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Box");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		
		Rect menuRect = new Rect (100, 100, Screen.width - 200, Screen.height - 200);


		// draw overlay
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), overlayTexture);

		// draw title
		DrawLabelWithShadow (menuRect, "<size=40>Instructions</size>");

		menuRect.y += 90;
		menuRect.height -= 100;
		centeredStyle = GUI.skin.GetStyle ("Box");
		centeredStyle.alignment = TextAnchor.UpperLeft;

		GUI.TextArea (menuRect, instructionsText);

		menuRect.y += menuRect.height + 20;
		menuRect.height = 50;
		if (GUI.Button (menuRect, "<size=30>Return</size>")) {
			GameManager.StartScreen ();
		}
	}

	/**
	 * Draws the game setup screen
	 */
	private void DrawGameSetup () {
		
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Box");
		centeredStyle.alignment = TextAnchor.MiddleCenter;

		Rect menuRect = new Rect (100, 100, Screen.width - 200, Screen.height - 200);

		// draw overlay
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), overlayTexture);

		// draw title
		DrawLabelWithShadow (menuRect, "<size=40>Multiplayer Lobby</size>");

		String userName = "";
		// draw username field
		Rect usernameRect = new Rect (menuRect);
		usernameRect.y += 90;
		usernameRect.height = 50;
		centeredStyle = GUI.skin.GetStyle ("Box");
		centeredStyle.alignment = TextAnchor.UpperLeft;
		GUI.Label (usernameRect, "<size=20>Screen name</size>");
		usernameRect.y += 50;

		GUIStyle textFieldStyle = GUI.skin.GetStyle ("TextField");
		textFieldStyle.fontSize = 30;
		PhotonNetwork.player.name = GUI.TextField (usernameRect, PhotonNetwork.player.name);

		Rect startRect = new Rect (usernameRect);
		startRect.y = Screen.height - 300;
		startRect.height = 100;
		if(GUI.Button (startRect, "<size=30>Start Game</size>")) {
			if(!PhotonNetwork.player.name.Equals("")) {
				GameManager.StartGame ();
			}
		}
	}
	
	/**
	 * Draw the game Heads up display.
	 */
	private void DrawHUD() {
		GUIStyle regularStyle = GUI.skin.GetStyle ("Label");
		regularStyle.alignment = TextAnchor.UpperLeft;

		//draw the reticle
		float xcorner = (Screen.width / 2) - 40;
		float ycorner = (Screen.height / 2) - 40;
		GUI.DrawTexture (new Rect(xcorner, ycorner, 80, 80), reticle);
		
		DrawHealthBar ();
		DrawFatigueBar ();
		DrawMessages ();
	}

	/**
	 * Draws the list of currently connected players
	 */
	private void DrawPlayersList () {

		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Box");
		centeredStyle.alignment = TextAnchor.MiddleCenter;

		centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;

		bool isBox = true;
		float width = ((Screen.width - (2 * pauseMenuMargins)) / 2)
						- pauseMenuMargins;
		Rect drawRect = new Rect (pauseMenuMargins + pauseMenuMargins + width, pauseMenuMargins, width, 50);

		
		DrawLabelWithShadow (drawRect, "<size=40><i>Players List</i></size>");
		
		// restart game button
		drawRect.y += 100;
		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			string team;
			if(((int)player.allProperties["Team Number"]) == 0) {
				team = ", <color=#FF0000>TEAM RED</color>";
			} else {
				team = ", <color=#0000FF>TEAM BLUE</color>";
			}

			if(isBox) {
				GUI.Box (drawRect, "<size=40>" + player.name + team + "</size>");
			} else {
				DrawLabelWithShadow (drawRect, "<size=40>" + player.name + team + "</size>");
			}
			drawRect.y += 50;
			isBox = !isBox;
		}
	}
	
	/**
	 * Wraps GUI.Label function to provide shadows.
	 * @param labelPos is the position of the label which is offset by shadowOffset
	 * @param label is the actual string we are drawing the shadow for
	 */
	private void DrawLabelWithShadow(Rect labelPos, string label) {
		
		// translate rect for shadow
		Rect offsetLabelPos = new Rect (labelPos);
		labelPos.x += shadowOffset;
		labelPos.y += shadowOffset;
		labelPos.xMax += shadowOffset;
		labelPos.yMax += shadowOffset;
		
		// draw shadow
		GUI.Label (offsetLabelPos, "<color=black>" + label + "</color>");
		
		// draw text
		GUI.Label (labelPos, label);
	}
	
	/**
	 * Draws the pause menu	labels and GUI.
	 */
	private void DrawPauseMenu() {
		
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;

		// draw overlay
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), overlayTexture);
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  ((Screen.width - (2 * pauseMenuMargins)) / 2) - pauseMenuMargins,
		                                  (Screen.height- (2 * pauseMenuMargins)));
		
		DrawLabelWithShadow (screenDimensions, "<size=40><i>Paused</i></size>");
		
		// restart game button
		screenDimensions.y += 100;
		screenDimensions.yMax = screenDimensions.y + 75;
		/*if (GUI.Button (screenDimensions, "<size=30>Restart</size>")) {
			GameManager.RestartGame();
		}
		
		// exit game button
		screenDimensions.y += 100;
		screenDimensions.yMax = screenDimensions.y + 75;*/
		if (GUI.Button (screenDimensions, "<size=30>Quit</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.Quit ();
		}

		// team message box
		screenDimensions.y += 100;
		DrawLabelWithShadow (screenDimensions, "<size=30>Team Message</size>");
		screenDimensions.y += 50;
		screenDimensions.height = 200;
		GUIStyle textFieldStyle = GUI.skin.GetStyle ("TextArea");
		textFieldStyle.fontSize = 22;
		this.teamMessage = GUI.TextArea (screenDimensions, this.teamMessage);
		screenDimensions.y += 250;
		screenDimensions.height = 50;

		if (GUI.Button (screenDimensions, "<size=30>Send Message</size>")) {
			GameObject.Find("GameManager").GetComponent<GameManager>()
				.TeamChat("Message from team: " + this.teamMessage);
			this.teamMessage = "";
			GameManager.UnPause ();
		}
	}
	
	/**
	 * Draws the start menu GUI.
	 * @param message is the message displayed on the start screen.
	 */
	private void DrawGameStart() {
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));
		
		screenDimensions.y += 100;
		
		Rect logoRect = new Rect ();
		logoRect.width = 807;
		logoRect.height = 278;
		logoRect.x = (Screen.width - logoRect.width) / 2;
		logoRect.y = (screenDimensions.y);
		
		GUI.DrawTexture (new Rect(0, 0, Screen.width, Screen.height), overlayTexture);
		GUI.DrawTexture (logoRect, logoTexture);
		
		screenDimensions.y += logoRect.height;
		screenDimensions.yMax = screenDimensions.y + 75;
		
		//Start game button
		if (GUI.Button (screenDimensions, "<size=30>Multiplayer Lobby</size>")) {
			GameManager.SetupGame ();
		}
		
		float buttonWidth = (screenDimensions.width - 40) / 3;
		screenDimensions.y += 85;
		screenDimensions.width = buttonWidth;
		//Instructions game button
		if (GUI.Button (screenDimensions, "<size=30>Instructions</size>")) {
			GameManager.InstructionsScreen ();
		}
		screenDimensions.x += buttonWidth + 20;
		//Toggle sound game button, toggles sound on/off: on by default
		if (GUI.Button (screenDimensions, soundSettingStringFalse)) {
			if (soundOn == false) {
				soundOn = true;
			} else {
				soundOn = false;
			}
		}
		screenDimensions.x += buttonWidth + 20;
		//Exit game button, only works on build
		if (GUI.Button (screenDimensions, "<size=30>Exit Game</size>")) {
			Application.Quit();
		}
	}

	private void UpdateConnectionState () {
		if(PhotonNetwork.connectionStateDetailed != lastState) {
			OnScreenDisplayManager.PostMessage("Network: " + PhotonNetwork.connectionStateDetailed.ToString(), Color.yellow);
		}
		lastState = PhotonNetwork.connectionStateDetailed;
	}
	
	/**
	 * Draws the GUI for winning a level.
	 */
	private void DrawWinLevelScreen() {
		
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));

		DrawLabelWithShadow (screenDimensions, "<size=40><i>" +  "Completed Level: </i></size>");
		
		
		
		// restart game button
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Next Level</size>")) {
			
			Debug.Log("Not Implemented Yet.");
		}
		
		// quit game button
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Quit Game</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.Quit();
		}
	}
	
	/**
	 * Draws the screen shown upon beating all levels or dying.
	 * @param message The message shown on the win screen (e.g. a congratulations message).
	 */
	private void DrawWinScreen(string message) {
		
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));
		
		DrawLabelWithShadow (screenDimensions, "<size=40><i>" +  message
		                     + "</i></size>");
		
		
		
		// restart game button
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Restart Game</size>")) {
			
			GameManager.RestartGame();
		}
		
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Quit Game</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.Quit();
		}
	}
	
	/**
	 * Draws the health bar, including the actual value of the health on the 
	 * health bar, and the health bar background 
	 */
	private void DrawHealthBar() {
		
		// draw background
		GUI.DrawTexture (healthBarBackgroundRect, healthBarBackgroundTexture);
		
		// draw health bar
		Rect scaledHealthBarRect = new Rect (healthBarRect.x, healthBarRect.y, 
		                                     (int)(((float)healthPoints / 100) * healthBarRect.width),
		                                     healthBarRect.height);
		GUI.DrawTexture (scaledHealthBarRect, healthBarTexture);
		
		// draw text
		GUI.color = Color.black;
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label (healthBarRect, "<b>" + healthPoints + "HP</b>");
		GUI.color = Color.white;
	}
	
	/**
	 * Draws the ammo bar for the HUD
	 */
	private void DrawFatigueBar() {
		
		// draw background
		GUI.DrawTexture (fatigueBarBackgroundRect, fatigueBarBackgroundTexture);
		
		// draw battery bar
		Rect scaledAmmoBarRect = new Rect (fatigueBarRect.x, fatigueBarRect.y, 
		                                   (int)(((float)ammoCount / maxAmmoCount) * fatigueBarRect.width),
		                                   fatigueBarRect.height);
		GUI.DrawTexture (scaledAmmoBarRect, fatigueBarTexture);
		
		// draw text
		GUI.color = Color.black;
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label (fatigueBarRect, "<b>" + ammoCount + " Stamina</b>");
		GUI.color = Color.white;
	}
	
	/**
	 * Draws the messages posted with PostMessage to the screen.
	 */
	private void DrawMessages() {
		float y = 150;
		
		GUIStyle upperLeft = GUI.skin.GetStyle ("Label");
		upperLeft.alignment = TextAnchor.UpperLeft;
		foreach(Message m in messageQueue) {
			GUI.color = m.color;
			GUI.Label (new Rect (10, y += 20, 1000, 1000),
			           "<size=15>" + m.message + "</size>");
		}
	}
	
	/**
	 * Iterates through the list of messages posted with PostMessage.
	 * For each message, the message state is updated. If it has been
	 * in the queue for messageDisplayTime, the system begins reducing
	 * the alpha for the current message until it disappears and is removed
	 * from the queue.
	 */
	private void DiscardMessages() {
		if (messageQueue.Count > 0) {
			/*			Message message = messageQueue.First.Value;*/
			
			foreach(Message message in messageQueue) {
				// see if a message has been up a long time, if so, delete it
				if ((DateTime.Now.Subtract (message.submitTime)).TotalSeconds > messageDisplayTime) {
					message.color.a -= 0.03f;
					if(message.color.a <= 0.0f) {
						messageQueue.Remove(message);
						break;
					}
				}
			}
		}
	}
	
	/**
	 * Defines a message that can be posted to the screen using PostMessage().
	 */
	private class Message {
		/** The time the message was submitted. */
		public DateTime submitTime;
		/** The message to display onscreen. */
		public string message;
		/** The color of the text */
		public Color color;
		
		/**
		 * Constructs an instance.
		 * @param submitTime The time the message was submitted.
		 * @param message The message text.
		 * @param color The color to draw the text.
		 */
		public Message(DateTime submitTime, string message, Color color) {
			this.submitTime = submitTime;
			this.message = message;
			this.color = color;
		}
	}
}