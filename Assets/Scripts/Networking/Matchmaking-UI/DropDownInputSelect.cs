using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownInputSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;


    public void Start() {
        //Ensure any pre-given value for the dropdown is accurately reflected
        MatchSetup.Get().arLocalInputTypes[plyrselectorParent.idPlayer] = (Player.InputType)dropdown.value;

    }

    public void OnInputSelectChange() {

        MatchSetup.Get().arLocalInputTypes[plyrselectorParent.idPlayer] = (Player.InputType)dropdown.value;

    }
}
