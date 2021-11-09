using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownOwnerSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;


    public void Start() {
        //Ensure any pre-given value for the dropdown is accurately reflected
        MatchSetup.Get().arnLocalPlayerOwners[plyrselectorParent.idPlayer] = dropdown.value;

    }

    public void OnOwnerSelectChange() {

        MatchSetup.Get().arnLocalPlayerOwners[plyrselectorParent.idPlayer] = dropdown.value;

    }
}
