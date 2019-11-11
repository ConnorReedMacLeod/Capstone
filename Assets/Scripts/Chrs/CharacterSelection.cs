using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class CharacterSelection : SingletonPersistent<CharacterSelection> {

    public Chr.CHARTYPE[] arChrVIEWABLE1 = new Chr.CHARTYPE[Player.MAXCHRS];
    public Chr.CHARTYPE[] arChrVIEWABLE2 = new Chr.CHARTYPE[Player.MAXCHRS];

    public Chr.CHARTYPE[][] arChrSelections = new Chr.CHARTYPE[Player.MAXPLAYERS][];

    public bool bSavedSelections;

    public override void Init() {
        arChrSelections[0] = new Chr.CHARTYPE[Player.MAXCHRS];
        arChrSelections[1] = new Chr.CHARTYPE[Player.MAXCHRS];
        arChrVIEWABLE1.CopyTo(arChrSelections[0], 0);
        arChrVIEWABLE2.CopyTo(arChrSelections[1], 0);

        Debug.Log("Finished initializing Character Selections");
    }

    //Use when player 2 realizes that their selections should shift to the player 2 slot
    public void SwapPlayerSelections() {

        Chr.CHARTYPE[] temp = arChrSelections[0];
        arChrSelections[0] = arChrSelections[1];
        arChrSelections[1] = temp;

    }

    public void SubmitSelection(int nPlayer) {

        Debug.Assert(0 <= nPlayer && nPlayer < Player.MAXCHRS);

        int[] arnTeamSelection = ArChrTypeToArInt(arChrSelections[nPlayer]);
        Debug.LogError("Sending selections for player " + nPlayer + " of " + arnTeamSelection[0] + ", " + arnTeamSelection[1] + ", " + arnTeamSelection[2]);
        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitCharacters, new object[2] { nPlayer, arnTeamSelection });

    }

    //TODO:: Eventually figure out if this can be generalized
    private int[] ArChrTypeToArInt(Chr.CHARTYPE[] _arChrTypes) {
        int[] arInt = new int[_arChrTypes.Length];

        for(int i=0; i<_arChrTypes.Length; i++) {
            arInt[i] = (int)_arChrTypes[i];
        }

        return arInt;
    }

    private Chr.CHARTYPE[] ArIntToArChrType(int[] _arInt) {
        Chr.CHARTYPE[] arChrTypes = new Chr.CHARTYPE[_arInt.Length];

        for (int i = 0; i < _arInt.Length; i++) {
            arChrTypes[i] = (Chr.CHARTYPE)_arInt[i];
        }

        return arChrTypes;
    }

    public void SaveSelections(int[][] _arChrSelections) {

        Debug.Log("Saving received selections");

        for(int i=0; i<Player.MAXPLAYERS; i++) {
            _arChrSelections[i].CopyTo(arChrSelections[i], 0);
        }

        bSavedSelections = true;

        Debug.Log("Saving Selections");
    }
}
