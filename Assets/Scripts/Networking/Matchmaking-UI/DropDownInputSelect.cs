using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownInputSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;


    public void Start() {
        //Ensure we reflect the default matchparams selection
        dropdown.value = (int)MatchSetup.Get().curMatchParams.arInputTypes[plyrselectorParent.idPlayer];

    }

    public void OnInputSelectChange() {

        MatchSetup.Get().curMatchParams.arInputTypes[plyrselectorParent.idPlayer] = (Player.InputType)dropdown.value;

    }
}
