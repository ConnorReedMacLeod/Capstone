using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

// Will generally contain everything in a match
// responsible for initializing and containing components a match

public class Match : MonoBehaviour {

    bool bStarted;                          //Confirms the Start() method has executed

    public int nPlayers = 2;
    public Player[] arPlayers;

    public Chr[][] arChrs;

    public GameObject pfPlayer;
    public GameObject pfChr;

    //TODO: Move cursor settings into their own script
    public Texture2D txCursor;                          //Cursor Texture
    public CursorMode cursorMode = CursorMode.Auto;     //Cursor Mode
    public Vector2 v2HotSpot = Vector2.zero;            //Cursor Start Position

    public static Match instance;

    public static Match Get() {
        if(instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("Match");
            if(go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A MATCH TAG!");
            }
            instance = go.GetComponent<Match>();
            if(instance == null) {
                Debug.LogError("ERROR! MATCH TAGGED OBJECT DOES NOT HAVE A MATCH COMPONENT!");
            }
            instance.Start();
        }
        return instance;
    }

    void InitPlayers(int _nPlayers) {
        nPlayers = _nPlayers;//in case this needs to be changed based on the match
        arPlayers = new Player[nPlayers];
        arChrs = new Chr[nPlayers][];

        for(int i = 0; i < _nPlayers; i++) {
            GameObject goPlayer = Instantiate(pfPlayer, this.transform);
            Player newPlayer = goPlayer.GetComponent<Player>();
            if(newPlayer == null) {
                Debug.LogError("ERROR! NO PLAYER COMPONENT ON PLAYER PREFAB!");
            }

            newPlayer.SetID(i);
            newPlayer.Start();

            arPlayers[i] = newPlayer;
        }
    }

    // Will eventually need a clean solution to adding/removing characters
    // while managing ids - some sort of Buffer of unused views will probably help
    void InitChr(CharType.CHARTYPE chartype, Player player, int idChar, LoadoutManager.Loadout loadout) {

        GameObject goChr = Instantiate(pfChr, this.transform);
        Chr newChr = goChr.GetComponent<Chr>();

        if(newChr == null) {
            Debug.LogError("ERROR! NO CHR COMPONENT ON CHR PREFAB!");
        }

        newChr.Start();

        newChr.InitChr(chartype, player, idChar, loadout);

        arChrs[player.id][idChar] = newChr;
        player.arChr[idChar] = newChr;

    }

    void InitAllChrs() {

        for(int i = 0; i < nPlayers; i++) {
            arChrs[i] = new Chr[Player.MAXCHRS];
            arPlayers[i].nChrs = Player.MAXCHRS;

            for(int j = 0; j < arPlayers[i].nChrs; j++) {
                InitChr(NetworkMatchSetup.GetCharacterSelection(i, j),
                    arPlayers[i], j,
                    NetworkMatchSetup.GetLoadout(i,j));
            }
        }
        
    }

    public void AssignAllLocalInputControllers() {
        for (int i = 0; i < Player.MAXPLAYERS; i++) {
            AssignLocalInputController(Match.Get().arPlayers[i]);
        }
    }

    public void InitNetworking() {

        Debug.Log("Spawning networkcontroller");

        //Spawn the  client networking manager for our local player (and let the opponent spawn their own controller)
        GameObject goNetworkController = PhotonNetwork.Instantiate("Prefabs/Networking/pfNetworkController", Vector3.zero, Quaternion.identity);

        if(goNetworkController = null) {
            Debug.LogError("No prefab found for network controller");
        }
    }

    public void AssignLocalInputController(Player plyr) {

        //If the player isn't controlled locally, just set the plyr's controller to null since it's not our job to control them
        if (NetworkMatchSetup.IsLocallyOwned(plyr.id) == false) {
            plyr.SetInputType(LocalInputType.InputType.NONE);
        } else {
            //Otherwise, this character is controlled by this local client - figure out which input type they'll need and add it
            plyr.SetInputType(NetworkMatchSetup.GetInputType(plyr.id));
        }
    }

    public void InitAllChrPositions() {

        //Ensure all positions have been initialized properly
        ContPositions.Get().Start();

        //Set up each team in a 'triangle' - two sides in the back, center in the front
        ContPositions.Get().MoveChrToPosition(arChrs[0][0], ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoords(0, 0)));
        ContPositions.Get().MoveChrToPosition(arChrs[0][1], ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoords(0, 1)));
        ContPositions.Get().MoveChrToPosition(arChrs[0][2], ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoords(0, 2)));

        ContPositions.Get().MoveChrToPosition(arChrs[1][0], ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoords(1, 0)));
        ContPositions.Get().MoveChrToPosition(arChrs[1][1], ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoords(1, 1)));
        ContPositions.Get().MoveChrToPosition(arChrs[1][2], ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoords(1, 2)));

    }

    public IEnumerator SetupMatch() {

        while (NetworkMatchSetup.HasAllMatchSetupInfo() == false) {
            //Spin until we have all the match setup info that we need to start the match
            yield return null;
        }

        while (NetworkMatchSender.Get() == null) {
            //Spin until the needed networking objects have been spawned and given a chance to initialize
            // - in particular, the NetworkMatchSender needs to be ready in case we need to immediately send inputs
            //    as part of initializing the match (like for loading a log file)
            yield return null;
        }

        Debug.Log("Starting match initializations since we have enough information");

        ContRandomization.Get().InitGenerator(NetworkMatchSetup.GetRandomizationSeed());

        Debug.Log("Finished initializing the randomizer");

        InitPlayers(nPlayers);

        Debug.Log("Finished initializing players");

        //Initialize characters 
        InitAllChrs();

        Debug.Log("After InitAllChrs");

        //Assign local input controllers for each player
        AssignAllLocalInputControllers();

        Debug.Log("After assigning local input controllers");

        ContManaDistributer.Get().InitializeReserves();

        Debug.Log("After initializing mana reserves");

        InitAllChrPositions();

        Debug.Log("After initializing positions");

        //ContPositions.Get().PrintAllPositions();

        ContTurns.Get().InitializePriorities();

        Debug.Log("After InitializePriorities");

        //Check if the LogManager wants to load in any starting inputs
        LogManager.Get().LoadStartingInputs();

        Debug.Log("After LoadStartingInputs");

        LogManager.Get().InitMatchLog();

        Debug.Log("Finished initializing the log file");
    }


    public void Start() {
        if(bStarted) {
            return;
        }
        bStarted = true;

        gameObject.tag = "Match"; // So that anything can find this very quickly
        
        Cursor.SetCursor(txCursor, v2HotSpot, cursorMode);

        //Do all the match setup stuff (once it is ready)
        StartCoroutine(SetupMatch());
    }
}
