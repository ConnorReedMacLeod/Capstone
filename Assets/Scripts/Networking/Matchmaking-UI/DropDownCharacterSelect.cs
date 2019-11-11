using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownCharacterSelect : MonoBehaviour {

    public int idPlayer;
    public int idChr;

    public void OnChrSelectChange(int nChrSelect) {

        Debug.Assert(0 <= idChr && idChr < 3);

        CharacterSelection.Get().arChrSelections[idPlayer][idChr] = (Chr.CHARTYPE)nChrSelect;

    }
}
