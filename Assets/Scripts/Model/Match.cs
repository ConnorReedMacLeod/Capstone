using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Will generally contain everything in a match
// responsible for creating and managing the game

public class Match : NetworkBehaviour {

    bool bStarted;                          //Confirms the Start() method has executed

    public Arena arena;

    [SyncVar]
    public int nRegisteredPlayers = 0;

	public int nPlayers = 2;

	public Player [] arPlayers;

	public Chr [,] arChrs;
    public Chr.CHARTYPE[,] arChrSelection;

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
            instance.Start();
		}
		return instance;
	}

	public Player GetLocalPlayer(){
		return arPlayers [0];
	}

    public void RequestPlayerID(Player plyr) {
        if (!isServer) {
            Debug.Log("We somehow called RequestPlayerId on a client");
            return;
        }
        arPlayers[nRegisteredPlayers] = plyr;
        plyr.SetID(nRegisteredPlayers);
        nRegisteredPlayers++;

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
			newChr.InitChr(player, id, new ChrKatara(newChr));
			break;
		case Chr.CHARTYPE.LANCER:
			newChr.InitChr(player, id, new ChrLancer(newChr));
			break;
		case Chr.CHARTYPE.SKELCOWBOY:
			newChr.InitChr(player, id, new ChrSkelCowboy(newChr));
			break;
        case Chr.CHARTYPE.SNEKGIRL:
            newChr.InitChr(player, id, new ChrSnekGirl(newChr));
            break;
        default: 
			Debug.LogError ("INVALID CHARACTER SELECTION");
			Application.Quit ();
			newChr.InitChr (player, id, new BaseChr(newChr)); //so the editor will let us compile
			break;
		}
		arChrs [player.id, id] = newChr;
        player.arChr[id] = newChr;
        newChr.plyrOwner = player;
        Debug.Log("Player owner is set");
	}

	void InitAllChrs(){
		for (int i = 0; i < nPlayers; i++) {
			for (int j = 0; j < Player.MAXCHRS; j++) {
				InitChr (arChrSelection [i,j], arPlayers[i], j);
			}
		}
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

    public void InitChrSelection() {
        arChrSelection = new Chr.CHARTYPE[2,3];

        arChrSelection[0,0] = Chr.CHARTYPE.KATARA;
        arChrSelection[0,1] = Chr.CHARTYPE.LANCER;
        arChrSelection[0,2] = Chr.CHARTYPE.SNEKGIRL;

        arChrSelection[1, 0] = Chr.CHARTYPE.SNEKGIRL;
        arChrSelection[1, 1] = Chr.CHARTYPE.SNEKGIRL;
        arChrSelection[1, 2] = Chr.CHARTYPE.SNEKGIRL;
    }

	public void Start(){
        if (bStarted) {
            return;
        }
        bStarted = true;

        gameObject.tag = "Match"; // So that anything can find this very quickly

        //TODO Do this connection in the editor
		arena = GetComponentInChildren<Arena> ();
		timeline = GetComponentInChildren<Timeline> ();
		controller = GetComponentInChildren<Controller> ();

        arPlayers = new Player[nPlayers];
        arChrs = new Chr[2, 3];

        arena.Start ();

		timeline.Start ();

        InitChrSelection();

		//InitPlayers (nPlayers);

		InitAllChrs ();

		//timeline.InitTimeline ();
	
		//TODO:: By default, automatically start evaluating turns

	}
}
