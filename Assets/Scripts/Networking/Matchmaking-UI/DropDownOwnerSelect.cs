using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownOwnerSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;


    public void Start() {

        //Start the match by reacting to 'selecting' whatever's defaultedly set here
        OnOwnerSelectChange();

    }

    public void OnOwnerSelectChange() {
        
        NetworkMatchSetup.SetPlayerOwner(plyrselectorParent.idPlayer, dropdown.value + 1); //Offset by one since our internalplayer ids start at 0

    }
}
