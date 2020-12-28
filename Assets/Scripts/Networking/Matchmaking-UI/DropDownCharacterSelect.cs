using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownCharacterSelect : MonoBehaviour {


    public PlayerSelector plyrselectorParent;
    public int idChr;

    public void OnChrSelectChange(int nChrSelect) {

        Debug.Assert(0 <= idChr && idChr < 3);

        CharacterSelection.Get().arChrSelections[plyrselectorParent.idPlayer][idChr] = (Chr.CHARTYPE)nChrSelect;

    }
}
