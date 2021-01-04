using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class CharacterSelection : SingletonPersistent<CharacterSelection> {

    //So that these can be easily configured in the Unity inspector
    public Chr.CHARTYPE[] arChrVIEWABLE1 = new Chr.CHARTYPE[Player.MAXCHRS];
    public Chr.CHARTYPE[] arChrVIEWABLE2 = new Chr.CHARTYPE[Player.MAXCHRS];

    public Chr.CHARTYPE[][] arChrSelections = new Chr.CHARTYPE[Player.MAXPLAYERS][];
    public int[] arnPlayerOwners = new int[Player.MAXPLAYERS];
    public Player.InputType[] arInputTypes = new Player.InputType[Player.MAXPLAYERS];

    public bool bSavedInputTypes;
    public bool bSavedOwners;
    public bool bSavedSelections;

    public override void Init() {

        bSavedSelections = false;

        arChrSelections[0] = new Chr.CHARTYPE[Player.MAXCHRS];
        arChrSelections[1] = new Chr.CHARTYPE[Player.MAXCHRS];
        arChrVIEWABLE1.CopyTo(arChrSelections[0], 0);
        arChrVIEWABLE2.CopyTo(arChrSelections[1], 0);

        Debug.Log("Finished CharacterSelection.Init");
    }



    //Send the signal to the master client that our locally saved character selection data, and player input data
    // is what should be used to initialize the game for this player
    public void SubmitSelection(int nPlayer) {

        Debug.Assert(0 <= nPlayer && nPlayer < Player.MAXCHRS);

        int[] arnTeamSelection = LibConversions.ArChrTypeToArInt(arChrSelections[nPlayer]);
        Debug.LogError("Sending selections for player " + nPlayer + " of " + arnTeamSelection[0] + ", " + arnTeamSelection[1] + ", " + arnTeamSelection[2] +
            "\nAnd input type: " + arInputTypes[nPlayer]);
        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitCharacters, new object[3] { nPlayer, arnTeamSelection, arInputTypes[nPlayer] });

    }


    // When the master has finalized all selections from players
    // he'll broadcast them out and we can save it here locally
    public void SaveSelections(int[][] _arChrSelections) {

        Debug.Log("Saving received selections");

        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            _arChrSelections[i].CopyTo(arChrSelections[i], 0);
        }

        bSavedSelections = true;

    }

    //When the master has finalized selections, he'll broadcast out 
    // which clients are controlling which players.
    public void SaveOwnerships(int[] _arnPlayerOwners) {

        Debug.Log("Saving received owners of players");

        _arnPlayerOwners.CopyTo(arnPlayerOwners, 0);

        bSavedOwners = true;

    }

    //When the master has finalized selections, he'll inform each
    // client about what input type each player is using
    public void SaveInputTypes(Player.InputType[] _arInputTypes) {

        Debug.Log("Saving received input types for each player");

        _arInputTypes.CopyTo(arInputTypes, 0);

        bSavedInputTypes = true;

    }

    public IEnumerator AssignAllLocalInputControllers() {
        //Sleep until we've recieved enough information to actually assign input controllers
        while(bSavedSelections == false || bSavedInputTypes == false || bSavedOwners == false) {
            Debug.Log("Waiting to assign input controllers until selections have all been recieved");
            yield return null;
        }

        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            AssignLocalInputController(Match.Get().arPlayers[i]);
        }
    }

    public void AssignLocalInputController(Player plyr) {

        //If the player isn't controlled locally, just set the plyr's controller to null since it's not our job to control them
        if(ClientNetworkController.Get().nLocalPlayerID != arnPlayerOwners[plyr.id]) {
            plyr.SetInputType(Player.InputType.NONE);
        } else {
            //Otherwise, this character is controlled by this local client - figure out which input type they'll need and add it
            plyr.SetInputType((Player.InputType)arInputTypes[plyr.id]);
        }
    }

}
