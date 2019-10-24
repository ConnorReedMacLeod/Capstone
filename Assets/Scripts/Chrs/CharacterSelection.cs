using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class CharacterSelection : SingletonPersistent<CharacterSelection> {

    public Chr.CHARTYPE[] arChrTeam1 = new Chr.CHARTYPE[Player.MAXCHRS] { Chr.CHARTYPE.FISCHER, Chr.CHARTYPE.KATARINA, Chr.CHARTYPE.PITBEAST };
    public Chr.CHARTYPE[] arChrTeam2 = new Chr.CHARTYPE[Player.MAXCHRS] { Chr.CHARTYPE.RAYNE, Chr.CHARTYPE.SAIKO, Chr.CHARTYPE.SOHPIDIA };



    public override void Init() {
        
    }

    //Use when player 2 realizes that their selections should shift to the player 2 slot
    public void SwapPlayerSelections() {

        Chr.CHARTYPE[] temp = arChrTeam1;
        arChrTeam1 = arChrTeam2;
        arChrTeam2 = temp;

    }

    public void SubmitSelection(int nPlayer) {

        switch (nPlayer) {

            case 1:
                int[] arnChrTeam1 = ArChrTypeToArInt(arChrTeam1);
                Debug.LogError("Sending selections for player " + nPlayer + " of " + arnChrTeam1[0] + ", " + arnChrTeam1[1] + ", " + arnChrTeam1[2]);
                NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitCharacters, new object[2] { 1, arnChrTeam1 });
                break;

            case 2:
                int[] arnChrTeam2 = ArChrTypeToArInt(arChrTeam2);
                Debug.LogError("Sending selections for player " + nPlayer + " of " + arnChrTeam2[0] + ", " + arnChrTeam2[1] + ", " + arnChrTeam2[2]);
                NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitCharacters, new object[2] { 2, arnChrTeam2 });
                break;

            default:
                Debug.Log("Unrecognized player number " + nPlayer);
                break;
        } 
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

    public void SaveSelections(int[][] _arCharacterSelections) {

        Debug.Log("Saving received selections");

        _arCharacterSelections[0].CopyTo(arChrTeam1, 0);
        _arCharacterSelections[1].CopyTo(arChrTeam2, 0);


    }
}
