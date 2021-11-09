using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownOwnerSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;


    public void Start() {
        //Ensure we reflect the default matchparams selection
        dropdown.value = MatchSetup.Get().arnLocalPlayerOwners[plyrselectorParent.idPlayer] - 1;//client ids start at 1, so offset it back

    }

    public void OnOwnerSelectChange() {

        MatchSetup.Get().arnLocalPlayerOwners[plyrselectorParent.idPlayer] = dropdown.value + 1;

    }
}
