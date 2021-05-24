using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownInputSelect : MonoBehaviour {

    public PlayerSelector plyrselectorParent;
    public Dropdown dropdown;


    public void Start() {
        //Ensure any pre-given value for the dropdown is accurately reflected
        CharacterSelection.Get().arInputTypes[plyrselectorParent.idPlayer] = (Player.InputType)dropdown.value;

    }

    public void OnInputSelectChange(int nInputSelect) {

        CharacterSelection.Get().arInputTypes[plyrselectorParent.idPlayer] = (Player.InputType)nInputSelect;
        Debug.Log("Sending updated selections to the master for player " + plyrselectorParent.idPlayer);
        CharacterSelection.Get().SubmitSelection(plyrselectorParent.idPlayer);

    }
}
