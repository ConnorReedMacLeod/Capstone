using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will generally contain everything in a match
// responsible for creating and managing the game

public class Match : MonoBehaviour {
	
	public Arena arena;

	public int nPlayers = 2;
	public Player [] arPlayers;

	public Chr [][] arChrs;

	public Timeline timeline;

	public Controller controller;

	public GameObject pfPlayer;
	public GameObject pfChr;

	public static Match instance;


	public static Match Get (){
		if (instance == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Match");
			if (go == null) {
				Debug.LogError ("ERROR! NO OBJECT HAS A MATCH TAG!");
			}
			instance = go.GetComponent<Match> ();
			if (instance == null) {
				Debug.LogError ("ERROR! MATCH TAGGED OBJECT DOES NOT HAVE A MATCH COMPONENT!");
			}
		}
		return instance;
	}

	public Player GetLocalPlayer(){
		return arPlayers [0];
	}

	void InitPlayers (int _nPlayers){
		nPlayers = _nPlayers;//in case this needs to be changed based on the match
		arPlayers = new Player[nPlayers];
		arChrs = new Chr[nPlayers][];

		for (int i = 0; i < _nPlayers; i++) {
			GameObject goPlayer = Instantiate (pfPlayer, this.transform);
			Player newPlayer = goPlayer.GetComponent<Player> ();// TODO:: Replace this with Network spawning somehow
			if (newPlayer == null) {
				Debug.LogError ("ERROR! NO PLAYER COMPONENT ON PLAYER PREFAB!");
			}

			newPlayer.SetID (i);
			newPlayer.Start ();
			newPlayer.setChrs (); /// TODO:: replace with actual character selection
			arPlayers [i] = newPlayer;
		}
	}

	// Will eventually need a clean solution to adding/removing characters
	// while managing ids - some sort of Buffer of unused views will probably help
	void InitChr (Chr.CHARTYPE type, Player player, int id){

		GameObject goChr = Instantiate (pfChr, this.transform);
		Chr newChr = goChr.GetComponent<Chr>();
		if (newChr == null) {
			Debug.LogError ("ERROR! NO CHR COMPONENT ON CHR PREFAB!");
		}

		newChr.Start ();

		switch (type) {
		case Chr.CHARTYPE.KATARA: 
			newChr.InitChr(player, id, "Katara");
			break;
		case Chr.CHARTYPE.LANCER:
			newChr.InitChr(player, id, "Lancer");
			break;
		case Chr.CHARTYPE.SKELCOWBOY:
			newChr.InitChr(player, id, "SkelCowboy");
			break;
		default: 
			Debug.LogError ("INVALID CHARACTER SELECTION");
			Application.Quit ();
			newChr.InitChr (player, id, "ERROR"); //so the editor will let us compile
			break;
		}
		arChrs [player.id] [id] = newChr;

	 
		//newChr.SetPosition(Random.Range(-10.0f, 10.0f), arena.arfStartingPosY[id]);
		arena.InitPlaceUnit(newChr);

	}

	void InitAllChrs(){
		for (int i = 0; i < nPlayers; i++) {
			arChrs [i] = new Chr[Player.MAXCHRS];

			for (int j = 0; j < arPlayers [i].nChrs; j++) {
				InitChr (arPlayers [i].arChrTypeSelection [j], arPlayers[i], j);
			}
		}
	}

	public void NextTimelineEvent(){
		timeline.EventFinished ();
	}

	public Controller GetController(){
		if (controller == null) {
			controller = GetComponentInChildren<Controller> ();
			if (controller == null) {
				Debug.LogError ("ERROR! NO CONTROLLER FOUND IN CHILDREN OF MAIN MATCH OBJECT");
			}
		}
		return controller;
	}


	public void Start(){
		gameObject.tag = "Match"; // So that anything can find this very quickly

		arena = GetComponentInChildren<Arena> ();
		timeline = GetComponentInChildren<Timeline> ();
		controller = GetComponentInChildren<Controller> ();

		arena.Start ();

		timeline.Start ();

		InitPlayers (nPlayers);

		InitAllChrs ();

		timeline.InitTimeline ();
	
		/*FOR TESTING -> REMOVE THIS AT SOME POINT*/
		//Invoke("NextTimelineEvent", 5);
		/**/

	}
}
