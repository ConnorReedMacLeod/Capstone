using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownCharacterSelect : MonoBehaviour {

    public int idPlayer;
    public int idChr;

    public void OnChrSelectChange(int nChrSelect) {

        Debug.Assert(0 <= idChr && idChr < 3);

        switch (idPlayer) {
            case 1:
                CharacterSelection.Get().arChrTeam1[idChr] = (Chr.CHARTYPE)nChrSelect;
                break;

            case 2:
                CharacterSelection.Get().arChrTeam2[idChr] = (Chr.CHARTYPE)nChrSelect;
                break;

            default:
                Debug.LogError("Unrecognized idChr " + idPlayer);
                break;
 
        }

    }
}
