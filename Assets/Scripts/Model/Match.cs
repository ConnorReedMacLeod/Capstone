using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

// Will generally contain everything in a match
// responsible for initializing and containing components a match

public class Match : MonoBehaviour {

    public const int NPLAYERS = 2;
    public const int NMINACTIVECHRSPERTEAM = 3;
    public const int NINITIALCHRSPERTEAM = 5;
    public const int NCHRSPERDRAFT = 7;

    bool bStarted;                          //Confirms the Start() method has executed

    public Player[] arPlayers;

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

    void InitPlayers() {
        arPlayers = new Player[NPLAYERS];

        for(int i = 0; i < arPlayers.Length; i++) {
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
    void InitChr(CharType.CHARTYPE chartype, Player player, LoadoutManager.Loadout loadout) {

        GameObject goChr = Instantiate(pfChr, this.transform);
        Chr newChr = goChr.GetComponent<Chr>();

        if(newChr == null) {
            Debug.LogError("ERROR! NO CHR COMPONENT ON CHR PREFAB!");
        }

        newChr.Start();

        newChr.InitChr(chartype, player, loadout);

    }

    void InitAllChrs() {

        for(int j = 0; j < NINITIALCHRSPERTEAM; j++) {

            for(int i = 0; i < NPLAYERS; i++) {

                InitChr(NetworkMatchSetup.GetCharacterSelection(i, j),
                    arPlayers[i],
                    NetworkMatchSetup.GetLoadout(i, j));

            }

        }

    }

    public void AssignAllLocalInputControllers() {
        for(int i = 0; i < Match.NPLAYERS; i++) {
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
        if(NetworkMatchSetup.IsLocallyOwned(plyr.id) == false) {
            plyr.SetInputType(LocalInputType.InputType.NONE);
        } else {
            //Otherwise, this character is controlled by this local client - figure out which input type they'll need and add it
            plyr.SetInputType(NetworkMatchSetup.GetInputType(plyr.id));
        }
    }

    public void InitAllChrPositions() {

        //Ensure all positions have been initialized properly
        ContPositions.Get().Start();

        for(int i = 0; i < Match.NPLAYERS; i++) {
            List<Chr> lstChrsOwned = ChrCollection.Get().GetAllChrsOwnedBy(arPlayers[i]);

            //Set up each team according to the saved positions from the NetworkMatchSetup
            for(int j = 0; j < Match.NINITIALCHRSPERTEAM; j++) {
                ContPositions.Get().MoveChrToPosition(lstChrsOwned[i], ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoords(i, j)));
            }
        }

    }

    public IEnumerator SetupMatch() {

        while(NetworkMatchSetup.HasAllMatchSetupInfo() == false) {
            //Spin until we have all the match setup info that we need to start the match
            yield return null;
        }

        while(NetworkMatchSender.Get() == null) {
            //Spin until the needed networking objects have been spawned and given a chance to initialize
            // - in particular, the NetworkMatchSender needs to be ready in case we need to immediately send inputs
            //    as part of initializing the match (like for loading a log file)
            yield return null;
        }

        Debug.Log("Starting match initializations since we have enough information");

        ContRandomization.Get().InitGenerator(NetworkMatchSetup.GetRandomizationSeed());

        Debug.Log("Finished initializing the randomizer");

        InitPlayers();

        Debug.Log("Finished initializing players");

        //Initialize characters 
        InitAllChrs();

        Debug.Log("After InitAllChrs");

        //Assign local input controllers for each player
        AssignAllLocalInputControllers();

        Debug.Log("After assigning local input controllers");

        ContManaDistributer.Get().InitializeRandomReserves();

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
