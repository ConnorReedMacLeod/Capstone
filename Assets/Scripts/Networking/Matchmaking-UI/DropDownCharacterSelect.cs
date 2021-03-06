﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownCharacterSelect : MonoBehaviour {


    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;
    public int idChr;

    public void Start() {
        //Ensure any pre-given value for the dropdown is accurately reflected
        CharacterSelection.Get().arChrSelections[plyrselectorParent.idPlayer][idChr] = (Chr.CHARTYPE)dropdown.value;

    }

    public void OnChrSelectChange(int nChrSelect) {

        Debug.Assert(0 <= idChr && idChr < 3);

        CharacterSelection.Get().arChrSelections[plyrselectorParent.idPlayer][idChr] = (Chr.CHARTYPE)nChrSelect;
        Debug.Log("Sending updated selections to the master for player " + plyrselectorParent.idPlayer);
        CharacterSelection.Get().SubmitSelection(plyrselectorParent.idPlayer);

    }
}
